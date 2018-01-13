using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{
    public class TclGenerator
    {
        public bool ExportNodalDisplacements;
        public bool ExportElementForces;
        public bool ExportTotalStiffness;

        public string nodesOut, elementsOut, stiffnessOut;

        public List<ElementToTclTranslator> ElementTranslators = new List<ElementToTclTranslator>();

        public int GetCounter(string name)
        {
            if (Counters.ContainsKey(name))
            {
                return Counters[name];
            }

            return 0;
        }

        public void SetCounter(string name, int value)
        {
            Counters[name] = value;
        }



        public Dictionary<string, int> Counters = new Dictionary<string, int>();

        public string Create(Model model, LoadCase loadCase)
        {
            nodesOut = ExportNodalDisplacements ? System.IO.Path.GetTempFileName() : null;
            elementsOut = ExportElementForces ? System.IO.Path.GetTempFileName() : null;
            stiffnessOut = ExportTotalStiffness ? System.IO.Path.GetTempFileName() : null;


            var Commands = new List<TclCommand>();
            var LoadCommands = new List<TclCommand>();

            model.ReIndexElements();


            var sb = new StringBuilder();

            Commands.Add(new TclCommand("wipe"));
            Commands.Add(new TclCommand("model", "BasicBuilder", "-ndm", "3", "-ndf", "6"));

            model.ReIndexNodes();

            foreach (var node in model.Nodes)
            {
                var cmd = new TclCommand("node", node.GetMemberValue("Index"),
                    node.Location.X, node.Location.Y, node.Location.Z
                    );

                Commands.Add(cmd);
            }


            foreach (var node in model.Nodes)
            {
                if (node.Constraints == Constraint.Released)
                    continue;

                var cmd = new TclCommand("fix", node.GetMemberValue("Index"),
                   node.Constraints.ToString_01()
                   );

                Commands.Add(cmd);
            }

            #region elements

            foreach (var element in model.Elements)
            {
                var trs = ElementTranslators.FirstOrDefault(i => i.CanTranslate(element));

                var cmd = trs.Translate(element);
                Commands.AddRange(cmd);
            }


            foreach (var element in model.MpcElements)
            {
                var trs = ElementTranslators.FirstOrDefault(i => i.CanTranslate(element));

                var cmd = trs.Translate(element);
                Commands.AddRange(cmd);
            }

            #endregion

            #region element Loads

            foreach (var element in model.Elements)
            {
                foreach (var load in element.Loads)
                {
                    throw new NotSupportedException();
                }
            }

            #endregion

            foreach (var node in model.Nodes)
            {
                if (node.Loads.Any())
                {
                    var sum =
                        node.Loads.Where(i => i.Case == loadCase).Select(i => i.Force).Aggregate((ii, jj) => ii + jj);

                    var cmd = new TclCommand("load", node.GetMemberValue("Index"),
                        sum.Fx, sum.Fy, sum.Fz,
                        sum.Mx, sum.My, sum.Mz
                        );

                    LoadCommands.Add(cmd);
                }
            }

            var n = model.Nodes.Count;

            if (ExportTotalStiffness)
                Commands.Add(new TclCommand("system", "FullGeneral"));
            else
                Commands.Add(new TclCommand("system", "BandSPD"));

            Commands.Add(new TclCommand("numberer", "Plain"));
            Commands.Add(new TclCommand("constraints", "Plain"));
            Commands.Add(new TclCommand("integrator", "LoadControl", "1.0"));
            Commands.Add(new TclCommand("algorithm", "Linear"));
            Commands.Add(new TclCommand("analysis", "Static"));

            if (ExportNodalDisplacements)
                Commands.Add(new TclCommand("recorder", "Node", "-xml", '"' + nodesOut.Replace("\\", "\\\\") + '"', "-dof", "1 2 3 4 5 6", "disp"));

            if (ExportElementForces)
                Commands.Add(new TclCommand("recorder", "Element", "-xml", '"' + elementsOut.Replace("\\", "\\\\") + '"', "-closeOnWrite", "strains", "material", "1"));

            Commands.Add(new TclCommand("analyze", "1"));

            if (ExportTotalStiffness)
                Commands.Add(new TclCommand("printA", "-file", '"' + stiffnessOut.Replace("\\", "\\\\") + '"'));

            foreach (var command in Commands)
            {
                sb.AppendLine(command.CommandName + " " + string.Join(" ", command.CommandArgs));
            }


            sb.AppendLine("pattern Plain 2 \"Linear\" {");
            foreach (var command in LoadCommands)
            {
                sb.AppendLine('\t' + command.CommandName + " " + string.Join(" ", command.CommandArgs));
            }
            sb.AppendLine("}");


            return sb.ToString();
        }


    }
}
