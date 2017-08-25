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
    /// Represents a base class for general 3D materials
    /// </summary>
    [Serializable]
    public abstract class BaseMaterial : ISerializable, IEquatable<BaseMaterial>
    {
        /// <summary>
        /// Gets the material properties at defined location.
        /// </summary>
        /// <param name="isoCoords">the isometric coordination, order: xi eta gama</param>
        /// <returns>the material of element, in local element's coordination system</returns>
        public abstract AnisotropicMaterialInfo GetMaterialPropertiesAt(params double[] isoCoords);

        /// <summary>
        /// Gets the maximum order (degree) of members of material regarding xi - eta and gama.
        /// </summary>
        /// <remarks>
        /// Will use for determining Gaussian sampling count.
        /// Assuming each property of material is a polynomial function of xi, eta and nu, this will return
        /// maximum degree of polynomial in xi, eta and gama directions.
        /// Ex = 3*xi^2+5*eta^3+1*xi : E: [2,3,0]
        /// Ex = 3*xi^2+5*eta^3+1*xi, nu_xy=7*xi^5 : E: [2,3,0], Nu: [5,0,0] >> result: [5,3,0] (maximum of each one). 
        /// </remarks>
        /// <returns>the number of Gaussian integration points needed in xi, eta ann nu directions</returns>
        public abstract int[] GetMaxFunctionOrder();

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        protected BaseMaterial(SerializationInfo info, StreamingContext context)
        {
        }

        public BaseMaterial()
        {
        }

        public bool Equals(BaseMaterial other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseMaterial) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(BaseMaterial left, BaseMaterial right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseMaterial left, BaseMaterial right)
        {
            return !Equals(left, right);
        }
    }
}