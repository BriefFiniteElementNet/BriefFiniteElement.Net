using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Materials
{
    /// <summary>
    /// Represents a uniform (not varying) isotropic material.
    /// </summary>
    [Serializable]
    public class UniformIsotropicMaterial: BaseMaterial, IEquatable<UniformIsotropicMaterial>
    {

        private double _youngModulus;
        private double _poissonRatio;

        /// <summary>
        /// Gets or sets the Young Modulus of material
        /// </summary>
        public double YoungModulus
        {
            get { return _youngModulus; }
            set { _youngModulus = value; }
        }

        /// <summary>
        /// Gets or sets the Poisson's Ratio of material
        /// </summary>
        public double PoissonRatio
        {
            get { return _poissonRatio; }
            set { _poissonRatio = value; }
        }

        /// <inheritdoc />
        public override AnisotropicMaterialInfo GetMaterialPropertiesAt(params double[] isoCoords)
        {
            var buf = new AnisotropicMaterialInfo();

            buf.Ex = buf.Ey = buf.Ez = _youngModulus;

            buf.NuXy = buf.NuYx =
                buf.NuXz = buf.NuZx =
                    buf.NuZy = buf.NuYz =
                        this._poissonRatio;

            return buf;
        }


        /// <summary>
        /// Creates the material from Young's Modulus and Poisson's Ratio
        /// </summary>
        /// <param name="youngModulus">Young's Modulus of material</param>
        /// <param name="poissonRatio">Poisson's Ratio of material</param>
        /// <returns>created material</returns>
        public static UniformIsotropicMaterial CreateFromYoungPoisson(double youngModulus, double poissonRatio)
        {
            return new UniformIsotropicMaterial() {_youngModulus = youngModulus, _poissonRatio = poissonRatio};
        }

        /// <summary>
        /// Creates the material from Young's Modulus and Shear Modulus
        /// </summary>
        /// <param name="youngModulus">Young's Modulus of material</param>
        /// <param name="shearModulus">Shear's Modulus of material</param>
        /// <returns>created material</returns>
        public static UniformIsotropicMaterial CreateFromYoungShear(double youngModulus, double shearModulus)
        {
            //based on formula from https://en.wikipedia.org/wiki/Young%27s_modulus
            var e = youngModulus;
            var g = shearModulus;

            var poisson = e / (2 * g) - 1;
            return new UniformIsotropicMaterial() { _youngModulus = youngModulus, _poissonRatio = poisson };
        }

        /// <summary>
        /// Creates the material from Shear Modulus and Poisson's Ratio
        /// </summary>
        /// <param name="shearModulus">Shear's Modulus of material</param>
        /// <param name="poissonModulus">Poisson's Ratio of material</param>
        /// <returns>created material</returns>

        public static UniformIsotropicMaterial CreateFromShearPoisson(double shearModulus, double poissonModulus)
        {
            //based on formula from https://en.wikipedia.org/wiki/Young%27s_modulus
            var nu = poissonModulus;
            var g = shearModulus;

            var e = (2 * g) * (1 + nu);

            return new UniformIsotropicMaterial() { _youngModulus = e, _poissonRatio = poissonModulus };
        }

        /// <inheritdoc />
        public override int[] GetMaxFunctionOrder()
        {
            return new[] {0, 0, 0};
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("_youngModulus", _youngModulus);
            info.AddValue("_poissonRatio", _poissonRatio);
        }

        protected UniformIsotropicMaterial(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _youngModulus = info.GetDouble("_youngModulus");
            _poissonRatio = info.GetDouble("_poissonRatio");
        }

        public UniformIsotropicMaterial()
        {
            
        }

        public UniformIsotropicMaterial(double youngModulus , double poissonRatio )
        {
            _youngModulus = youngModulus;
            _poissonRatio = poissonRatio;
        }


        public bool Equals(UniformIsotropicMaterial other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _youngModulus.Equals(other._youngModulus) && _poissonRatio.Equals(other._poissonRatio);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UniformIsotropicMaterial) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ _youngModulus.GetHashCode();
                hashCode = (hashCode*397) ^ _poissonRatio.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(UniformIsotropicMaterial left, UniformIsotropicMaterial right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UniformIsotropicMaterial left, UniformIsotropicMaterial right)
        {
            return !Equals(left, right);
        }
    }
}
