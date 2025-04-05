using BriefFiniteElementNet.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Class1.T();

            var buf = new BarElementTester().DoPopularValidation();
            BriefFiniteElementNet.Validation.Ui.Program.ExportToHtmFile("c:\\temp\\fil.html", buf);



            //SupportReactionCalculator.Run();
        }
    }
}
