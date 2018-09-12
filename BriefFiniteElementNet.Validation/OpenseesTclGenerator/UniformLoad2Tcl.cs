using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{
    public class UniformLoad2Tcl : ElementalLoadToTclTranslator
    {
        public override bool CanTranslate(Load targetLoad, Elements.Element targetElement)
        {
            if (targetLoad is Loads.UniformLoad)
                return true;

            return false;
        }

        public override TclCommand[] Translate(Load ld, Elements.Element targetElement,string targetElementTag)
        {
            var uld = (ld as Loads.UniformLoad);

            var elm = targetElement as Elements.BarElement;

            var localLoad =
                uld.CoordinationSystem == CoordinationSystem.Local ?
                uld.Direction :
                elm.GetTransformationManager().TransformGlobalToLocal(uld.Direction);

            localLoad = localLoad.GetUnit();

            var buf = new List<TclCommand>();

            buf.Add(new TclCommand("eleLoad", "-ele", targetElementTag,
                "-type", "-beamUniform",
                localLoad.Z * uld.Magnitude,
                localLoad.Y * uld.Magnitude, 
                localLoad.X * uld.Magnitude));

            return buf.ToArray();
        }
    }
}
