using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a point in Y-Z plane with only two properties (Y and Z). 
    /// This struct is used for representing cross section of members as polygon.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Y:{Y}, Z:{Z}")]
    public struct PointYZ : IEquatable<PointYZ>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointYZ"/> struct.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public PointYZ(double y, double z)
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

        public bool Equals(PointYZ other)
        {
            return y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointYZ && Equals((PointYZ) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (y.GetHashCode()*397) ^ z.GetHashCode();
            }
        }

        public static bool operator ==(PointYZ left, PointYZ right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointYZ left, PointYZ right)
        {
            return !left.Equals(right);
        }

        #endregion

    }
}
