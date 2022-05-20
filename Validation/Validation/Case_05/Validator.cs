using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;
using HtmlTags;

namespace BriefFiniteElementNet.Validation.Case_05
{
    [ValidationCase("Console beam with tetrahedron", typeof(RigidElement_MPC))]
    public class Validator : IValidationCase
    {
        public ValidationResult Validate()
        {
            var model = StructureGenerator.Generate3DTetrahedralElementGrid(2, 2, 50);

            RigidElement_MPC mpc;
            
            {
                var e = 210e9;


                foreach (var elm in model.Elements)
                {
                    if (elm is TetrahedronElement)
                    {
                        var tet = elm as TetrahedronElement;

                        tet.Material = new Materials.UniformIsotropicMaterial(e, 0.25);
                    }
                }

                var dx = model.Nodes.Max(i => i.Location.X) - model.Nodes.Min(i => i.Location.X);
                var dy = model.Nodes.Max(i => i.Location.Y) - model.Nodes.Min(i => i.Location.Y);
                var dz = model.Nodes.Max(i => i.Location.Z) - model.Nodes.Min(i => i.Location.Z);

                var l = dz;// model.Nodes.Max(i => i.Location.Z);

                var cnt = model.Nodes.Where(i => i.Location.Z == l);

                var f = 1e7;
                var I = dy * dx * dx * dx / 12;
                var rigid = mpc = new MpcElements.RigidElement_MPC() {UseForAllLoads = true};

                foreach (var node in cnt)
                {
                    node.Loads.Add(new NodalLoad(new Force(f / cnt.Count(), 0, 0, 0, 0, 0)));
                    rigid.Nodes.Add(node);

                    node.Constraints = Constraints.Released;
                }

                model.MpcElements.Add(rigid);
                model.Trace.Listeners.Add(new ConsoleTraceListener());
                model.Solve_MPC();


                var delta = f * l * l * l / (3 * e * I);

                var t = cnt.FirstOrDefault().GetNodalDisplacement();

                var ratio = delta / t.DX;
            }
            

            {
                var span = new HtmlTag("span");
                span.Add("p").Text("Validation of rigid element");
                span.Add("paragraph").Text("The rigid element connects different nodes through non-deformable elements. ");
                span.Add("h3").Text("Model Definition");

                span.Add("paragraph").Text(string.Format(CultureInfo.CurrentCulture, "A console beam with tetrahedron, at the end (tip of console) nodes are connected with a rigid element.")).AddClosedTag("br");

                span.Add("paragraph").Text("The start of beam is totally fixed in the 3D space.").AddClosedTag("br");
                span.Add("paragraph").Text("The end of beam is loaded with a vertical load.").AddClosedTag("br");
                span.Add("paragraph").Text("All nodes connected to rigid element should have a rigid body displacement.").AddClosedTag("br");

                var mpcNodes = mpc.Nodes.ToArray();

                var disps = mpcNodes.Select(i => Tuple.Create(i.Location, i.GetNodalDisplacement()));


                span.Add("h3").Text("Validation Result");

                if (disps.Any(i => double.IsNaN(i.Item2.DX)|| double.IsNaN(i.Item2.DY)|| double.IsNaN(i.Item2.DZ)|| double.IsNaN(i.Item2.RX)))
                {
                    span.Add("paragraph").Add("h1").Text(string.Format("Validation failed, there are NaNs in the result for nodal displacements"));
                }
                
                span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:"));
                var n1 = model.Nodes[1].GetNodalDisplacement();
                var n2 = model.Nodes[2].GetNodalDisplacement();
                var diff = n1 - n2;
                //linear beam with loads -> rigid elements introduces rigid body -> Y and Z (translations perpendicular to the beam) are non zero and scaled. Set to zero to avoid confusion with result.
                diff.DY = diff.DZ = 0.0;

                //span.Add("p").AddClass("bg-info").Text(string.Format("-Max ABSOLUTE Error: {0:e3}", diff));//htmltags cannot encode the delta and teta chars so will use vector length
                span.Add("p").AddClass("bg-info")
                    .Text(string.Format("-Max ABSOLUTE Error: Displacement:{0:e3} , Rotation:{1:e3}",
                        diff.Displacements.Length, diff.Rotations.Length));

                var buf = new ValidationResult();
                buf.Span = span;
                buf.Title = "Rigid element Validation, Console beam with tetrahedron";

                return buf;
            }
        }
    }
}
