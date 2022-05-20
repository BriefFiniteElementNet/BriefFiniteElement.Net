using System.Collections.Generic;
using System.Globalization;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{
    public class TetrahedralElementToTcl : ElementToTclTranslator
    {
        public override bool CanTranslate(StructurePart targetElement)
        {
            var elm = targetElement as TetrahedronElement;

            if (elm == null)
                return false;

            return true;
        }

        public override TclCommand[] Translate(StructurePart targetElement, out string elementTag)
        {
            var elm = targetElement as TetrahedronElement;

            var e = elm.Material.GetMaterialPropertiesAt(0, 0, 0).Ex;
            var nu = elm.Material.GetMaterialPropertiesAt(0, 0, 0).NuXy;


            var buf = new List<TclCommand>();

            var eleTag = TargetGenerator.GetCounter("element");
            //var transTag = TargetGenerator.GetCounter("geomTransf");
            var matTag = TargetGenerator.GetCounter("nDMaterial");

            //var vec = elm.GetTransformationManager().TransformLocalToGlobal(Vector.K).GetUnit();

            buf.Add(new TclCommand("nDMaterial", "ElasticIsotropic", matTag,
                e, nu));

            buf.Add(new TclCommand("element", "FourNodeTetrahedron", eleTag,
                elm.Nodes[0].GetIndex(),
                elm.Nodes[1].GetIndex(),
                elm.Nodes[2].GetIndex(),
                elm.Nodes[3].GetIndex(),

                matTag, 0, 0, 0));

            TargetGenerator.SetCounter("element", eleTag + 1);
            TargetGenerator.SetCounter("nDMaterial", matTag + 1);

            elementTag = eleTag.ToString(CultureInfo.CurrentCulture);

            return buf.ToArray();
        }
    }
}