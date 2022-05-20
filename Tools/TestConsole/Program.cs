using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Resolvers;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.FemUtilies;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Sections;
using CSparse;
using CSparse.Double.Factorization;
using CSparse.Storage;
using BriefFiniteElementNet.Common;
using CSparse.Double;
using System.Globalization;

namespace BriefFiniteElementNet.TestConsole
{
    class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "BFE tests & temporary codes";

            
        }
    }
}
