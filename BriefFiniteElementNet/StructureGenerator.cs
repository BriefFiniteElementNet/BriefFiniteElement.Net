using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BriefFiniteElementNet.Elements;
using rnd=BriefFiniteElementNet.RandomHelper;
using System.Globalization;


namespace BriefFiniteElementNet
{
    public static class StructureGenerator
    {

        public static void AddRandomiseNodalLoads(Model mdl, params LoadCase[] cases)
        {
            foreach (var nde in mdl.Nodes)
                foreach (var cse in cases)
                {
                    nde.Loads.Add(new NodalLoad(RandomHelper.GetRandomForce(-1000, 1000), cse));
                }
        }

        public static void AddRandomiseNodalLoads(Model mdl,double min,double max, params LoadCase[] cases)
        {
            foreach (var nde in mdl.Nodes)
                foreach (var cse in cases)
                {
                    nde.Loads.Add(new NodalLoad(RandomHelper.GetRandomForce(min, max), cse));
                }
        }

        public static void AddRandomiseBeamUniformLoads(Model mdl, params LoadCase[] cases)
        {
            foreach (var elm in mdl.Elements)
                foreach (var cse in cases)
                {
                    var ul = new Loads.UniformLoad();

                    ul.Direction = RandomHelper.GetRandomVector(-10, 10);

                    ul.CoordinationSystem = RandomHelper.GetRandomBool() ?
                        CoordinationSystem.Local :
                        CoordinationSystem.Global;

                    ul.Magnitude = RandomHelper.GetRandomNumber(-100, 100);

                    elm.Loads.Add(ul);
                }
        }


        public static void AddRandomiseLoading(Model mdl, bool addNodalLoads, bool addElementLoads,
            params LoadCase[] cases)
        {
            if (addNodalLoads)
                foreach (var nde in mdl.Nodes)
                    foreach (var cse in cases)
                    {
                        nde.Loads.Add(new NodalLoad(RandomHelper.GetRandomForce(-1000, 1000), cse));
                    }

            if (addElementLoads)
                foreach (var elm in mdl.Elements)
                    foreach (var cse in cases)
                    {
                        var uniformLoad =
                            new UniformLoad1D(rnd.GetRandomNumber(-1000, 1000), LoadDirection.X,
                                CoordinationSystem.Global,
                                cse);

                        var l = (elm.Nodes[0].Location - elm.Nodes[1].Location).Length;

                        var concenstratedLoad = new ConcentratedLoad1D(rnd.GetRandomForce(-1000, 1000),
                            rnd.GetRandomNumber(0, l), CoordinationSystem.Global, cse);


                        elm.Loads.Add(uniformLoad);
                        elm.Loads.Add(concenstratedLoad);
                    }


        }

        public static void SetRandomiseConstraints(Model mdl)
        {
            foreach (var nde in mdl.Nodes)
                nde.Constraints = rnd.GetRandomConstraint();
        }

        public static void SetRandomiseSections(Model mdl)
        {
            foreach (var ele in mdl.Elements)
            {
                if (ele is Elements.BarElement)
                {
                    var br = ele as Elements.BarElement;

                    var sec = new Sections.UniformParametric1DSection();

                    var b = RandomHelper.GetRandomNumber(0.03, 0.05);
                    var h = RandomHelper.GetRandomNumber(0.03, 0.05);

                    sec.A = b * h;
                    sec.Iy = b * b * b * h / 12;
                    sec.Iz = h * h * h * b / 12;
                    sec.Iyz = (sec.Iy + sec.Iz) / 2;//!!! 

                    br.Section = sec;
                }

                if (ele is Elements.TriangleElement)
                {
                    var tri = ele as Elements.TriangleElement;

                    var sec = new Sections.UniformParametric2DSection();

                    var t = RandomHelper.GetRandomNumber(0.03, 0.05);
                    sec.T = t;

                    tri.Section = sec;
                }
            }
        }

        public static void SetRandomiseMaterial(Model mdl)
        {
            var e = RandomHelper.GetRandomNumber(0.9, 1.1) * 210e9;
            var nu = RandomHelper.GetRandomNumber(0.25, 0.3) ;
            
            foreach (var ele in mdl.Elements)
            {
                
                if (ele is Elements.BarElement)
                {
                    var br = ele as Elements.BarElement;

                   
                    br.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e,nu);
                }

                if (ele is Elements.TriangleElement)
                {
                    var tri = ele as Elements.TriangleElement;

                    tri.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);
                }
            }
        }

        public static void AddRandomDisplacements(Model mdl,double max)
        {


            foreach (var nde in mdl.Nodes)
            {
                var disp = rnd.GetRandomDisplacement(0, max);

                nde.Location = nde.Location + disp.Displacements;
            }
        }

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


        public static Model Generate3DFrameElementGrid(int m, int n, int l)
        {
            var buf = new Model();

            var dx = 1.0;
            var dy = 1.0;
            var dz = 1.0;

            var nodes = new Node[m, n, l];

            for (int k = 0; k < l; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {

                        var pos = new Point(i*dx, j*dy, k*dz);
                        var nde = new Node() {Location = pos};
                        buf.Nodes.Add(nde);
                        nodes[j, i, k] = nde;
                    }
                }
            }

            for (int k = 0; k < l - 1; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var elm = new FrameElement2Node();
                        elm.StartNode = nodes[j, i, k];
                        elm.EndNode = nodes[j, i, k + 1];
                        buf.Elements.Add(elm);
                    }
                }
            }


            for (int i = 0; i < n - 1; i++)
            {
                for (int k = 0; k < l; k++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var elm = new FrameElement2Node();
                        elm.StartNode = nodes[j, i, k];
                        elm.EndNode = nodes[j, i + 1, k];
                        buf.Elements.Add(elm);
                    }
                }
            }

            for (int j = 0; j < m-1; j++)
            {
                for (int k = 0; k < l; k++)
                {
                    for (int i = 0; i < n; i++) 
                        
                    {
                        var elm = new FrameElement2Node();
                        elm.StartNode = nodes[j, i, k];
                        elm.EndNode = nodes[j+1, i , k];
                        buf.Elements.Add(elm);
                        
                    }
                }
            }

            foreach (var elm in buf.Elements)
            {
                var framElm = elm as FrameElement2Node;

                if (framElm == null)
                    continue;

                framElm.A = 7.64*1e-4;// 0.01;
                framElm.Iy = framElm.Iz = framElm.J = 80*1e-8;// 0.1 * 0.1 * 0.1 * 0.1 / 12.0;
                framElm.E = framElm.G = 210e9;
                framElm.MassDensity = 7800;
            }


            for (int i = 0; i < n*m; i++)
            {
                buf.Nodes[i].Constraints = Constraint.Fixed;
            }


            return buf;
        }

        public static Model Generate3DBarElementGrid(int m, int n, int l,Func<BarElement> factory)
        {
            var buf = new Model();

            var dx = 5.0;
            var dy = 4.0;
            var dz = 3.0;

            var nodes = new Node[m, n, l];

            for (int k = 0; k < l; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {

                        var pos = new Point(i * dx, j * dy, k * dz);
                        var nde = new Node() { Location = pos };
                        buf.Nodes.Add(nde);
                        nodes[j, i, k] = nde;
                    }
                }
            }

            for (int k = 0; k < l - 1; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var elm = factory();// new BarElement();
                        elm.StartNode = nodes[j, i, k];
                        elm.EndNode = nodes[j, i, k + 1];
                        buf.Elements.Add(elm);
                    }
                }
            }


            for (int i = 0; i < n - 1; i++)
            {
                for (int k = 0; k < l; k++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var elm = factory();//new BarElement();
                        elm.StartNode = nodes[j, i, k];
                        elm.EndNode = nodes[j, i + 1, k];
                        buf.Elements.Add(elm);
                    }
                }
            }

            for (int j = 0; j < m - 1; j++)
            {
                for (int k = 0; k < l; k++)
                {
                    for (int i = 0; i < n; i++)

                    {
                        var elm = factory();//new BarElement();
                        elm.StartNode = nodes[j, i, k];
                        elm.EndNode = nodes[j + 1, i, k];
                        buf.Elements.Add(elm);

                    }
                }
            }

            foreach (var elm in buf.Elements)
            {
                var framElm = elm as BarElement;


                if (framElm == null)
                    continue;

                framElm.Behavior = BarElementBehaviours.FullFrame;


                var h = RandomHelper.GetRandomNumber(0.01, 0.1);
                var w = RandomHelper.GetRandomNumber(0.01, 0.1);

                var a = h * w;
                var iy = h * h * h * w / 12;
                var iz = w * w * w * h / 12;
                var j = iy + iz;
                var e= RandomHelper.GetRandomNumber(100e9, 200e9);
                var nu = RandomHelper.GetRandomNumber(0.2, 0.3);

                var sec = (Sections.UniformParametric1DSection)(framElm.Section = new Sections.UniformParametric1DSection());
                var mat = framElm.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

                sec.A = a;
                sec.Iy = iy;
                sec.Iz = iz;
                sec.Iyz = j;

            }


            for (int i = 0; i < n * m; i++)
            {
                buf.Nodes[i].Constraints = Constraint.Fixed;
            }


            return buf;
        }

        public static Model Generate3DBarElementGrid(int m, int n, int l)
        {
            var fnc = new Func<BarElement>(() => new BarElement());

            return Generate3DBarElementGrid(m, n, l, fnc);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m">number of levels in X direction</param>
        /// <param name="n">number of levels in Y direction</param>
        /// <param name="l">number of levels in Z direction</param>
        /// <returns></returns>
        public static Model Generate3DTriangleElementGrid(int m, int n, int l)
        {
            var buf = new Model();

            var dx = 1.0;
            var dy = 1.0;
            var dz = 1.0;

            var nodes = new Node[m, n, l];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < l; k++)
                    {
                        var pos = new Point(j * dx, i * dy, k * dz);
                        var nde = new Node() { Location = pos };
                        buf.Nodes.Add(nde);
                        nodes[i, j, k] = nde;
                    }
                }
            }

            //elements parallel to XZ
            for (int j = 0; j < n; j++) 
            {
                for (int i = 0; i < m - 1; i++)
                {
                    for (int k = 0; k < l - 1; k++)
                    {
                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j, k];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i + 1, j, k];

                            buf.Elements.Add(elm2);
                        }

                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i+1, j, k+1];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i + 1, j, k];

                            buf.Elements.Add(elm2);
                        }

                    }
                }
            }


            //elements parallel to YZ
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    for (int k = 0; k < l - 1; k++)
                    {
                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j, k];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j + 1, k + 1];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                    }
                }
            }

            //elements parallel to XY
            for (int k = 0; k < l; k++) 
            {
                for (int j = 0; j < n - 1; j++)
                {
                    for (int i = 0; i < m-1; i++)
                    {
                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j, k];
                            elm2.Nodes[1] = nodes[i + 1, j, k ];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i + 1, j + 1, k];
                            elm2.Nodes[1] = nodes[i + 1, j, k ];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                    }
                }
            }


            foreach (var elm in buf.Elements)
            {
                var triElm = elm as TriangleElement;


                if (triElm == null)
                    continue;

                triElm.Behavior = TriangleElementBehaviours.Shell;



                var h = RandomHelper.GetRandomNumber(0.01, 0.1);
                var w = RandomHelper.GetRandomNumber(0.01, 0.1);

                var e = RandomHelper.GetRandomNumber(100e9, 200e9);
                var nu = RandomHelper.GetRandomNumber(0.2, 0.3);

                var sec = (Sections.UniformParametric2DSection)(triElm.Section = new Sections.UniformParametric2DSection());

                var mat = triElm.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

                sec.T = 0.1;

            }


            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    nodes[i, j, 0].Constraints = Constraint.Fixed;
                }
            }




            return buf;
        }

        public static Model Generate3DTet4Grid(int m, int n, int l)
        {
            var buf = new Model();

            var dx = 1.0;
            var dy = 1.0;
            var dz = 1.0;

            var nodes = new Node[m, n, l];

            for (int k = 0; k < l; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {

                        var pos = new Point(i * dx, j * dy, k * dz);
                        var nde = new Node() { Location = pos };
                        buf.Nodes.Add(nde);

                        nde.Constraints = Constraint.RotationFixed;

                        nodes[j, i, k] = nde;
                    }
                }
            }


            var elm=new Func<Node, Node, Node, Node,Tetrahedral>((n1, n2, n3, n4) =>
            {
                var buff = new Tetrahedral();

                buff.Nodes[0] = n1;
                buff.Nodes[1] = n2;
                buff.Nodes[2] = n3;
                buff.Nodes[3] = n4;

                buff.E = 210e9;
                buff.Nu = 0.33;
                
                return buff;
            });

            var elms = new List<Element>();

            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n-1; j++)
                {
                    for (int k = 0; k < l - 1; k++)
                    {
                        var ns = new Node[] {
                            nodes[i, j, k],
                            nodes[i+1, j, k],
                            nodes[i+1, j+1, k],
                            nodes[i, j+1, k],

                            nodes[i, j, k+1],
                            nodes[i+1, j, k+1],
                            nodes[i+1, j+1, k+1],
                            nodes[i, j+1, k+1],
                        };


                        elms.Add(elm(ns[0], ns[1], ns[3], ns[4]));
                        elms.Add(elm(ns[2], ns[1], ns[3], ns[6]));
                        elms.Add(elm(ns[1], ns[3], ns[4], ns[6]));

                        elms.Add(elm(ns[4], ns[5], ns[6], ns[1]));
                        elms.Add(elm(ns[4], ns[6], ns[7], ns[3]));

                    }
                }
            }
            
            buf.Elements.Add(elms.ToArray());

            for (int i = 0; i < n * m; i++)
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
                mdl.Elements[i].Label = string.Format(CultureInfo.CurrentCulture, "e-{0}", i);
            }


            for (int i = 0; i < mdl.Nodes.Count; i++)
            {
                mdl.Nodes[i].Label = string.Format(CultureInfo.CurrentCulture, "n-{0}", i);
            }
        }


        
    }
}
