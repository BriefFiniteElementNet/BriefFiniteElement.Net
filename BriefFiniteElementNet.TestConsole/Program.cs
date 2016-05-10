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
            DkqElementChecker.Test1();
        }
    }
}
