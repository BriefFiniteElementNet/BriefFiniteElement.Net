using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
            TestSerialization();


            Console.ReadKey();
        }

        private static void Test3()
        {
            var cases = new LoadCase[] { new LoadCase(), new LoadCase("dead1", LoadType.Dead) };
            var cmb = new LoadCombination();


            var model = StructureGenerator.Generate3DGrid(5, 5, 5);
            StructureGenerator.AddRandomiseLoading(model, cases);
            StructureGenerator.SetRandomiseConstraints(model);

           
            cmb[cases[0]] = 1.0;
            cmb[cases[1]] = 2.0;

            model.Solve();
            var r= model.Nodes[0].GetSupportReaction(cmb);
        }

        private static void TestSerialization()
        {
            var model = StructureGenerator.Generate3DGrid(5, 5, 5);

            var cases = new LoadCase[] {new LoadCase(), new LoadCase("dead1", LoadType.Dead)};
            var cmb = new LoadCombination();
            cmb[cases[0]] = 1.0;
            cmb[cases[1]] = 2.0;

            StructureGenerator.AddRandomiseLoading(model,cases );
            StructureGenerator.SetRandomiseConstraints(model);


            IFormatter formatter = new BinaryFormatter();
            var str = new MemoryStream();
            
            formatter.Serialize(str, model);

            str.Position = 0;
            //otImplementedException

            var recreated = formatter.Deserialize(str) as Model;

            recreated.Solve(cases);
            model.Solve(cases);


            var d1 = recreated.LastResult.Displacements[cases[1]];
            var d2 = model.LastResult.Displacements[cases[1]];


            var buf = new double[d1.Length];

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = d1[i] - d2[i];
            }
        }

        private static void TestConcentratedLoad()
        {
            var model = StructureGenerator.Generate3DGrid(10,10,10);




            foreach (var elm in model.Elements)
            {
                var pos = 0.5;

                var force = new Force(new Vector(1, 2, 3), new Vector(4, 5, 6));

                var load = new ConcentratedLoad1D(
                    force,
                    pos,
                    CoordinationSystem.Global);

                elm.Loads.Add(load);
            }

            

            


            model.Solve(LoadCase.DefaultLoadCase);

            var reac = model.Nodes[0].GetSupportReaction(LoadCombination.DefaultLoadCombination);


            //var frc = model.Elements[0].As<FrameElement2Node>().GetInternalForceAt(0.49);


            CheckingUtil.IsInStaticEquilibrium(model.LastResult, new LoadCase());
        }

        private static void TestTruss()
        {
            var n1 = new Node() { Location = new Point(0, 0, 0) };
            var n2 = new Node() {Location = new Point(1, 0, 0)};

            var trussElm = new TrussElement2Node(n1, n2);
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
