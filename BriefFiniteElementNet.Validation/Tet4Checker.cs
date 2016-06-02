using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    class Tet4Checker
    {
        static void Test1()
        {
            var nodes = System.IO.File.ReadAllText(@"Data\Tet4-78knv\Nodes-Position.txt") ;

            var elms = System.IO.File.ReadAllText(@"Data\Tet4-78knv\Elements-Nodes.txt");

            var nodesDisps = System.IO.File.ReadAllText(@"Data\Tet4-78knv\Nodes-Displacement.txt");


        }
    }
}
