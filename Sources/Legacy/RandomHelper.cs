namespace BriefFiniteElementNet.Legacy
{
    public static class RandomHelper
    {
        public static Model GetRandomFrameModel(int n = 50)
        {
            var buf = new Model();

            var nodes = new Node[n];

            for (var i = 0; i < n; i++)
            {
                nodes[i] = new Node(BriefFiniteElementNet.RandomHelper.GetRandomNumber(0, 10), BriefFiniteElementNet.RandomHelper.GetRandomNumber(0, 10), BriefFiniteElementNet.RandomHelper.GetRandomNumber(0, 10));

                nodes[i].Constraints = BriefFiniteElementNet.RandomHelper.GetRandomConstraint();
            }

            buf.Nodes.Add(nodes);

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (i == j)
                        continue;

                    var elm = new FrameElement2Node(nodes[i], nodes[j]);
                    elm.A = BriefFiniteElementNet.RandomHelper.GetRandomNumber();
                    elm.Iy = BriefFiniteElementNet.RandomHelper.GetRandomNumber();
                    elm.Iz = BriefFiniteElementNet.RandomHelper.GetRandomNumber();
                    elm.J = BriefFiniteElementNet.RandomHelper.GetRandomNumber();

                    elm.E = BriefFiniteElementNet.RandomHelper.GetRandomNumber();
                    elm.G = BriefFiniteElementNet.RandomHelper.GetRandomNumber();

                    var ld = new UniformLoad1D(BriefFiniteElementNet.RandomHelper.GetRandomNumber(100, 1000), LoadDirection.Z, CoordinationSystem.Global);

                    elm.Loads.Add(ld);

                    buf.Elements.Add(elm);
                }
            }

            for (int i = 0; i < n; i++)
            {
                nodes[i].Loads.Add(new NodalLoad(BriefFiniteElementNet.RandomHelper.GetRandomForce(1000, 10000)));
            }

            return buf;
        }
    }
}