using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class IndividualTests
    {
        public static void TestNoFreeDof()
        {
            var model = new Model();

            var n1 = new Node(0, 0, 0);

            var n2 = new Node(1, 0, 0);

            var elm = new FrameElement2Node(n1, n2);

            n1.Constraints = n2.Constraints = Constraint.Fixed;

            model.Nodes.Add(n1, n2);

            model.Elements.Add(elm);

            model.Solve();
        }
    }
}
