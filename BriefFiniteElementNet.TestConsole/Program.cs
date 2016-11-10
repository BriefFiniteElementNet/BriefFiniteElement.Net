using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BriefFiniteElementNet.Elements;
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
            var iy = 0.01;
            var iz = 0.02;
            var a = 0.03;
            var j = 0.05;

            var e = 100;
            var g = 50;

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0));
            model.Nodes.Add(new Node(1, 0, 0));

            var elm = new BarElement(model.Nodes[0], model.Nodes[1]);
            var elm2 = new FrameElement2Node(model.Nodes[0], model.Nodes[1]) {Iy = iy, Iz = iz, A = a, J = j, E = e, G = g};

            elm.Behavior = BarElementBehaviours.FullFrame;

            elm.Material = new UniformBarElementMaterial(e, g);
            elm.Section = new ParametricBarElementCrossSection() {Iy = iy, Iz = iz, A = a,J=j};

            var e1 = elm2.GetLocalStiffnessMatrix();

            var e2 = elm.GetLocalStifnessMatrix();

            var d = (e2 - e1).CoreArray.Max(i => Math.Abs(i));

            model.Nodes[0].Constraints = Constraint.Fixed;

            model.Solve();
        }
    }
}
