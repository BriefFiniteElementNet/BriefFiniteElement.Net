using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a class that represents cross section geometrical properties at specific location of length of 1D elements.
    /// </summary>
    public class _1DCrossSectionGeometricProperties
    {
        /// <summary>
        /// Calculates the cross section geometrical properties for given polygon
        /// </summary>
        /// <param name="points">the corners of polygon</param>
        /// <param name="resetCentroid">if set to true, properties are calculated based on section centroid, if false then properties are calculated based on coordination system origins</param>
        /// <returns>Geometrical properties of given polygon</returns>
        public static _1DCrossSectionGeometricProperties Calculate(PointYZ[] points,bool resetCentroid)
        {
            var _geometry = points;


            var buf = new _1DCrossSectionGeometricProperties();

            {
                var lastPoint = _geometry[_geometry.Length - 1];

                if (lastPoint != _geometry[0])
                    throw new InvalidOperationException("First point and last point on PolygonYz should put on each other");



                double a = 0.0, iy = 0.0, ix = 0.0, ixy = 0.0;
                double qy = 0.0, qx = 0.0;


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
                    //formulation: https://apps.dtic.mil/dtic/tr/fulltext/u2/a183444.pdf
                    //also https://en.wikipedia.org/wiki/Second_moment_of_area#Any_polygon

                    ai = x[i] * y[i + 1] - x[i + 1] * y[i];

                    a += ai;

                    iy += (x[i] * x[i] + x[i] * x[i + 1] + x[i + 1] * x[i + 1]) * ai;
                    ix += (y[i] * y[i] + y[i] * y[i + 1] + y[i + 1] * y[i + 1]) * ai;

                    qy += (x[i] + x[i + 1]) * ai;
                    qx += (y[i] + y[i + 1]) * ai;

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

                if (resetCentroid)
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

        private double _a;
        private double _ay;
        private double _az;
        private double _iy;
        private double _iz;
        private double _qy;
        private double _qz;
        private double _iyz;

        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>
        /// The area of section in m^2
        /// </value>
        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        /// <summary>
        /// Gets or sets the ay.
        /// </summary>
        /// <value>
        /// shear area of element, in local y direction, only used when shear deformation should be considered
        /// </value>
        public double Ay
        {
            get { return _ay; }
            set { _ay = value; }
        }

        /// <summary>
        /// Gets or sets the az.
        /// </summary>
        /// <value>
        /// shear area of element, in local z direction, only used when shear deformation should be considered
        /// </value>
        public double Az
        {
            get { return _az; }
            set { _az = value; }
        }

        /// <summary>
        /// Gets or sets the iy.
        /// </summary>
        /// <value>
        /// The Second Moment of Area of section regard to Z axis.
        /// </value>
        /// <remarks>
        ///     /
        /// Iy= | Z² . dA
        ///    /A
        /// </remarks>
        public double Iy
        {
            get { return _iy; }
            set { _iy = value; }
        }

        /// <summary>
        /// Gets or sets the _iz.
        /// </summary>
        /// <value>
        /// The Second Moment of Area of section regard to Y axis
        /// </value>
        /// <remarks>
        ///     /
        /// Iz= | Y² . dA
        ///    /A
        /// </remarks>
        public double Iz
        {
            get { return _iz; }
            set { _iz = value; }
        }

        /// <summary>
        /// Gets or sets the Qy.
        /// </summary>
        /// <value>
        /// The first Moment of Area of section regard to Z axis.
        /// </value>
        /// <remarks>
        ///     /
        /// Iy= | Z . dA
        ///    /A
        /// </remarks>
        public double Qy
        {
            get { return _qy; }
            set { _qy = value; }
        }

        /// <summary>
        /// Gets or sets the Qz.
        /// </summary>
        /// <value>
        /// The first Moment of Area of section regard to Y axis
        /// </value>
        /// <remarks>
        ///     /
        /// Iz= | Y . dA
        ///    /A
        /// </remarks>
        public double Qz
        {
            get { return _qz; }
            set { _qz = value; }
        }

        /// <summary>
        /// Gets the polar moment of inertia (J).
        /// </summary>
        /// <value>
        /// The polar moment of inertial.
        /// </value>
        /// <remarks>
        ///     /          /
        /// J= | ρ². dA = | (y²+z²).dA = <see cref="Iy"/> + <see cref="Iz"/> 
        ///    /A         /A
        /// </remarks>
        public double J
        {
            get { return _iy + _iz; }
        }

        /// <summary>
        /// Gets or sets the Iyz.
        /// </summary>
        /// <value>
        /// The Product Moment of Area of section
        /// </value>
        /// <remarks>
        ///      /
        /// Iyz= | Y . Z . dA
        ///      /A
        /// </remarks>
        public double Iyz
        {
            get { return _iyz; }
            set { _iyz = value; }
        }
    }
}
