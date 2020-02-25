using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a uniform section for <see cref="BarElement"/> which defines section's geometry as a polygon
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class UniformGeometric1DSection : Base1DSection
    {
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_geometry", _geometry);

            base.GetObjectData(info, context);
        }

        protected UniformGeometric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _geometry = (PointYZ[])info.GetValue("_geometry", typeof(PointYZ[]));
        }

        public UniformGeometric1DSection()
        {
        }

        public UniformGeometric1DSection(PointYZ[] geometry)
        {
            _geometry = geometry;
        }

        PointYZ[] _geometry;

        /// <summary>
        /// Gets or sets the Geometry.
        /// </summary>
        /// <value>
        /// The _geometry of section.
        /// </value>
        public PointYZ[] Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
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

        private bool _resetCentroid = true;

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            return _1DCrossSectionGeometricProperties.Calculate(this.Geometry,this._resetCentroid);
        }


        public override int[] GetMaxFunctionOrder()
        {
            return new int[] { 0, 0, 0 };
        }

    }
}
