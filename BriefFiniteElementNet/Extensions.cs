using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BriefFiniteElementNet.CSparse.Double;

namespace BriefFiniteElementNet
{
    public static class Extensions
    {

        public static double Max(this double[] array)
        {
            if (array.Length == 0)
                throw new Exception();

            var max = array[0];

            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (array[i] > max)
                    max = array[i];
            }

            return max;
        }


        /// <summary>
        /// Simplify the accessing to <see cref="SerializationInfo.GetValue"/> and then casting the returned type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info">The information.</param>
        /// <param name="name">The name of field.</param>
        /// <returns></returns>
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T) info.GetValue(name, typeof (T));
        }

        public static Force Sum(this IEnumerable<Force> forces)
        {
            var buf = new Force();

            foreach (var force in forces)
            {
                buf += force;
            }

            return buf;
        }

        public static Force Move(this Force frc, Point location, Point destination)
        {
            var r = destination - location;
            
            var addMoment = Vector.Cross(r, frc.Forces);

            frc.Moments -= addMoment;

            return frc;
        }


        public static Force Move(this Force frc, Vector r)
        {
            var addMoment = Vector.Cross(r, frc.Forces);

            frc.Moments -= addMoment;

            return frc;
        }

        
        public static int IndexOfReference<T>(this IEnumerable<T> arr, T obj)
        {
            var cnt = arr.Count();

            if (typeof (T).IsValueType)
                throw new Exception();

            var i = 0;

            foreach (var arrMem in arr)
            {
                if (ReferenceEquals(arrMem, obj))
                    return i;
                i++;
            }

            return -1;
        }

        public static void Restart(this System.Diagnostics.Stopwatch sp)
        {
            sp.Stop();
            sp.Reset();
            sp.Start();
        }

        public static Matrix ToDenseMatrix(this CompressedColumnStorage csr)
        {
            var buf = new Matrix(csr.RowCount, csr.ColumnCount);

            for (int i = 0; i < csr.ColumnPointers.Length-1; i++)
            {
                var col = i;

                var st = csr.ColumnPointers[i];
                var en = csr.ColumnPointers[i+1];

                for (int j = st; j < en; j++)
                {
                    var row = csr.RowIndices[j];
                    buf[row, col] = csr.Values[j];
                }
            }

            return buf;
        }

        public static double GetLargestAbsoluteValue(this double[] vals)
        {
            var buf = 0.0;

            for (var i = 0; i < vals.Length; i++)
            {
                if (Math.Abs(vals[i]) > buf)
                    buf = Math.Abs(vals[i]);
            }

            return buf;
        }

        /// <summary>
        /// Transforms the force in local coordination system into the global coordination system.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <param name="force">The force.</param>
        /// <returns>transformed force</returns>
        public static Force TransformLocalToGlobal(this FrameElement2Node elm,Force force)
        {
            var f = elm.TransformLocalToGlobal(force.Forces);
            var m = elm.TransformLocalToGlobal(force.Moments);

            return new Force(f,m);
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


        /// <summary>
        /// Clones the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>a clone on the <see cref="model"/></returns>
        public static Model Clone(this Model model)
        {
            var str = new MemoryStream();

            Model.Save(str, model);

            str.Position = 0;

            var clone = Model.Load(str);

            return clone;
        }


        /// <summary>
        /// Returns the first occurrence of <see cref="value"/> in the <see cref="arr"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">The enumerable.</param>
        /// <param name="value">The value.</param>
        /// <returns>first occurrence of <see cref="value"/> in the <see cref="arr"/>.</returns>
        public static int FirstIndexOf<T>(this IEnumerable<T> arr, T value) where T : struct
        {
            var cnt = 0;

            foreach (var obj in arr)
            {
                if (obj.Equals(value))
                    return cnt;

                cnt++;
            }

            return -1;
        }


        /// <summary>
        /// Assembles the <see cref="submatrix"/> inside the <see cref="main"/> matrix.
        /// </summary>
        /// <param name="main">The main matrix.</param>
        /// <param name="submatrix">The sub matrix.</param>
        /// <param name="i">The i</param>
        /// <param name="j">The j</param>
        public static void AssembleInside(this Matrix main, Matrix submatrix, int i, int j)
        {
            if (main.RowCount < i + submatrix.RowCount || main.ColumnCount < j + submatrix.ColumnCount)
                throw new InvalidOperationException("dimension mismatch");

            for (var _i = 0; _i < submatrix.RowCount; _i++)
            {
                for (var _j = 0; _j < submatrix.ColumnCount; _j++)
                {
                    main[i + _i, j + _j] = submatrix[_i, _j];
                }
            }

        }


        /// <summary>
        /// Multiplies the <see cref="matrix"/> by a constant value.
        /// </summary>
        /// <param name="matrix">The Matrix</param>
        /// <param name="constant">The constant value</param>
        public static void MultiplyByConstant(this Matrix matrix, double constant)
        {
            for (var i = matrix.CoreArray.Length - 1; i != 0; i++)
            {
                matrix.CoreArray[i] *= constant;
            }
        }


        /// <summary>
        /// Gets the sum of external loads (both from external sources and supports) which are applying to the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="loadCase">The load case.</param>
        /// <returns>Resultant of external load on <see cref="node"/></returns>
        public static Force GetExternalLoads(this Node node, LoadCase loadCase)
        {
            var forces = node.Parent.LastResult.Forces[loadCase];

            var force = Force.FromVector(forces, 6*node.Index);

            return force;
        }

        public static void WriteValue<T>(this XmlWriter rwtr, string attributeName, T value)
        {
            if (typeof (T).IsValueType)
                WriteStructValue(rwtr, attributeName, (ValueType) (object) value);
            else
                WriteClassValue(rwtr, attributeName, (object) value);
        }

        public static void WriteClassValue(this XmlWriter rwtr, string attributeName, object value) 
        {
            if (!value.IsNull())
                rwtr.WriteAttributeString(attributeName, value.ToString());
        }

        public static void WriteStructValue(this XmlWriter rwtr, string attributeName, ValueType value) 
        {
            rwtr.WriteAttributeString(attributeName, value.ToString());
        }

        public static bool IsNull<T>(this T obj) where T : class
        {
            return ReferenceEquals(obj, null);
        }
    }
}
