using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public class UniformBarElementMaterial:BaseBarElementMaterial
    {
        private double e, g;

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

        public override BarCrossSectionMechanicalProperties GetMaterialPropertiesAt(double x)
        {
            var buf = new BarCrossSectionMechanicalProperties();

            buf.E = this.e;
            buf.G = this.g;

            return buf;
        }

        public override int GetGaussianIntegrationPoints()
        {
            return 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarElementMaterial"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="g">The g.</param>
        public UniformBarElementMaterial(double e, double g)
        {
            this.e = e;
            this.g = g;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBarElementMaterial"/> class.
        /// </summary>
        public UniformBarElementMaterial()
        {
        }
    }
}
