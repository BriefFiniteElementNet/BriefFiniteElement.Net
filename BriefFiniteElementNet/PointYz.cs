using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a point in Y-Z plane with only two properties (Y and Z). 
    /// This struct is used for representing cross secion of members as as polygon.
    /// </summary>
    [Serializable]
    public struct PointYz : IEquatable<PointYz>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointYz"/> struct.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public PointYz(double y, double z)
        {
            this.y = y;
            this.z = z;
        }

        private double y;
        private double z;

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The Y component.
        /// </value>
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>
        /// The Z component.
        /// </value>
        public double Z
        {
            get { return z; }
            set { z = value; }
        }


        #region Equality members

        public bool Equals(PointYz other)
        {
            return y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointYz && Equals((PointYz) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (y.GetHashCode()*397) ^ z.GetHashCode();
            }
        }

        public static bool operator ==(PointYz left, PointYz right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointYz left, PointYz right)
        {
            return !left.Equals(right);
        }

        #endregion

    }
}
