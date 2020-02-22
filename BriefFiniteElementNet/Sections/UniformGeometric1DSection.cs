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

                double a = 0, iy = 0, iz = 0, ixy = 0, sy = 0, sz = 0, yc = 0, zc = 0;

                var x = new double[this._geometry.Length];
                var y = new double[this._geometry.Length];

                for (int i = 0; i < _geometry.Length; i++)
                {
                    x[i] = _geometry[i].Y;
                    y[i] = _geometry[i].Z;
                }

                var l = _geometry.Length - 1;
                for (var i = 0; i < l; i++)
                {
                    a += (x[i] - x[i + 1]) * (y[i] + y[i + 1]) / 2.0;
                    iy += (y[i] * y[i] + y[i + 1] * y[i + 1]) * (x[i] - x[i + 1]) * (y[i] + y[i + 1]) / 12.0;
                    iz += (x[i] * x[i] + x[i + 1] * x[i + 1]) * (y[i + 1] - y[i]) * (x[i] + x[i + 1]) / 12.0;
                    ixy += (iy + iz);
                    sy += (x[i] - x[i + 1]) * (y[i] * y[i] + y[i] * y[i + 1] + y[i + 1] * y[i + 1]) / 6;
                    sz += -(y[i] - y[i + 1]) * (x[i] * x[i] + x[i] * x[i + 1] + x[i + 1] * x[i + 1]) / 6;
                }
                yc = sz / a;
                zc = sy / a;
                //if the base axis is not the centroid axis, all the point need move
                if (Math.Abs(yc) > 0.00001 || Math.Abs(zc) > 0.00001)
                {
                    for (int i = 0; i < _geometry.Length; i++)
                    {
                        x[i] = _geometry[i].Y - yc;
                        y[i] = _geometry[i].Z - zc;
                    }
                    iy = 0;
                    iz = 0;
                    ixy = 0;
                    for (var i = 0; i < l; i++)
                    {
                        iy += (y[i] * y[i] + y[i + 1] * y[i + 1]) * (x[i] - x[i + 1]) * (y[i] + y[i + 1]) / 12.0;
                        iz += (x[i] * x[i] + x[i + 1] * x[i + 1]) * (y[i + 1] - y[i]) * (x[i] + x[i + 1]) / 12.0;
                        ixy += (iy + iz);
                    }
                }
                
                buf.A = Math.Abs(a);
                buf.Iz = Math.Abs(iz);
                buf.Iy = Math.Abs(iy);
                buf.J = Math.Abs(ixy);
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
