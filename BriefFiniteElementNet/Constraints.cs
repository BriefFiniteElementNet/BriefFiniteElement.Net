using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// represents some predefined most used <see cref="Constraint"/> for <see cref="Node"/>s.
    /// </summary>
    public static class Constraints
    {
        /// <summary>
        /// Gets a totally fixed Constraint.
        /// </summary>
        /// <value>
        /// A totally Fixed Constraint.
        /// </value>
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
        public static Constraint Released
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
        public static Constraint FixedRZ
        {
            get
            {
                return new Constraint(
                    DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed);
            }
        }


    }
}
