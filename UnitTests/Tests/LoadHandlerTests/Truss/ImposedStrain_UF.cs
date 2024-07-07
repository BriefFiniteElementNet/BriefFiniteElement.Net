using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using NUnit.Framework;

namespace BriefFiniteElementNet.Tests.LoadHandlerTests.Truss
{
    public class ImposedStrain_UF
    {
        [Test]
        public void EqNodalForce()
        {
            var imposedStrainMagnitude = 2e-6;
            var L = 4;//[m]
            double A = 0.01;
            double E = 210e9;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            elm.Section = new UniformParametric1DSection(A, 0, 0);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            

            var u1 = new Loads.ImposedStrainLoad(imposedStrainMagnitude);

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.ImposedStrain_UF_Handler();

            var currentEnds = handler.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var expectedEnds = new Force[2];

            var f = imposedStrainMagnitude * E * A;

            expectedEnds[0] = new Force(-f, 0, 0, 0, 0, 0);
            expectedEnds[1] = new Force(+f, 0, 0, 0, 0, 0);


            var d1 = currentEnds[0] - expectedEnds[0];
            var d2 = currentEnds[1] - expectedEnds[1];

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d2.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d2.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void InternalForce()
        {
            //internal force of 2 node truss with concentrated load and both ends fixed

            double ft = 2.0;
            double L = 4;//[m]
            double A = 0.01;
            double E = 210e9;
            var G = 1;
            double xt = 2;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(A, 0, 0);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);
            
            var strain = +0.001;//[m]

            var u1 = new Loads.ImposedStrainLoad(strain);

            u1.Case = LoadCase.DefaultLoadCase;

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.ImposedStrain_UF_Handler();

            var f = strain * E * A;

            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var testTensor = (Force)handler.GetLocalLoadInternalForceAt(elm, hlpr, u1, new IsoPoint(xi[0] * (1 - 1e-9)));
                
                var exactFx = -f;

                var testFx = testTensor.Fx;

                var err = testFx - exactFx;

                Assert.IsTrue(Math.Abs(err) < 1e-5, "invalid value");
            }
        }

        [Test]
        public void InternalDisplacement()
        {
            double ft = 2.0;
            double L = 4;//[m]
            double A = 1;
            double E = 1;
            var G = 1;
            double xt = 2;
            var strain = 0.002;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(A, 0, 0);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            var u1 = new Loads.ImposedStrainLoad();
            
            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.ImposedStrain_UF_Handler();

            var epsilon = 1e-6;

            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var currentTensor = handler.GetLocalLoadDisplacementAt(elm, hlpr, u1, new IsoPoint(xi));
                var currentDx = currentTensor.DX;

                var expected = 0.0;

                Assert.IsTrue(Math.Abs(currentDx - expected) < epsilon, "invalid value");
            }
        }
    }
}
