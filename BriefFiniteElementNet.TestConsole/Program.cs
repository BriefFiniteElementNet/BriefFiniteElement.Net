using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Resolvers;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Validation;

namespace BriefFiniteElementNet.TestConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            TestTriangle();

            //SparseMatrixMultiplyValidation.Test1();

            Console.ReadKey();
        }

        private void Test1()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(5, 5, 5);
            //StructureGenerator.AddRandomiseLoading(model, LoadCase.DefaultLoadCase);

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);


            new Frame3DDValidator(model).Validate();

        }

        private static void Test2()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(5, 5, 5);
            //StructureGenerator.AddRandomiseLoading(model, LoadCase.DefaultLoadCase);

            //var wrapped = SerializationObsolete.ObjectWrapper.Wrap(model);

            var data = DataContractSerializerHelper.SerializeXml(model);

        }

        private static void TestBar()
        {
            var iy = 0.03;
            var iz = 0.02;
            var a = 0.01;
            var j = 0.05;

            var e = 7;
            var g = 11;
            var rho = 13;

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0));
            model.Nodes.Add(new Node(1, 2, 3));

            var barElement = new BarElement(model.Nodes[0], model.Nodes[1]);

            barElement.Behavior = BarElementBehaviours.FullFrame;

            var frameElement = new FrameElement2Node(model.Nodes[0], model.Nodes[1])
            {
                Iy = iy,
                Iz = iz,
                A = a,
                J = j,
                E = e,
                G = g,
                MassDensity = rho
            };


            barElement.Material = new UniformBarMaterial(e, g, rho);
            barElement.Section = new UniformParametricBarElementCrossSection() {Iy = iy, Iz = iz, A = a,J=j};

            frameElement.MassFormulationType = MassFormulation.Consistent;

            var frameM = frameElement.GetLocalMassMatrix();
            MathUtil.FillLowerTriangleFromUpperTriangle(frameM);

            var barM = barElement.GetLocalMassMatrix();

            var t = 1;//- 1e-10;

            var d = (frameM - t* barM);//
            var dMax = d.CoreArray.Max(i => Math.Abs(i));

            model.Nodes[0].Constraints = Constraint.Fixed;

            model.Solve();
        }

        private static void TestTriangle()
        {
            var t = 0.01;
            var e = 210e9;
            var nu = 0.2;

            var n1 = new Node(new Point(0, 0, 0));
            var n2 = new Node(new Point(3, 5, 7));
            var n3 = new Node(new Point(1, -5, 4));

            var dkt = new TriangleFlatShell()
            {
                Behavior = FlatShellBehaviour.ThinPlate,
                PoissonRatio = nu,
                ElasticModulus = e,
                Thickness=t
            };


            dkt.Nodes[0] = n1;
            dkt.Nodes[1] = n2;
            dkt.Nodes[2] = n3;

            var tri = new TriangleElement();
            tri.Behavior = FlatShellBehaviours.FullThinShell;
            tri.Section = new UniformTriangleThickness() { T = t };
            tri.Material = new UniformTriangleMaterial() {E = e, Nu = nu};

            tri.Nodes[0] = n1;
            tri.Nodes[1] = n2;
            tri.Nodes[2] = n3;


            var kTri = tri.GetLocalStifnessMatrix();
            var kDkt = dkt.GetLocalPlateBendingStiffnessMatrix();

            var d = kTri - kDkt;

            var xi = 0.162598494;
            var eta = 0.284984989;

            var b1 = new DktHelper().GetBMatrixAt(tri, tri.GetTransformationMatrix(), xi, eta);
            var lpts = dkt.GetLocalPoints();

            var b2 = DktElement.GetBMatrix(xi, eta,
                new[] {lpts[0].X, lpts[1].X, lpts[2].X},
                new[] {lpts[0].Y, lpts[1].Y, lpts[2].Y});
                // new DktHelper().GetBMatrixAt(tri, tri.GetTransformationMatrix(), xi, eta);

            var db = b1 - b2;
        }
    }
}
