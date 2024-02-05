using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;

namespace BriefFiniteElementNet.Sections
{
    public class NonUniformSamplingParametric1DSection : Base1DSection
    {

        List<Tuple<double, _1DCrossSectionGeometricProperties>> samples = new List<Tuple<double, _1DCrossSectionGeometricProperties>>();

        /// <summary>
        /// 
        /// </summary>
        public NonUniformSamplingParametric1DSection()
        {
        }

        public NonUniformSamplingParametric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            samples = (List<Tuple<double, _1DCrossSectionGeometricProperties>>)info.GetValue("samples", typeof(List<Tuple<double, _1DCrossSectionGeometricProperties>>));
        }

        /// <summary>
        /// Gets or sets the section geometry at start of element
        /// </summary>
        public List<Tuple<double, _1DCrossSectionGeometricProperties>> Samples
        {
            get { return samples; }
            set
            {
                samples = value;
            }
        }

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            return GetCrossSectionPropertiesAt(xi, null);
        }

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi, Element targetElement)
        {
            var target = new _1DCrossSectionGeometricProperties();

            throw new NotImplementedException();
            /*
            var sampleCount = samples.Count;


            var a = samples.Select(i => Tuple.Create(i.Item1, i.Item2.A));
            var ay = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Ay));
            var az = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Az));

            var iy = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Iy));
            var iz = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Iz));
            var iyz = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Iyz));
            var j = samples.Select(i => Tuple.Create(i.Item1, i.Item2.J));

            var jx = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Jx));
            var ky = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Ky));
            var kz = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Kz));
            var qy = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Qy));
            var qz = samples.Select(i => Tuple.Create(i.Item1, i.Item2.Qz));
            


            var s1 = sectionAtStart;
            var s2 = sectionAtStart;

            {
                var f1 = (1.0 - xi) / 2.0;
                var f2 = (1.0 + xi) / 2.0;

                var pt = new PointYZ();

                target.A = s1.A * f1 + s2.A * f2;
                target.Ay = s1.Ay * f1 + s2.Ay * f2;
                target.Az = s1.Az * f1 + s2.Az * f2;
                target.Iyz = s1.Iyz * f1 + s2.Iyz * f2;
                target.Iy = s1.Iy * f1 + s2.Iy * f2;
                target.Iz = s1.Iz * f1 + s2.Iz * f2;
                target.Ky = s1.Ky * f1 + s2.Ky * f2;
                target.Kz = s1.Kz * f1 + s2.Kz * f2;

            }

            return target;
            */
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new int[] { 2, 0, 0 };
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("samples", samples);

            base.GetObjectData(info, context);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
