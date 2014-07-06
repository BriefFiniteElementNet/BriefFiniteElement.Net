using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using BriefFiniteElementNet;
using GlobalUtils.Core;

namespace TemporaryConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TestTruss();
            var t = "".GetHashCode();
            
            var model = StructureGenerator.GenerateSimpleBeam(10);

            model.Elements[0].Loads.Add(new UniformLoad1D(2, Direction.Z, CoordinationSystem.Global));


            model.Elements[3].As<FrameElement2Node>().HingedAtStart = true;
            model.Elements[3].As<FrameElement2Node>().HingedAtEnd = true;
            

            model.Nodes.First().Constraints = Constraint.Fixed;

            model.Nodes.Last().Constraints = Constraint.Fixed;

            //model.Nodes.Last().Settlements = new Displacement(0, 1, 0, 0, 0, 0);
            
            model.Solve(new LoadCase());

            var d1 = model.Nodes[0].GetSupportReaction();
            var d2 = model.Nodes.Last().GetSupportReaction();

            //CheckingUtil.IsInStaticEquilibrium(model.LastResult, new LoadCase());

            CheckingUtil.IsInStaticEquilibrium(model.LastResult, new LoadCase());

            Console.ReadKey();
        }

        private static void TestTruss()
        {
            var n1 = new Node() { Location = new Point(0, 0, 0) };
            var n2 = new Node() {Location = new Point(1, 0, 0)};

            var trussElm = new TrussElement(n1, n2);
            var framElm = new FrameElement2Node(n1, n2){HingedAtStart = true, HingedAtEnd = true};

            framElm.A = framElm.Az = framElm.Ay = trussElm.A = 1;
            framElm.E = trussElm.E = 210e9;
            var no = 0.3;
            framElm.G = 0*framElm.E / (2 * (1 + no));

            framElm.Iy = framElm.Iz = 0.1;
            framElm.J = 0.2;

            var s2 = trussElm.GetGlobalStifnessMatrix();
            var s1 = framElm.GetGlobalStifnessMatrix();

            var t = s1 - s2;

            for (int i = 0; i < 144; i++) if (Math.Abs(t.CoreArray[i]) < 1e-4) t.CoreArray[i] = 0;

            

           
        }

        private static void tmp()
        {
            Point p1 = new Point(2, 3, 4);//p1={2, 3, 4}
            Point p2 = new Point(5, 6, 9);//p1={5, 6, 9}

            Vector v1 = p2 - p1;// v1 will be {5-2, 6-3, 9-4} = {3, 3, 5}
            double distance = v1.Length;// distance will be (v1.X^2, v1.Y^2, v1.Z^2)
        }


        private static Random rnd = new Random(1);

        private static Constraint getRandomConstraint()
        {
            var buf = new Constraint(
                (DofConstraint)rnd.Next(0, 2),
                (DofConstraint)rnd.Next(0, 2),
                (DofConstraint)rnd.Next(0, 2),
                (DofConstraint)rnd.Next(0, 2),
                (DofConstraint)rnd.Next(0, 2),
                (DofConstraint)rnd.Next(0, 2));

            return buf;
        }

        private static Displacement getRandomDeiplacement()
        {
            var sc = 0.010;
            var add = 10.0*0;

            var buf = new Displacement(
                rnd.NextDouble() * sc + add,
                rnd.NextDouble() * sc + add,
                rnd.NextDouble() * sc + add,
                rnd.NextDouble() * sc + add,
                rnd.NextDouble() * sc + add,
                rnd.NextDouble() * sc + add);

            return buf;
        }

        private static Force getRandomForce()
        {
            var sc = 000;
            var add = 1.0;

            var buf = new Force(
                rnd.NextDouble()*sc + add,
                rnd.NextDouble()*sc + add,
                rnd.NextDouble()*sc + add,
                rnd.NextDouble()*sc + add,
                rnd.NextDouble()*sc + add,
                rnd.NextDouble()*sc + add);

            return buf;
        }
    }
}
