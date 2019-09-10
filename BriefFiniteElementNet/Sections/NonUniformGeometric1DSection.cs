using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;

namespace BriefFiniteElementNet.Sections
{
    public class NonUniformGeometric1DSection : Base1DSection
    {
        PointYZ[] sectionAtStart;
        PointYZ[] sectionAtEnd;

        /// <summary>
        /// 
        /// </summary>
        public NonUniformGeometric1DSection()
        {
        }

        public NonUniformGeometric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            sectionAtStart = (PointYZ[])info.GetValue("sectionAtStart", typeof(PointYZ[]));
            sectionAtEnd = (PointYZ[])info.GetValue("sectionAtEnd", typeof(PointYZ[]));
        }

        /// <summary>
        /// Gets or sets the section geometry at start of element
        /// </summary>
        public PointYZ[] SectionAtStart
        {
            get { return sectionAtStart; }
            set
            {
                sectionAtStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the section geometry at end of element
        /// </summary>
        public PointYZ[] SectionAtEnd
        {
            get { return sectionAtEnd; }
            set
            {
                sectionAtEnd = value;
            }
        }



        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            if (sectionAtStart.Length != sectionAtEnd.Length)
                throw new InvalidOperationException();

            var n = sectionAtStart.Length;

            var target = new PointYZ[n];

            var s1 = (1.0 - xi) / 2.0;
            var s2 = (1.0 + xi) / 2.0;


            for (var i = 0; i < n; i++)
            {
                var pt = new PointYZ();

                pt.Y = sectionAtStart[i].Y * s1 + sectionAtEnd[i].Y * s2;
                pt.Z = sectionAtStart[i].Z * s1 + sectionAtEnd[i].Z * s2;

                target[i] = pt;
            }


            var _geometry = target;
            var buf = new _1DCrossSectionGeometricProperties();

            {
                var lastPoint = _geometry[_geometry.Length - 1];

                if (lastPoint != _geometry[0])
                    throw new InvalidOperationException("First point and last point ot PolygonYz should put on each other");

                double a = 0.0, iz = 0.0, iy = 0.0, ixy = 0.0;

                var x = new double[_geometry.Length];
                var y = new double[_geometry.Length];

                for (int i = 0; i < _geometry.Length; i++)
                {
                    x[i] = _geometry[i].Y;
                    y[i] = _geometry[i].Z;
                }

                var l = _geometry.Length - 1;

                var ai = 0.0;

                for (var i = 0; i < l; i++)
                {
                    ai = x[i] * y[i + 1] - x[i + 1] * y[i];
                    a += ai;
                    iy += (y[i] * y[i] + y[i] * y[i + 1] + y[i + 1] * y[i + 1]) * ai;
                    iz += (x[i] * x[i] + x[i] * x[i + 1] + x[i + 1] * x[i + 1]) * ai;

                    ixy += (x[i] * y[i + 1] + 2 * x[i] * y[i] + 2 * x[i + 1] * y[i + 1] + x[i + 1] * y[i]) * ai;

                }

                a = a * 0.5;
                iz = iz * 1 / 12.0;
                iy = iy * 1 / 12.0;
                ixy = ixy * 1 / 24.0;
                var j = iy + iz;
                //not sure which one is correct j = ix + iy or j = ixy >:)~ 

                buf.A = Math.Abs(a);
                buf.Iz = Math.Abs(iz);
                buf.Iy = Math.Abs(iy);
                buf.J = Math.Abs(j);
                buf.Ay = Math.Abs(a);//TODO: Ay is not equal to A, this is temporary fix
                buf.Az = Math.Abs(a);//TODO: Az is not equal to A, this is temporary fix

            }

            return buf;
        }


        public override int[] GetMaxFunctionOrder()
        {
            return new int[] { 2, 0, 0 };
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("sectionAtStart", sectionAtStart);
            info.AddValue("sectionAtEnd", sectionAtEnd);

            base.GetObjectData(info, context);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
