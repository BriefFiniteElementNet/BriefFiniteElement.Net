using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{
    public class TriangleElementToTcl : ElementToTclTranslator
    {
        public override bool CanTranslate(StructurePart targetElement)
        {
            var elm = targetElement as TriangleElement;

            if (elm == null)
                return false;

            var isUPara = elm.Section is Sections.UniformParametric2DSection;

            if (!isUPara )
                return false;

            if (!(elm.Material is Materials.UniformIsotropicMaterial))
                return false;

            return true;
        }

        public override TclCommand[] Translate(StructurePart targetElement, out string elementTag)
        {
            var elm = targetElement as TriangleElement;
            var sec = elm.Section.GetThicknessAt(0);// as Sections.UniformParametric1DSection;
            var mat = elm.Material as Materials.UniformIsotropicMaterial;
            var g = mat.YoungModulus / (2 * (1 + mat.PoissonRatio));


            var buf = new List<TclCommand>();

            var eleTag = TargetGenerator.GetCounter("element");
            var secTag = TargetGenerator.GetCounter("section");

            var vec = elm.GetTransformationManager().TransformLocalToGlobal(Vector.K).GetUnit();

            buf.Add(new TclCommand("section", "ElasticMembranePlateSection", secTag,
                mat.YoungModulus, mat.PoissonRatio, sec, 0));

            buf.Add(new TclCommand("element", "ShellDKGT", eleTag,
                elm.Nodes[0].GetIndex(),
                elm.Nodes[1].GetIndex(),
                elm.Nodes[2].GetIndex(),
                secTag));

            TargetGenerator.SetCounter("element", eleTag + 1);
            TargetGenerator.SetCounter("section", secTag + 1);

            elementTag = eleTag.ToString(CultureInfo.CurrentCulture);

            return buf.ToArray();
        }
    }

    public class TriangleElementToTri3Tcl : ElementToTclTranslator
    {
        public override bool CanTranslate(StructurePart targetElement)
        {
            var elm = targetElement as TriangleElement;

            if (elm == null)
                return false;

            var isUPara = elm.Section is Sections.UniformParametric2DSection;

            if (!isUPara)
                return false;

            if (!(elm.Material is Materials.UniformIsotropicMaterial))
                return false;

            return true;
        }

        public override TclCommand[] Translate(StructurePart targetElement, out string elementTag)
        {
            var elm = targetElement as TriangleElement;
            var sec = elm.Section.GetThicknessAt(0);// as Sections.UniformParametric1DSection;
            var mat = elm.Material as Materials.UniformIsotropicMaterial;
            var g = mat.YoungModulus / (2 * (1 + mat.PoissonRatio));


            var buf = new List<TclCommand>();

            var eleTag = TargetGenerator.GetCounter("element");
            var matTag = TargetGenerator.GetCounter("nDMaterial");

            buf.Add(new TclCommand("nDMaterial", "ElasticIsotropic", matTag, mat.YoungModulus, mat.PoissonRatio));

            buf.Add(new TclCommand("element", "tri31 ", eleTag,
                elm.Nodes[0].GetIndex(),
                elm.Nodes[1].GetIndex(),
                elm.Nodes[2].GetIndex(),
                sec, 
                "PlaneStrain",
                matTag));

            TargetGenerator.SetCounter("element", eleTag + 1);
            TargetGenerator.SetCounter("nDMaterial", matTag + 1);

            elementTag = eleTag.ToString(CultureInfo.CurrentCulture);

            return buf.ToArray();
        }
    }
}
