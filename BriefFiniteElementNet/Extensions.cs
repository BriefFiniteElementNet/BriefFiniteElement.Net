using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;

namespace BriefFiniteElementNet
{
    public static class Extensions
    {

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
    }
}
