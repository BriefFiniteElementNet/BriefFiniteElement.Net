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

        /// <summary>
        /// If sets to true, all geometric properties are calculated based on centroid, not coordination system origins.
        /// for more info see pull request #34
        /// </summary>
        /// <remarks>
        /// If set to true, then all properties are calculated based on centroid, not coordination system origins. In this case <see cref="Qy"/> and <see cref="Qz"/> are zeros,
        /// <see cref="Iy"/> and <see cref="Iz"/> and <see cref="Iyz"/> are calculated based on the section centroid. <see cref="A"/> do not change.
        /// </remarks>
        public bool ResetCentroid
        {
            get { return _resetCentroid; }
            set
            {
                _resetCentroid = value;
            }
        }

        private bool _resetCentroid;

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            return GetCrossSectionPropertiesAt(xi, null);
        }

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi, Element targetElement)
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

            return _1DCrossSectionGeometricProperties.Calculate(target, this._resetCentroid);
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
