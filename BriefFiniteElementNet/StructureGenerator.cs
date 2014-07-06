using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public static class StructureGenerator
    {
        public static Model GenerateRandomStructure(int nodeCount)
        {
            var rnd = new Random();

            var buf = new Model();

            for (int i = 0; i < nodeCount; i++)
            {
                var nde = new Node() { Location = new Point(rnd.NextDouble() * 100, rnd.NextDouble() * 100, rnd.NextDouble() * 100) };
                buf.Nodes.Add(nde);
            }


            for (var i = 0; i < nodeCount-1; i++)
            {
                var framElm = new FrameElement2Node() {StartNode = buf.Nodes[i], EndNode = buf.Nodes[i + 1]};
                framElm.A = 0.01;
                framElm.Iy = framElm.Iz = framElm.J = 0.1*0.1*0.1*0.1/12;
                framElm.E = framElm.G = 210e9;

                buf.Elements.Add(framElm);
            }

            return buf;
        }


        public static Model Generate3DGrid(int m, int n, int l)
        {
            var buf = new Model();

            var dx = 1.0;
            var dy = 1.0;
            var dz = 1.0;

            for (int k = 0; k < l; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var pos = new Point(i*dx, j*dy, k*dz);
                        var nde = new Node() {Location = pos};
                        buf.Nodes.Add(nde);
                    }
                }
            }


            for (int k = 0; k < l-1; k++)
            {
                for (int num = 0; num < n*m; num++)
                {
                    var elm = new FrameElement2Node();
                    elm.StartNode = buf.Nodes[k * n * m + num];
                    elm.EndNode = buf.Nodes[(k + 1)*n*m + num];
                    buf.Elements.Add(elm);
                }
            }


            foreach (var elm in buf.Elements)
            {
                var framElm = elm as FrameElement2Node;

                if (framElm == null)
                    continue;

                framElm.A = 0.01;
                framElm.Iy = framElm.Iz = framElm.J = 0.1 * 0.1 * 0.1 * 0.1 / 12;
                framElm.E = framElm.G = 210e9;

            }


            for (int i = 0; i < n*m; i++)
            {
                buf.Nodes[i].Constraints = Constraint.Fixed;
            }


            return buf;
        }

        public static Model GenerateSimpleBeam(int nodes)
        {
            var delta = 1.0;

            var buf = new Model();

            for (int i = 0; i < nodes; i++)
            {
                buf.Nodes.Add(new Node() {Location = new Point(i*delta, 0, 0)});
            }

            for (int i = 0; i < nodes-1; i++)
            {
                var start = buf.Nodes[i];
                var end = buf.Nodes[i+1];
                var elm = new FrameElement2Node() {StartNode = start, EndNode = end};
                buf.Elements.Add(elm);
            }

            foreach (var elm in buf.Elements)
            {
                var framElm = elm as FrameElement2Node;

                if (framElm == null)
                    continue;

                framElm.A = 0.01;
                framElm.Iy = framElm.Iz = 0.1 * 0.1 * 0.1 * 0.1 / 12;
                framElm.J = 2*0.1*0.1*0.1*0.1/12;
                framElm.E = 210e9;
                var no = 0.3;
                framElm.G = framElm.E/(2*(1 + no));
            }

            buf.Nodes[0].Constraints = Constraint.Fixed;

            TagModel(buf);
            return buf;
        }

        public static void TagModel(Model mdl)
        {
            for (int i = 0; i < mdl.Elements.Count; i++)
            {
                mdl.Elements[i].Label = string.Format("e-{0}", i);
            }


            for (int i = 0; i < mdl.Nodes.Count; i++)
            {
                mdl.Nodes[i].Label = string.Format("n-{0}", i);
            }
        }
    }
}
