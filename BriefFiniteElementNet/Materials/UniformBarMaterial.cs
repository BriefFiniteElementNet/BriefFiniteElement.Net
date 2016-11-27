using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Materials;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// 
    /// </summary>
    public class UniformBarMaterial:BaseBarMaterial
    {
        private double e, g, rho, mu;

        /// <summary>
        /// Gets or sets the elastic module of section.
        /// </summary>
        /// <value>
        /// The elastic modulus of member
        /// </value>
        public double E
        {
            get { return e; }
            set { e = value; }
        }

        /// <summary>
        /// Gets or sets the shear module of section.
        /// </summary>
        /// <value>
        /// The shear modulus of member
        /// </value>
        public double G
        {
            get { return g; }
            set { g = value; }
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

        public override BarCrossSectionMechanicalProperties GetMaterialPropertiesAt(double x)
        {
            var buf = new BarCrossSectionMechanicalProperties();

            buf.E = this.e;
            buf.G = this.g;
            buf.Density = this.rho;
            buf.Mu = this.mu;

            return buf;
        }

        public override int GetMaxFunctionOrder()
        {
            return 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarMaterial"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        public UniformBarMaterial(double e)
        {
            this.e = e;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarMaterial"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="g">The g.</param>
        public UniformBarMaterial(double e, double g)
        {
            this.e = e;
            this.g = g;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarMaterial"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="g">The g.</param>
        /// <param name="rho">The rho.</param>
        public UniformBarMaterial(double e, double g, double rho)
        {
            this.e = e;
            this.g = g;
            this.rho = rho;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarMaterial"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="g">The g.</param>
        /// <param name="rho">The rho.</param>
        /// <param name="mu">The mu.</param>
        public UniformBarMaterial(double e, double g, double rho, double mu)
        {
            this.e = e;
            this.g = g;
            this.rho = rho;
            this.mu = mu;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarMaterial"/> class.
        /// </summary>
        public UniformBarMaterial()
        {
        }


    }
}
