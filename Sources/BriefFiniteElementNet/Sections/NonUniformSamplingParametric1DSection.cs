using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;

namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a non uniform parametric 1d section, which consist of several samplings along the length of element
    /// </summary>
    public class NonUniformSamplingParametric1DSection : Base1DSection
    {

        List<Tuple<IsoPoint, _1DCrossSectionGeometricProperties>> samples = new List<Tuple<IsoPoint, _1DCrossSectionGeometricProperties>>();

        /// <summary>
        /// 
        /// </summary>
        public NonUniformSamplingParametric1DSection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public NonUniformSamplingParametric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            info.TryGetValue<List<Tuple<IsoPoint, _1DCrossSectionGeometricProperties>>>("samples", out samples);
        }

        /// <summary>
        /// Gets or sets the section geometry at start of element
        /// </summary>
        public List<Tuple<IsoPoint, _1DCrossSectionGeometricProperties>> Samples
        {
            get { return samples; }
            set
            {
                samples = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xi"></param>
        /// <returns></returns>
        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            return GetCrossSectionPropertiesAt(xi, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="targetElement"></param>
        /// <returns></returns>
        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi, Element targetElement)
        {
            var sampleCount = samples.Count;

            var a = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.A)).ToArray();
            var ay = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Ay)).ToArray();
            var az = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Az)).ToArray();

            var iy = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Iy)).ToArray();
            var iz = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Iz)).ToArray();
            var iyz = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Iyz)).ToArray();
            var j = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.J)).ToArray();

            var jx = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Jx)).ToArray();
            var ky = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Ky)).ToArray();
            var kz = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Kz)).ToArray();
            var qy = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Qy)).ToArray();
            var qz = samples.Select(i => Tuple.Create(i.Item1.Xi, i.Item2.Qz)).ToArray();


            var buf = new _1DCrossSectionGeometricProperties();

            buf.A = CalcUtil.PolynomialInterpolate(a, xi);
            buf.Ay = CalcUtil.PolynomialInterpolate(ay, xi);
            buf.Az = CalcUtil.PolynomialInterpolate(az, xi);
            buf.Iyz = CalcUtil.PolynomialInterpolate(iyz, xi);
            buf.J = CalcUtil.PolynomialInterpolate(j, xi);

            buf.Iy = CalcUtil.PolynomialInterpolate(iy, xi);
            buf.Iz = CalcUtil.PolynomialInterpolate(iz, xi);
            buf.Ky = CalcUtil.PolynomialInterpolate(ky, xi);
            buf.Kz = CalcUtil.PolynomialInterpolate(kz, xi);

            return buf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int[] GetMaxFunctionOrder()
        {
            var n = samples.Count;

            return new int[] { n - 1, 0, 0 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("samples", samples);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
