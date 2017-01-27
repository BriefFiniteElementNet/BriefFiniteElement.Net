using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for managing the linear coordination transformation from local to global and vice versa.
    /// </summary>
    /// <remarks>
    /// Planned to be used for transforming local to global
    /// means <see cref="TransformLocalToGlobal(BriefFiniteElementNet.Vector)"/> will transform from local to global, and <see cref="TransformGlobalToLocal"/> converts from global to local.
    /// </remarks>
    public class LocalGlobalTransformManager
    {
        /// <summary>
        /// The transform
        /// </summary>
        /// <remarks>
        /// transform matrix, which when applied to local vector, </remarks>
        public Matrix TransformMatrix;

        /// <summary>
        /// Transforms the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Transformed vector (vector in new coordination system : global)</returns>
        public Vector TransformLocalToGlobal(Vector vector)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * vector.X +
                lambda[0, 1] * vector.Y +
                lambda[0, 2] * vector.Z;

            buf.Y =
                lambda[1, 0] * vector.X +
                lambda[1, 1] * vector.Y +
                lambda[1, 2] * vector.Z;

            buf.Z =
                lambda[2, 0] * vector.X +
                lambda[2, 1] * vector.Y +
                lambda[2, 2] * vector.Z;

            return buf;
        }

        /// <summary>
        /// Transforms back the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Back transformed vector (vector in original coordination system : local)</returns>
        public Vector TransformGlobalToLocal(Vector vector)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * vector.X +
                lambda[1, 0] * vector.Y +
                lambda[2, 0] * vector.Z;

            buf.Y =
                lambda[0, 1] * vector.X +
                lambda[1, 1] * vector.Y +
                lambda[2, 1] * vector.Z;

            buf.Z =
                lambda[0, 2] * vector.X +
                lambda[1, 2] * vector.Y +
                lambda[2, 2] * vector.Z;

            return buf;
        }

        /// <summary>
        /// Transforms the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Transformed point (point in new coordination system : global)</returns>
        public Vector TransformLocalToGlobal(Point point)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * point.X +
                lambda[0, 1] * point.Y +
                lambda[0, 2] * point.Z;

            buf.Y =
                lambda[1, 0] * point.X +
                lambda[1, 1] * point.Y +
                lambda[1, 2] * point.Z;

            buf.Z =
                lambda[2, 0] * point.X +
                lambda[2, 1] * point.Y +
                lambda[2, 2] * point.Z;

            return buf;
        }

        /// <summary>
        /// Transforms back the specified point.
        /// </summary>
        /// <param name="point">The vector.</param>
        /// <returns>Back transformed point (point in original coordination system : local)</returns>
        public Vector TransformGlobalToLocal(Point point)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * point.X +
                lambda[1, 0] * point.Y +
                lambda[2, 0] * point.Z;

            buf.Y =
                lambda[0, 1] * point.X +
                lambda[1, 1] * point.Y +
                lambda[2, 1] * point.Z;

            buf.Z =
                lambda[0, 2] * point.X +
                lambda[1, 2] * point.Y +
                lambda[2, 2] * point.Z;

            return buf;
        }


    }
}
