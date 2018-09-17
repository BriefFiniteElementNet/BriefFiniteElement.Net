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

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            var buf = new _1DCrossSectionGeometricProperties();

            {
                var lastPoint = this._geometry[this._geometry.Length - 1];

                if (lastPoint != _geometry[0])
                    throw new InvalidOperationException("First point and last point ot PolygonYz should put on each other");



                double a = 0.0, iz = 0.0, iy = 0.0, ixy = 0.0;


                var x = new double[this._geometry.Length];
                var y = new double[this._geometry.Length];

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
            return new int[] { 0, 0, 0 };
        }

    }
}
