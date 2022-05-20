using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class TowerExample
    {
        public class Tower
        {
            public List<Member> Members;

            public List<Node> Nodes;
        }

        public class Member
        {
            public double SectionArea { get; set; }

            public Node StartNode { get; set; }

            public Node EndNode { get; set; }
        }

        
    }
}
