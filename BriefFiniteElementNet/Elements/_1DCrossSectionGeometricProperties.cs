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
            get { return _iy+_iz; }
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
