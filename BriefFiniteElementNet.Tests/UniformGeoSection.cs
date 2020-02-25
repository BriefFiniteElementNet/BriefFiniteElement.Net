using System;
using BriefFiniteElementNet.Sections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BriefFiniteElementNet.Tests
{
    [TestClass]
    public class UniformGeoSection
    {
        [TestMethod]
        public void test_calculate_properties()
        {
            var sec = new UniformGeometric1DSection();

            sec.Geometry = new PointYZ[] { new PointYZ(-1, -2), new PointYZ(-1, 2), new PointYZ(1, 2), new PointYZ(1, -2), new PointYZ(-1, -2) };

            var t = sec.GetCrossSectionPropertiesAt(0);

            var b = 2.0;
            var h = 4.0;


            var iy = b * h * h * h / 12.0;
            var iz = h * b * b * b / 12.0;
            var qy = 0;
            var qz = 0;
            var a = b * h;

            var epsilon = 1e-6;

            Assert.IsTrue(Math.Abs(t.Iy - iy) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Iz - iz) < epsilon, "wrong value");

            Assert.IsTrue(Math.Abs(t.Qy - qy) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Qz - qz) < epsilon, "wrong value");

            Assert.IsTrue(Math.Abs(t.A - a) < epsilon, "wrong value");

        }

        [TestMethod]
        public void test_reset_centroid()
        {
            var sec = new UniformGeometric1DSection();

            sec.Geometry = new PointYZ[] { new PointYZ(-1, -2), new PointYZ(-1.23, 2.54), new PointYZ(1.75, 2.36), new PointYZ(1.2, -2.1), new PointYZ(-1, -2) };

            
            var dy = 2.849;//random values
            var dz = 1.118;

            var sec2 = new UniformGeometric1DSection((PointYZ[])sec.Geometry.Clone());
            //move section by random values dy dz
            //is ResetCentroid=true then values are regard centroid and should not change

            for (var i=0;i<sec2.Geometry.Length;i++)
            {
                sec2.Geometry[i].Y += dy;
                sec2.Geometry[i].Z += dz;
            }


            sec2.ResetCentroid = true;
            sec.ResetCentroid = true;

            var t = sec.GetCrossSectionPropertiesAt(0);
            var t2 = sec2.GetCrossSectionPropertiesAt(0);

            var epsilon = 1e-6;

            Assert.IsTrue(Math.Abs(t.Iy - t2.Iy) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Iz - t2.Iz) < epsilon, "wrong value");

            Assert.IsTrue(Math.Abs(t.Qy - t2.Qy) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Qz - t2.Qz) < epsilon, "wrong value");

            Assert.IsTrue(Math.Abs(t.A - t2.A) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Ay - t2.Ay) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Az - t2.Az) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.J - t2.J) < epsilon, "wrong value");
            Assert.IsTrue(Math.Abs(t.Iyz - t2.Iyz) < epsilon, "wrong value");

        }
    }
}
