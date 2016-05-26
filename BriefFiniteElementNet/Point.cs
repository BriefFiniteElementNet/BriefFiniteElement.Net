using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a Point in 3D space
    /// </summary>
    [Serializable]
    public struct Point :ISerializable
    {
        #region Properties

        private double x;
        private double y;
        private double z;

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
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
        /// The z.
        /// </value>
        public double Z
        {
            get { return z; }
            set { z = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates a p from the X and Y and Z.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public static Point FromXYZ(double x, double y, double z)
        {
            return new Point(x, y, z);
        }

        
        /// <summary>
        /// Gets a p witch is on origins (0,0,0).
        /// </summary>
        public static Point Origins
        {
            get { return new Point(0, 0, 0); }
        }
        
        

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Point(double x, double y, double z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region Operators

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>
        /// p1-p2
        /// </returns>
        public static Vector operator -(Point p1, Point p2)
        {
            return new Vector(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="v">The v.</param>
        /// <returns>
        /// p+v
        /// </returns>
        public static Point operator +(Point p, Vector v)
        {
            return new Point(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="v">The v.</param>
        /// <returns>
        /// p-v
        /// </returns>
        public static Point operator -(Point p, Vector v)
        {
            return new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
        }

        #endregion


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", x, y, z);
        }


        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x", x);
            info.AddValue("y", y);
            info.AddValue("z", z);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private Point(SerializationInfo info, StreamingContext context)
        {
            x = info.GetDouble("x");
            y = info.GetDouble("y");
            z = info.GetDouble("z");
        }

        #endregion

        public bool Equals(Point other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode*397) ^ y.GetHashCode();
                hashCode = (hashCode*397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
    }
}