using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BriefFiniteElementNet.Validation;

namespace BriefFiniteElementNet.TestConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //FlatShellElementChecker.Test1();

            var model = StructureGenerator.Generate3DGrid(5, 5, 5);
            //StructureGenerator.AddRandomiseLoading(model, LoadCase.DefaultLoadCase);

            StructureGenerator.AddRandomiseLoading(model,true,false, LoadCase.DefaultLoadCase);


            new Frame3DDValidator(model).Validate();


            Console.ReadKey();
        }

       
    }
}
