using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;

namespace BriefFiniteElementNet.Validation
{
    public class BarElementTester
    {
        public static void TestBarStiffness()
        {
            var iy = 0.02;
            var iz = 0.02;
            var a = 0.01;
            var j = 0.05;

            var e = 210e9;
            var g = 70e9;
            var rho = 13;

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0));
            model.Nodes.Add(new Node(3, 5, 7));

            var barElement = new BarElement(model.Nodes[0], model.Nodes[1]);

            barElement.Behavior = BarElementBehaviours.FullFrame;
            barElement.Material = UniformIsotropicMaterial.CreateFromYoungShear(e, g);

            var frameElement = new FrameElement2Node(model.Nodes[0], model.Nodes[1])
            {
                Iy = iy,
                Iz = iz,
                A = a,
                J = j,
                E = e,
                G = g,
                //MassDensity = rho
            };

            frameElement.ConsiderShearDeformation = false;

            //barElement.Material = new UniformBarMaterial(e, g, rho);
            barElement.Section = new UniformParametric1DSection() { Iy = iy, Iz = iz, A = a, J = j };

            var frK = frameElement.GetGlobalStifnessMatrix();
            var barK = barElement.GetGlobalStifnessMatrix();

            var d = (frK - barK).Max(i => Math.Abs(i));


        }
    }
}
