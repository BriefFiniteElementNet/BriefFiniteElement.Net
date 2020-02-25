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
            info.AddValue("_resetCentroid", _resetCentroid);

            base.GetObjectData(info, context);
        }

        protected UniformGeometric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _geometry = (PointYZ[])info.GetValue("_geometry", typeof(PointYZ[]));

            if (info.GetFieldType("_resetCentroid") != null)//older version compatibility
                _resetCentroid = info.GetBoolean("_resetCentroid");
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

        private bool _resetCentroid;



        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            var buf = new _1DCrossSectionGeometricProperties();

            {
                var lastPoint = this._geometry[this._geometry.Length - 1];

                if (lastPoint != _geometry[0])
                    throw new InvalidOperationException("First point and last point on PolygonYz should put on each other");



                double a = 0.0, iy = 0.0, ix = 0.0, ixy = 0.0;
                double qy = 0.0, qx = 0.0;


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
                    //formulation: https://apps.dtic.mil/dtic/tr/fulltext/u2/a183444.pdf

                    ai = x[i] * y[i + 1] - x[i + 1] * y[i];
                    
                    a += ai;
                    
                    ix += (x[i] * x[i] + x[i] * x[i + 1] + x[i + 1] * x[i + 1]) * ai;
                    iy += (y[i] * y[i] + y[i] * y[i + 1] + y[i + 1] * y[i + 1]) * ai;

                    qx += (x[i] + x[i + 1]) * ai;
                    qy += (y[i] + y[i + 1]) * ai;

                    ixy += (x[i] * y[i + 1] + 2 * x[i] * y[i] + 2 * x[i + 1] * y[i + 1] + x[i + 1] * y[i]) * ai;
                }

                a = a * 1 / 2.0;
                qy = qy * 1 / 6.0;
                qx = qx * 1 / 6.0; 
                iy = iy * 1 / 12.0;
                ix = ix * 1 / 12.0;

                ixy = ixy * 1 / 24.0;

                var sign = Math.Sign(a);//sign is negative if points are in clock wise order

                buf.A = sign * a;
                buf.Qy = sign * qx;
                buf.Qz = sign * qy; 
                buf.Iz = sign * iy;
                buf.Iy = sign * ix;
                buf.Iyz = sign * ixy;

                buf.Ay = sign * a;//TODO: Ay is not equal to A, this is temporary fix
                buf.Az = sign * a;//TODO: Az is not equal to A, this is temporary fix

                if (_resetCentroid)
                {
                    //parallel axis theorem
                    buf.Iz -= buf.Qz * buf.Qz / buf.A;//A *D^2 = (A*D)*(A*D)/A
                    buf.Iy -= buf.Qy * buf.Qy / buf.A;

                    buf.Iyz -= buf.Qy * buf.Qz / buf.A;

                    buf.Qy = 0;
                    buf.Qz = 0;
                }
            }

            return buf;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new int[] { 0, 0, 0 };
        }

    }
}
