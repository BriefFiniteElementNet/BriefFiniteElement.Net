using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents Orthotropic Material.
    /// </summary>
    /// <remarks>
    /// For Isotropic materials Ex=Ey=Ez=E (Young's ) and nu_xy=nu_yz=nu_zx=nu (Poisson's ratio)
    /// More info:
    /// http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm
    /// </remarks>
    public struct OrthotropicMaterial
    {
        public double ex;
        public double ey;
        public double ez;

        public double nu_xy;
        public double nu_yx;

        public double nu_yz;
        public double nu_zy;

        public double nu_zx;
        public double nu_xz;

        /// <summary>
        /// Gets or sets the Young's modulus in X direction.
        /// </summary>
        /// <value>
        /// The ex.
        /// </value>
        public double Ex
        {
            get { return ex; }
            set { ex = value; }
        }

        /// <summary>
        /// Gets or sets the Young's modulus in Y direction.
        /// </summary>
        /// <value>
        /// The ey.
        /// </value>
        public double Ey
        {
            get { return ey; }
            set { ey = value; }
        }

        /// <summary>
        /// Gets or sets the Young's modulus in Z direction.
        /// </summary>
        /// <value>
        /// The ez.
        /// </value>
        public double Ez
        {
            get { return ez; }
            set { ez = value; }
        }


        public double NuXy
        {
            get { return nu_xy; }
            set { nu_xy = value; }
        }

        public double NuYz
        {
            get { return nu_yz; }
            set { nu_yz = value; }
        }

        public double NuZx
        {
            get { return nu_zx; }
            set { nu_zx = value; }
        }

        public double NuYx
        {
            get { return nu_yx; }
            set { nu_yx = value; }
        }

        public double NuZy
        {
            get { return nu_zy; }
            set { nu_zy = value; }
        }

        public double NuXz
        {
            get { return nu_xz; }
            set { nu_xz = value; }
        }
    }
}
