using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Validation;

namespace BriefFiniteElementNet.TestConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Test3();

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

        private static void Test3()
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
            model.Nodes.Add(new Node(1, 0, 0));

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


            barElement.Material = new UniformBarElementMaterial(e, g, rho);
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
    }
}
