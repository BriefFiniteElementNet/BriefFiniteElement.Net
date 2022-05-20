using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Globalization;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{
    public class TetrahedronToTcl: ElementToTclTranslator
    {
        public override bool CanTranslate(StructurePart targetElement)
        {
            var elm = targetElement as Tetrahedral;

            if (elm == null)
                return false;

            return true;
        }

        public override TclCommand[] Translate(StructurePart targetElement, out string elementTag)
        {
            var elm = targetElement as Tetrahedral;

            //var sec = elm.Section as Sections.UniformParametric1DSection;

            //var mat = elm.Material as Materials.UniformIsotropicMaterial;
            //var g = mat.YoungModulus / (2 * (1 + mat.PoissonRatio));
            var e = elm.E;


            var buf = new List<TclCommand>();

            var eleTag = TargetGenerator.GetCounter("element");
            //var transTag = TargetGenerator.GetCounter("geomTransf");
            var matTag = TargetGenerator.GetCounter("nDMaterial");

            //var vec = elm.GetTransformationManager().TransformLocalToGlobal(Vector.K).GetUnit();

            buf.Add(new TclCommand("nDMaterial", "ElasticIsotropic", matTag,
                elm.E, elm.Nu));

            buf.Add(new TclCommand("element", "FourNodeTetrahedron", eleTag,
                elm.Nodes[0].GetIndex(),
                elm.Nodes[1].GetIndex(),
                elm.Nodes[2].GetIndex(),
                elm.Nodes[3].GetIndex(),

                matTag,0,0,0));

            TargetGenerator.SetCounter("element", eleTag + 1);
            TargetGenerator.SetCounter("nDMaterial", matTag + 1);

            elementTag = eleTag.ToString(CultureInfo.CurrentCulture);

            return buf.ToArray();
        }
    }
}
