using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Determines Displacement of a node in 3D (with 6 DoFs)
    /// </summary>
    [Serializable]
    public struct Displacement :ISerializable
    {
        #region Members

        private double dx;
        private double dy;
        private double dz;

        private double rx;
        private double ry;
        private double rz;

        /// <summary>
        /// Gets or sets the dx.
        /// </summary>
        /// <value>
        /// The movement in X direction.
        /// </value>
        public double DX
        {
            get { return dx; }
            set { dx = value; }
        }

        /// <summary>
        /// Gets or sets the dy.
        /// </summary>
        /// <value>
        /// The movement in Y direction.
        /// </value>
        public double DY
        {
            get { return dy; }
            set { dy = value; }
        }


        /// <summary>
        /// Gets or sets the dz.
        /// </summary>
        /// <value>
        /// The movement in Z direction.
        /// </value>
        public double DZ
        {
            get { return dz; }
            set { dz = value; }
        }

        /// <summary>
        /// Gets or sets the rx.
        /// </summary>
        /// <value>
        /// The rotation in X direction (in radian).
        /// </value>
        public double RX
        {
            get { return rx; }
            set { rx = value; }
        }

        /// <summary>
        /// Gets or sets the ry.
        /// </summary>
        /// <value>
        /// The rotation in Y direction (in radian).
        /// </value>
        public double RY
        {
            get { return ry; }
            set { ry = value; }
        }

        /// <summary>
        /// Gets or sets the rz.
        /// </summary>
        /// <value>
        /// The rotation in Z direction (in radian).
        /// </value>
        public double RZ
        {
            get { return rz; }
            set { rz = value; }
        }





        /// <summary>
        /// Gets or sets the displacements.
        /// </summary>
        /// <value>
        /// The displacements as a <see cref="Vector" />.
        /// </value>
        public Vector Displacements
        {
            get { return new Vector(dx, dy, dz); }
            set
            {
                this.dx = value.X;
                this.dy = value.Y;
                this.dz = value.Z;
            }
        }



        /// <summary>
        /// Gets or sets the rotations.
        /// </summary>
        /// <value>
        /// The rotations as a <see cref="Vector" /> all in Radian.
        /// </value>
        public Vector Rotations
        {
            get { return new Vector(rx, ry, rz); }
            set
            {
                this.rx = value.X;
                this.ry = value.Y;
                this.rz = value.Z;
            }
        }

        #endregion

        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dx", dx);
            info.AddValue("dy", dy);
            info.AddValue("dz", dz);

            info.AddValue("rx", rx);
            info.AddValue("ry", ry);
            info.AddValue("rz", rz);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Displacement"/> struct.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private Displacement(SerializationInfo info, StreamingContext context)
        {
            this.dx = info.GetDouble("dx");
            this.dy = info.GetDouble("dy");
            this.dz = info.GetDouble("dz");

            this.rx = info.GetDouble("rx");
            this.ry = info.GetDouble("ry");
            this.rz = info.GetDouble("rz");
        }

        #endregion


        /// <summary>
        /// Gets a zero displacement.
        /// </summary>
        /// <value>
        /// A zero displacement.
        /// </value>
        public static Displacement Zero
        {
            get { return new Displacement(0, 0, 0, 0, 0, 0); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Displacement"/> struct.
        /// </summary>
        /// <param name="dx">The displacement in x direction.</param>
        /// <param name="dy">The displacement in y direction.</param>
        /// <param name="dz">The displacement in z direction.</param>
        /// <param name="rx">The rotation in x direction.</param>
        /// <param name="ry">The rotation in y direction.</param>
        /// <param name="rz">The rotation in z direction.</param>
        public Displacement(double dx, double dy, double dz, double rx, double ry, double rz)
        {
            this.dx = dx;
            this.dy = dy;
            this.dz = dz;
            this.rx = rx;
            this.ry = ry;
            this.rz = rz;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Displacement"/> struct.
        /// </summary>
        /// <param name="displacements">The displacements.</param>
        /// <param name="rotations">The rotations.</param>
        public Displacement(Vector displacements, Vector rotations) : this()
        {
            this.Displacements = displacements;
            this.Rotations = rotations;
        }



        /// <summary>
        /// Created the <see cref="Displacement"/> from a <see cref="Double"/> array.
        /// </summary>
        /// <param name="vec">The vec.</param>
        /// <param name="startIndex">The start index of displacement vector.</param>
        /// <returns></returns>
        public static Displacement FromVector(double[] vec, int startIndex)
        {
            return new Displacement(
                vec[startIndex + 0],
                vec[startIndex + 1],
                vec[startIndex + 2],
                vec[startIndex + 3],
                vec[startIndex + 4],
                vec[startIndex + 5]);
        }

        public static double[] ToVector(Displacement disp)
        {
            var buf = new double[]
            {
                disp.DX, disp.DY, disp.DZ,
                disp.RX, disp.RY, disp.RZ,
            };

            return buf;
        }

        #region Operator Overloads

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Displacement operator +(Displacement d1, Displacement d2)
        {
            return new
                Displacement(d1.dx + d2.dx,
                    d1.dy + d2.dy,
                    d1.dz + d2.dz,
                    d1.rx + d2.rx,
                    d1.ry + d2.ry,
                    d1.rz + d2.rz);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Displacement operator -(Displacement d1, Displacement d2)
        {
            return new
                Displacement(d1.dx - d2.dx,
                    d1.dy - d2.dy,
                    d1.dz - d2.dz,
                    d1.rx - d2.rx,
                    d1.ry - d2.ry,
                    d1.rz - d2.rz);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Displacement operator *(double c, Displacement d)
        {
            return new
                Displacement(c*d.dx,
                    c*d.dy,
                    c*d.dz,
                    c*d.rx,
                    c*d.ry,
                    c*d.rz);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="c">The c.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Displacement operator *(Displacement d, double c)
        {
            return new
                Displacement(
                    c * d.dx,
                    c * d.dy,
                    c * d.dz,
                    c * d.rx,
                    c * d.ry,
                    c * d.rz);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Displacement operator -(Displacement d1)
        {
            return new
                Displacement(-d1.dx, -d1.dy, -d1.dz, -d1.rx, -d1.ry, -d1.rz);
        }
        #endregion

        #region Equality members

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Displacement obj)
        {
            return dx.Equals(obj.dx) && dy.Equals(obj.dy) && dz.Equals(obj.dz) && rx.Equals(obj.rx) &&
                   ry.Equals(obj.ry) && rz.Equals(obj.rz);
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
            if (ReferenceEquals(null, obj)) return false;
            return obj is Displacement && Equals((Displacement) obj);
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
                int hashCode = dx.GetHashCode();
                hashCode = (hashCode*397) ^ dy.GetHashCode();
                hashCode = (hashCode*397) ^ dz.GetHashCode();
                hashCode = (hashCode*397) ^ rx.GetHashCode();
                hashCode = (hashCode*397) ^ ry.GetHashCode();
                hashCode = (hashCode*397) ^ rz.GetHashCode();
                return hashCode;
            }
        }


        public static bool operator ==(Displacement left, Displacement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Displacement left, Displacement right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Δx : {0:e2}, Δy : {1:e2}, Δz : {2:e2}, θx : {3:e2}, θy : {4:e2}, θz : {5:e2}", this.dx, this.dy, this.dz, this.rx, this.ry, this.rz);
        }

        public string ToString(int decimalDigits)
        {
            var d = decimalDigits;

            return string.Format("Δx : {0:e" + d + "}, Δy : {1:e" + d + "}, Δz : {2:e" + d + "}, θx : {3:e" + d + "}, θy : {4:e" + d + "}, θz : {5:e" + d + "}", this.dx, this.dy, this.dz, this.rx, this.ry, this.rz, decimalDigits);

        }
    }
}
