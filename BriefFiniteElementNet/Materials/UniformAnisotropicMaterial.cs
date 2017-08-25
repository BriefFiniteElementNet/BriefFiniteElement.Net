using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Materials
{
    /// <summary>
    /// Represents a uniform (not varying) anisotropic material.
    /// </summary>
    [Serializable]
    public class UniformAnisotropicMaterial : BaseMaterial
    {
        #region props

        private double _ex;
        private double _ey;
        private double _ez;

        private double _nuXy;
        private double _nuYx;

        private double _nuYz;
        private double _nuZy;

        private double _nuZx;
        private double _nuXz;

        private double rho, mu;

        /// <summary>
        /// Gets or sets the Young's modulus in X direction.
        /// </summary>
        /// <value>
        /// The ex.
        /// </value>
        public double Ex
        {
            get { return _ex; }
            set { _ex = value; }
        }

        /// <summary>
        /// Gets or sets the Young's modulus in Y direction.
        /// </summary>
        /// <value>
        /// The ey.
        /// </value>
        public double Ey
        {
            get { return _ey; }
            set { _ey = value; }
        }

        /// <summary>
        /// Gets or sets the Young's modulus in Z direction.
        /// </summary>
        /// <value>
        /// The ez.
        /// </value>
        public double Ez
        {
            get { return _ez; }
            set { _ez = value; }
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


        public double NuXy
        {
            get { return _nuXy; }
            set { _nuXy = value; }
        }

        public double NuYz
        {
            get { return _nuYz; }
            set { _nuYz = value; }
        }

        public double NuZx
        {
            get { return _nuZx; }
            set { _nuZx = value; }
        }

        public double NuYx
        {
            get { return _nuYx; }
            set { _nuYx = value; }
        }

        public double NuZy
        {
            get { return _nuZy; }
            set { _nuZy = value; }
        }

        public double NuXz
        {
            get { return _nuXz; }
            set { _nuXz = value; }
        }
        

        #endregion

        public override AnisotropicMaterialInfo GetMaterialPropertiesAt(params double[] isoCoords)
        {
            var buf = new AnisotropicMaterialInfo();

            buf.Ex = this.Ex;
            buf.Ey = this.Ey;
            buf.Ez = this.Ez;

            buf.NuXy = this.NuXy;
            buf.NuYx = this.NuYx;

            buf.NuXz = this.NuXz;
            buf.NuZx = this.NuZx;

            buf.NuZy = this.NuZy;
            buf.NuYz = this.NuYz;

            buf.Rho = this.rho;
            buf.Mu = this.mu;

            return buf;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new[] {0, 0, 0};
        }

        protected UniformAnisotropicMaterial(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _ex = info.GetDouble("_ex");
            _ey = info.GetDouble("_ey");
            _ez = info.GetDouble("_ez");

            _nuXy = info.GetDouble("_nuXy");
            _nuYx = info.GetDouble("_nuYx");

            _nuXz = info.GetDouble("_nuXz");
            _nuZx = info.GetDouble("_nuZx");

            _nuYz = info.GetDouble("_nuYz");
            _nuZy = info.GetDouble("_nuZy");

            rho = info.GetDouble("rho");
            mu = info.GetDouble("mu");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_ex", _ex);
            info.AddValue("_ey", _ey);
            info.AddValue("_ez", _ez);

            info.AddValue("_nuXy", _nuXy);
            info.AddValue("_nuYx", _nuYx);

            info.AddValue("_nuXz", _nuXz);
            info.AddValue("_nuZx", _nuZx);

            info.AddValue("_nuYz", _nuYz);
            info.AddValue("_nuZy", _nuZy);

            info.AddValue("rho", rho);
            info.AddValue("mu", mu);

            base.GetObjectData(info, context);
        }

        public UniformAnisotropicMaterial()
        {
        }
    }
}
