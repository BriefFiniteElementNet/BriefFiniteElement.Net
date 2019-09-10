using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;


namespace BriefFiniteElementNet.Sections
{
    public class NonUniformParametric1DSection: Base1DSection
    {
        _1DCrossSectionGeometricProperties sectionAtStart;
        _1DCrossSectionGeometricProperties sectionAtEnd;

        /// <summary>
        /// 
        /// </summary>
        public NonUniformParametric1DSection()
        {
        }

        public NonUniformParametric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            sectionAtStart = (_1DCrossSectionGeometricProperties)info.GetValue("sectionAtStart", typeof(_1DCrossSectionGeometricProperties));
            sectionAtEnd = (_1DCrossSectionGeometricProperties)info.GetValue("sectionAtEnd", typeof(_1DCrossSectionGeometricProperties));
        }

        /// <summary>
        /// Gets or sets the section geometry at start of element
        /// </summary>
        public _1DCrossSectionGeometricProperties SectionAtStart
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
        public _1DCrossSectionGeometricProperties SectionAtEnd
        {
            get { return sectionAtEnd; }
            set
            {
                sectionAtEnd = value;
            }
        }

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            var target = new _1DCrossSectionGeometricProperties();

            var s1 = sectionAtStart;
            var s2 = sectionAtStart;

            {
                var f1 = (1.0 - xi) / 2.0;
                var f2 = (1.0 + xi) / 2.0;

                var pt = new PointYZ();

                target.A = s1.A * f1 + s2.A * f2;
                target.Ay = s1.Ay * f1 + s2.Ay * f2;
                target.Az = s1.Az * f1 + s2.Az * f2;
                target.J = s1.J * f1 + s2.J * f2;
                target.Iy = s1.Iy * f1 + s2.Iy * f2;
                target.Iz = s1.Iz * f1 + s2.Iz * f2;
            }

            return target;
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
