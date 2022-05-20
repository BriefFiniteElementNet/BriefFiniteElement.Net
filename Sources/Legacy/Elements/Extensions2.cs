using System;
using System.Collections.Generic;
using System.Text;

namespace BriefFiniteElementNet
{
    public static class Extensions2
    {

        /// <summary>
        /// Transforms the force in local coordination system into the global coordination system.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <param name="force">The force.</param>
        /// <returns>transformed force</returns>
        public static Force TransformLocalToGlobal(this FrameElement2Node elm, Force force)
        {
            var f = elm.TransformLocalToGlobal(force.Forces);
            var m = elm.TransformLocalToGlobal(force.Moments);

            return new Force(f, m);
        }

        public static Vector TransformLocalToGlobal(this FrameElement2Node elm, Vector vec)
        {
            var buf = elm.TransformLocalToGlobal(vec);

            return buf;
        }

        /// <summary>
        /// Transforms the force in global coordination system into the local coordination system.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <param name="force">The force.</param>
        /// <returns>transformed force</returns>
        public static Force TransformGlobalToLocal(this FrameElement2Node elm, Force force)
        {
            var f = elm.TransformGlobalToLocal(force.Forces);
            var m = elm.TransformGlobalToLocal(force.Moments);

            return new Force(f, m);
        }

        public static Vector TransformGlobalToLocal(this FrameElement2Node elm, Vector vec)
        {
            var f = elm.TransformGlobalToLocal(vec);

            return f;
        }

        /// <summary>
        /// Applies the release matrix to calculated local end forces.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="localEndForces">The local end forces.</param>
        /// <returns></returns>
        /// <remarks>
        /// When <see cref="FrameElement2Node"/> has one or two hinged ends, then local end forces due to element interior loads (like distributed loads) 
        /// will be different than normal ones (both ends fixed). This method will apply end releases...
        /// </remarks>
        /// <exception cref="System.NotImplementedException"></exception>
        public static Force[] ApplyReleaseMatrixToEndForces(FrameElement2Node element, Force[] localEndForces)
        {
            if (localEndForces.Length != 2)
                throw new InvalidOperationException();

            var fullLoadVector = new double[12];//for applying release matrix


            {
                fullLoadVector[00] = localEndForces[0].Fx;
                fullLoadVector[01] = localEndForces[0].Fy;
                fullLoadVector[02] = localEndForces[0].Fz;
                fullLoadVector[03] = localEndForces[0].Mx;
                fullLoadVector[04] = localEndForces[0].My;
                fullLoadVector[05] = localEndForces[0].Mz;

                fullLoadVector[06] = localEndForces[1].Fx;
                fullLoadVector[07] = localEndForces[1].Fy;
                fullLoadVector[08] = localEndForces[1].Fz;
                fullLoadVector[09] = localEndForces[1].Mx;
                fullLoadVector[10] = localEndForces[1].My;
                fullLoadVector[11] = localEndForces[1].Mz;
            }

            var ld = Matrix.OfVector(fullLoadVector);
            var rsm = element.GetReleaseMatrix();
            ld = rsm * ld;

            var buf = new Force[2];

            buf[0] = Force.FromVector(ld.Values, 0);
            buf[1] = Force.FromVector(ld.Values, 6);

            return buf;
        }
    }
}
