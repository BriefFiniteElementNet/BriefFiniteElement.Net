using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the constraints on a single node with 6 independent degrees of freedom (3 movement and 3 rotational).
    /// </summary>
    [Serializable]
    public struct Constraint :ISerializable, IEquatable<Constraint>
    {

        #region Static Members

        /// <summary>
        /// Gets a totally fixed Constraint.
        /// </summary>
        /// <value>
        /// A totally Fixed Constraint.
        /// </value>
        [Obsolete("Use Constraints.Fixed static class")]
        public static Constraint Fixed
        {
            get
            {
                return new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed);
            }
        }


        /// <summary>
        /// Gets a totally free Constraint.
        /// </summary>
        /// <value>
        /// A totally Released Constraint.
        /// </value>
        [Obsolete("Use Constraints.Released static class")]
        public static Constraint Released
        {
            get
            {
                return new Constraint(DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released);
            }
        }

        /// <summary>
        /// Gets a totally free Constraint.
        /// </summary>
        /// <value>
        /// The free.
        /// </value>
        [Obsolete("Use Constraint.Released instead")]
        public static Constraint Free
        {
            get
            {
                return new Constraint(DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released);
            }
        }

        /// <summary>
        /// Gets a movement fixed (but rotation free) Constraint.
        /// </summary>
        /// <value>
        /// A Movement Fixed and Rotation Released Constraint.
        /// </value>
        [Obsolete("Use Constraints.MovementFixed static class")]
        public static Constraint MovementFixed
        {
            get
            {
                return new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released);
            }
        }


        /// <summary>
        /// Gets a rotation fixed (but movement free) Constraint.
        /// </summary>
        /// <value>
        /// A Rotation Fixed and Movement Released Constraint.
        /// </value>
        [Obsolete("Use Constraints.RotationFixed static class")]
        public static Constraint RotationFixed
        {
            get
            {
                return new Constraint(DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed);
            }
        }


        /// <summary>
        /// Gets a totally free but DX fixed Constraint.
        /// </summary>
        /// <value>
        /// A DX fixed constraint.
        /// </value>
        [Obsolete("Use Constraints.FixedDX static class")]
        public static Constraint FixedDX
        {
            get
            {
                return new Constraint(
                    DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released);
            }
        }


        /// <summary>
        /// Gets a totally free but DY fixed Constraint.
        /// </summary>
        /// <value>
        /// A DY fixed constraint.
        /// </value>
        [Obsolete("Use Constraints.FixedDY static class")]
        public static Constraint FixedDY
        {
            get
            {
                return new Constraint(
                    DofConstraint.Released, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released);
            }
        }


        /// <summary>
        /// Gets a totally free but DZ fixed Constraint.
        /// </summary>
        /// <value>
        /// A DZ fixed constraint.
        /// </value>
        [Obsolete("Use Constraints.FixedDZ static class")]
        public static Constraint FixedDZ
        {
            get
            {
                return new Constraint(
                    DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released);
            }
        }


        /// <summary>
        /// Gets a totally free but RX fixed Constraint.
        /// </summary>
        /// <value>
        /// A RX fixed constraint.
        /// </value>
        [Obsolete("Use Constraints.FixedRX static class")]
        public static Constraint FixedRX
        {
            get
            {
                return new Constraint(
                    DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed, DofConstraint.Released, DofConstraint.Released);
            }
        }


        /// <summary>
        /// Gets a totally free but RY fixed Constraint.
        /// </summary>
        /// <value>
        /// A RY fixed constraint.
        /// </value>
        [Obsolete("Use Constraints.FixedRY static class")]
        public static Constraint FixedRY
        {
            get
            {
                return new Constraint(
                    DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed, DofConstraint.Released);
            }
        }


        /// <summary>
        /// Gets a totally free but RZ fixed Constraint.
        /// </summary>
        /// <value>
        /// A RZ fixed constraint.
        /// </value>
        [Obsolete("Use Constraints.FixedRZ static class")]
        public static Constraint FixedRZ
        {
            get
            {
                return new Constraint(
                    DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint"/> struct.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="dz">The dz.</param>
        /// <param name="rx">The rx.</param>
        /// <param name="ry">The ry.</param>
        /// <param name="rz">The rz.</param>
        public Constraint(DofConstraint dx, DofConstraint dy, DofConstraint dz, DofConstraint rx, DofConstraint ry, DofConstraint rz)
        {
            this.dx = dx;
            this.dy = dy;
            this.dz = dz;
            this.ry = ry;
            this.rz = rz;
            this.rx = rx;
        }

        #region Members

        private DofConstraint dx;
        private DofConstraint dy;
        private DofConstraint dz;

        private DofConstraint rx;
        private DofConstraint ry;
        private DofConstraint rz;

        /// <summary>
        /// Gets or sets the dx.
        /// </summary>
        /// <value>
        /// Constraint on degree of freedom of movement in X direction
        /// </value>
        public DofConstraint DX
        {
            get { return dx; }
            set { dx = value; }
        }

        /// <summary>
        /// Gets or sets the dy.
        /// </summary>
        /// <value>
        /// Constraint on degree of freedom of movement in Y direction
        /// </value>
        public DofConstraint DY
        {
            get { return dy; }
            set { dy = value; }
        }

        /// <summary>
        /// Gets or sets the dz.
        /// </summary>
        /// <value>
        /// Constraint on degree of freedom of movement in Z direction
        /// </value>
        public DofConstraint DZ
        {
            get { return dz; }
            set { dz = value; }
        }

        /// <summary>
        /// Gets or sets the rx.
        /// </summary>
        /// <value>
        /// Constraint on degree of freedom of rotation in X direction
        /// </value>
        public DofConstraint RX
        {
            get { return rx; }
            set { rx = value; }
        }

        /// <summary>
        /// Gets or sets the ry.
        /// </summary>
        /// <value>
        /// Constraint on degree of freedom of rotation in Y direction
        /// </value>
        public DofConstraint RY
        {
            get { return ry; }
            set { ry = value; }
        }

        /// <summary>
        /// Gets or sets the rz.
        /// </summary>
        /// <value>
        /// Constraint on degree of freedom of rotation in Z direction
        /// </value>
        public DofConstraint RZ
        {
            get { return rz; }
            set { rz = value; }
        }

        #endregion

        /// <summary>
        /// Creates new Constraint based on <see cref="constraint" /> parameter.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>new constraint</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static Constraint FromString(string constraint)
        {
            if (string.IsNullOrEmpty(constraint))
                throw new ArgumentException("constraint");

            if (constraint.Length != 6)
                throw new ArgumentException("constraint");


            for (var i = 0; i < constraint.Length; i++)
            {
                if (!char.IsNumber(constraint[i]))
                    throw new ArgumentException("constraint");

                if (char.GetNumericValue(constraint[i]) != 0 && char.GetNumericValue(constraint[i]) != 1)
                    throw new ArgumentException("constraint");
            }


            var buf = new Constraint();

            buf.DX = (DofConstraint)(int)char.GetNumericValue(constraint[0]);
            buf.DY = (DofConstraint)(int)char.GetNumericValue(constraint[1]);
            buf.DZ = (DofConstraint)(int)char.GetNumericValue(constraint[2]);
            
            buf.RX = (DofConstraint)(int)char.GetNumericValue(constraint[3]);
            buf.RY = (DofConstraint)(int)char.GetNumericValue(constraint[4]);
            buf.RZ = (DofConstraint)(int)char.GetNumericValue(constraint[5]);

            return buf;
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
            info.AddValue("dx", (int) dx);
            info.AddValue("dy", (int) dy);
            info.AddValue("dz", (int) dz);

            info.AddValue("rx", (int) rx);
            info.AddValue("ry", (int) ry);
            info.AddValue("rz", (int) rz);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint"/> struct.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private Constraint(SerializationInfo info, StreamingContext context) : this()
        {
            this.dx = (DofConstraint) info.GetInt32("dx");
            this.dy = (DofConstraint) info.GetInt32("dy");
            this.dz = (DofConstraint) info.GetInt32("dz");

            this.rx = (DofConstraint) info.GetInt32("rx");
            this.ry = (DofConstraint) info.GetInt32("ry");
            this.rz = (DofConstraint) info.GetInt32("rz");
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
            if (this.Equals(Released))
                return "Release";

            if (this.Equals(Fixed))
                return "Fixed";

            if (this.Equals(MovementFixed))
                return "Movement Fixed";

            if (this.Equals(RotationFixed))
                return "Rotation Fixed";

            var arr = new DofConstraint[] {dx, dy, dz, rx, ry, rz};

            var buf = new char[6];

            for (int i = 0; i < 6; i++)
            {
                buf[i] = arr[i] == DofConstraint.Fixed ? '1' : '0';
            }

            return new string(buf);
        }

        public string ToString_01()
        {
            var arr = new DofConstraint[] { dx, dy, dz, rx, ry, rz };

            var buf = new char[12];

            for (int i = 0; i < 6; i++)
            {
                buf[2 * i + 0] = arr[i] == DofConstraint.Fixed ? '1' : '0';
                buf[2 * i + 1] = ' ';
            }

            return new string(buf).Trim();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Constraint other)
        {
            return dx == other.dx && dy == other.dy && dz == other.dz && rx == other.rx && ry == other.ry && rz == other.rz;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Constraint && Equals((Constraint) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) dx;
                hashCode = (hashCode*397) ^ (int) dy;
                hashCode = (hashCode*397) ^ (int) dz;
                hashCode = (hashCode*397) ^ (int) rx;
                hashCode = (hashCode*397) ^ (int) ry;
                hashCode = (hashCode*397) ^ (int) rz;
                return hashCode;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Constraint left, Constraint right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Constraint left, Constraint right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &amp;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Constraint operator &(Constraint left, Constraint right)
        {
            var buf = new Constraint();

            if (left.dx == DofConstraint.Fixed || right.dx == DofConstraint.Fixed)
                buf.dx = DofConstraint.Fixed;

            if (left.dy == DofConstraint.Fixed || right.dy == DofConstraint.Fixed)
                buf.dy = DofConstraint.Fixed;

            if (left.dz == DofConstraint.Fixed || right.dz == DofConstraint.Fixed)
                buf.dz = DofConstraint.Fixed;


            if (left.rx == DofConstraint.Fixed || right.rx == DofConstraint.Fixed)
                buf.rx = DofConstraint.Fixed;

            if (left.ry == DofConstraint.Fixed || right.ry == DofConstraint.Fixed)
                buf.ry = DofConstraint.Fixed;

            if (left.rz == DofConstraint.Fixed || right.rz == DofConstraint.Fixed)
                buf.rz = DofConstraint.Fixed;

            return buf;
        }
    }
}
