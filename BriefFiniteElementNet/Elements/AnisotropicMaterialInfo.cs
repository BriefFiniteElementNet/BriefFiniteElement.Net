using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents Anisotropic Material (most general material type).
    /// </summary>
    /// <remarks>
    /// For Isotropic materials Ex=Ey=Ez=E (Young's ) and nu_xy=nu_yz=nu_zx=nu (Poisson's ratio)
    /// More info:
    /// http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm
    /// </remarks>
    public struct AnisotropicMaterialInfo
    {
        private double ex;
        private double ey;
        private double ez;

        private double nu_xy;
        private double nu_yx;

        private double nu_yz;
        private double nu_zy;

        private double nu_zx;
        private double nu_xz;

        private double rho;
        private double mu;

        //private double e, g, rho, mu;

        /// <summary>
        /// Determines whether this info has valid mechanizal properties.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has valid mechanizal properties]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidMechanizalProperties()
        {
            //based on http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm

            var tol = new double[] {ex, ey, ez}.Max() * 1e-10;

            var flags = new bool[]
            {
                MathUtil.Equals(nu_yz / ey, nu_zy / ez, tol),
                MathUtil.Equals(nu_zx / ez, nu_xz / ex, tol),
                MathUtil.Equals(nu_yx / ey, nu_xy / ex, tol),
            };

            return flags.All(i => i);
        }

        

        /// <summary>
        /// Gets or sets the mass density of material.
        /// </summary>
        /// <value>
        /// The rho.
        /// </value>
        public double Rho
        {
            get { return rho; }
            set { rho = value; }
        }

        /// <summary>
        /// Gets or sets the damp density of material.
        /// </summary>
        /// <value>
        /// The mu.
        /// </value>
        public double Mu
        {
            get { return mu; }
            set { mu = value; }
        }


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
