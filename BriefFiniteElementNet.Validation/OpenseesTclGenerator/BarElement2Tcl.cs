using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{
    public class BarElement2Tcl : ElementToTclTranslator
    {
        public override bool CanTranslate(StructurePart targetElement)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                return false;

            if (!(elm.Section is Sections.UniformParametric1DSection))
                return false;

            if (!(elm.Material is Materials.UniformIsotropicMaterial))
                return false;

            return true;
        }

        public override TclCommand[] Translate(StructurePart targetElement)
        {
            var elm = targetElement as BarElement;
            var sec = elm.Section as Sections.UniformParametric1DSection;
            var mat = elm.Material as Materials.UniformIsotropicMaterial;
            var g = mat.YoungModulus / (2 * (1 + mat.PoissonRatio));


            var buf = new List<TclCommand>();

            var eleTag = TargetGenerator.GetCounter("element");
            var transTag = TargetGenerator.GetCounter("geomTransf");

            var vec = elm.GetTransformationManager().TransformLocalToGlobal(Vector.I);

            buf.Add(new TclCommand("geomTransf", "Linear ", transTag,
                vec.X, vec.Y, vec.Z));

            buf.Add(new TclCommand("element", "elasticBeamColumn", eleTag,
                elm.StartNode.GetIndex(),
                elm.EndNode.GetIndex(),
                sec.A, mat.YoungModulus,g, sec.Iz, sec.Iy, "$transfTag"));

            TargetGenerator.SetCounter("element", eleTag + 1);
            TargetGenerator.SetCounter("geomTransf", transTag + 1);

            return buf.ToArray();
        }
    }
}
