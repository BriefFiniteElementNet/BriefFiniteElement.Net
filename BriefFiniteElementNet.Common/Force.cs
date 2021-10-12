﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Globalization;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a general concentrated force in 3D (can also include moments in addition to forces)
    /// </summary>
    [Serializable]
    public struct Force:ISerializable
    {
        #region Members

        /// <summary>
        /// A force with zero components
        /// </summary>
        public static Force Zero = new Force(0, 0, 0, 0, 0, 0);

        private double fx;
        private double fy;
        private double fz;

        private double mx;
        private double my;
        private double mz;



        /// <summary>
        /// Gets or sets the fx.
        /// </summary>
        /// <value>
        /// The X component of force 
        /// </value>
        public double Fx
        {
            get { return fx; }
            set { fx = value; }
        }

        /// <summary>
        /// Gets or sets the fy.
        /// </summary>
        /// <value>
        /// The y component of force.
        /// </value>
        public double Fy
        {
            get { return fy; }
            set { fy = value; }
        }

        /// <summary>
        /// Gets or sets the fz.
        /// </summary>
        /// <value>
        /// The z component of force.
        /// </value>
        public double Fz
        {
            get { return fz; }
            set { fz = value; }
        }



        /// <summary>
        /// Gets or sets the mx.
        /// </summary>
        /// <value>
        /// The x component of moment.
        /// </value>
        public double Mx
        {
            get { return mx; }
            set { mx = value; }
        }

        /// <summary>
        /// Gets or sets my.
        /// </summary>
        /// <value>
        /// the y component of moment.
        /// </value>
        public double My
        {
            get { return my; }
            set { my = value; }
        }

        /// <summary>
        /// Gets or sets the mz.
        /// </summary>
        /// <value>
        /// the z component of moment.
        /// </value>
        public double Mz
        {
            get { return mz; }
            set { mz = value; }
        }



        /// <summary>
        /// Gets or sets the forces.
        /// </summary>
        /// <value>
        /// The forces as a <see cref="Vector"/>.
        /// </value>
        public Vector Forces
        {
            get
            {
                return new Vector(fx,fy,fz);
            }

            set
            {
                this.fx = value.X;
                this.fy = value.Y;
                this.fz = value.Z;
            }
        }

        public static double[] ToVector(Force force)
        {
            return new double[] { force.fx, force.fy, force.fz, force.mx, force.my, force.mz };
        }


        /// <summary>
        /// Gets or sets the moments.
        /// </summary>
        /// <value>
        /// The moments as a <see cref="Vector"/>.
        /// </value>
        public Vector Moments
        {
            get
            {
                return new Vector(mx, my, mz);
            }

            set
            {
                this.mx = value.X;
                this.my = value.Y;
                this.mz = value.Z;
            }
        }

        #endregion

        #region Serialization Stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("fx", fx);
            info.AddValue("fy", fy);
            info.AddValue("fz", fz);

            info.AddValue("mx", mx);
            info.AddValue("my", my);
            info.AddValue("mz", mz);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Force"/> struct.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private Force(SerializationInfo info, StreamingContext context)
        {
            fx = info.GetDouble("fx");
            fy = info.GetDouble("fy");
            fz = info.GetDouble("fz");

            mx = info.GetDouble("mx");
            my = info.GetDouble("my");
            mz = info.GetDouble("mz");
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fx">X component of force</param>
        /// <param name="fy">Y component of force</param>
        /// <param name="fz">Z component of force</param>
        /// <param name="mx">X component of moment</param>
        /// <param name="my">Y component of moment</param>
        /// <param name="mz">Z component of moment</param>
        public Force(double fx, double fy, double fz, double mx, double my, double mz)
        {
            this.fx = fx;
            this.fy = fy;
            this.fz = fz;
            this.mx = mx;
            this.my = my;
            this.mz = mz;
        }

        /// <summary>
        /// Initiates a new instance of <see cref="Force"/>.
        /// </summary>
        /// <param name="forces">Force vector</param>
        /// <param name="moments">Moment vector</param>
        public Force(Vector forces,Vector moments) : this()
        {
            this.Forces = forces;
            this.Moments = moments;
        }

        /// <summary>
        /// Created the <see cref="Force"/> from a <see cref="Double"/> array.
        /// </summary>
        /// <param name="vec">The vec.</param>
        /// <param name="startIndex">The start index of forces vector.</param>
        /// <returns></returns>
        public static Force FromVector(double[] vec, int startIndex)
        {
            return new Force(
                vec[startIndex + 0],
                vec[startIndex + 1],
                vec[startIndex + 2],
                vec[startIndex + 3],
                vec[startIndex + 4],
                vec[startIndex + 5]);
        }

        #region Operators

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="f1">The f1.</param>
        /// <param name="f2">The f2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Force operator +(Force f1, Force f2)
        {
            return new Force(f1.fx + f2.fx, f1.fy + f2.fy, f1.fz + f2.fz, f1.mx + f2.mx, f1.my + f2.my, f1.mz + f2.mz);
        }


        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="f1">The f1.</param>
        /// <param name="f2">The f2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Force operator -(Force f1, Force f2)
        {
            return new Force(f1.fx - f2.fx, f1.fy - f2.fy, f1.fz - f2.fz, f1.mx - f2.mx, f1.my - f2.my, f1.mz - f2.mz);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="f">The f.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Force operator *(double c, Force f)
        {
            return new Force(c*f.fx, c*f.fy, c*f.fz, c*f.mx, c*f.my, c*f.mz);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="c">The c.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Force operator *(Force f, double c)
        {
            return new Force(c*f.fx, c*f.fy, c*f.fz, c*f.mx, c*f.my, c*f.mz);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Force operator -(Force f)
        {
            return new Force(-f.fx, -f.fy, -f.fz, -f.mx, -f.my, -f.mz);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator ==(Force f1, Force f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator !=(Force f1, Force f2)
        {
            return !f1.Equals(f2);
        }

        #endregion

        /// <summary>
        /// Dot divides the defined forces
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns>componentwise divide of two forces</returns>
        public static Force DotDivide(Force f1, Force f2)
        {
            var buf = new Force();

            buf.fx = f1.fx / f2.fx;
            buf.fy = f1.fy / f2.fy;
            buf.fz = f1.fz / f2.fz;

            buf.mx = f1.mx / f2.mx;
            buf.my = f1.my / f2.my;
            buf.mz = f1.mz / f2.mz;

            return buf;
        }

        public bool Equals(Force other)
        {
            return fx.Equals(other.fx) && fy.Equals(other.fy) && fz.Equals(other.fz) && mx.Equals(other.mx) && my.Equals(other.my) && mz.Equals(other.mz);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Force && Equals((Force) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = fx.GetHashCode();
                hashCode = (hashCode*397) ^ fy.GetHashCode();
                hashCode = (hashCode*397) ^ fz.GetHashCode();
                hashCode = (hashCode*397) ^ mx.GetHashCode();
                hashCode = (hashCode*397) ^ my.GetHashCode();
                hashCode = (hashCode*397) ^ mz.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var buf = string.Format(CultureInfo.CurrentCulture, "F: {0}, M: {1}", Forces, Moments);

            //MathUtil.FEquals(X, 0) && X != 0 ? "~0" : (object)X,
            //MathUtil.FEquals(Y, 0) && Y != 0 ? "~0" : (object)Y,
            //MathUtil.FEquals(Z, 0) && Z != 0 ? "~0" : (object)Z);
            return buf;
        }

        /// <summary>
        /// Gets the defined component of Force object
        /// </summary>
        /// <param name="dof"></param>
        /// <returns></returns>
        public double GetComponent(DoF dof)
        {
            switch (dof)
            {
                case DoF.Dx: return Fx;
                case DoF.Dy: return Fy;
                case DoF.Dz: return Fz;

                case DoF.Rx: return Mx;
                case DoF.Ry: return My;
                case DoF.Rz: return Mz;

                default: throw new ArgumentOutOfRangeException("dof");
            }
        }

        /// <summary>
        /// Sets the defined component of Force object
        /// </summary>
        /// <param name="dof"></param>
        /// <returns></returns>
        public void SetComponent(DoF dof,double value)
        {
            switch (dof)
            {
                case DoF.Dx: Fx = value; break;
                case DoF.Dy: Fy = value; break;
                case DoF.Dz: Fz = value; break;

                case DoF.Rx: Mx = value; break;
                case DoF.Ry: My = value; break;
                case DoF.Rz: Mz = value; break;

                default: throw new ArgumentOutOfRangeException("dof");
            }
        }
    }
}
