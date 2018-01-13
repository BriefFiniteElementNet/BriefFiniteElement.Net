using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation.OpenseesTclGenerator
{

    public class TclCommand
    {
        public TclCommand()
        {

        }

        public TclCommand(string commandName, params object[] commandArgs)
        {
            CommandArgs = commandArgs.Select(i=>i.ToString()).ToArray();
            CommandName = commandName;
        }

        public string CommandName;
        public string[] CommandArgs;
    }

    public abstract class ElementToTclTranslator
    {
        public TclGenerator TargetGenerator;

        public abstract TclCommand[] Translate(StructurePart targetElement);

        public abstract bool CanTranslate(StructurePart targetElement);
    }

    public abstract class ElementalLoadToTclTranslator
    {
        public TclGenerator TargetGenerator;

        public abstract TclCommand[] Translate(Load targetElement);

        public abstract bool CanTranslate(Load targetElement);
    }
}
