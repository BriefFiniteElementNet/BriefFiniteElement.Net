using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BriefFiniteElementNet.Mathh
{
    public class LinkedListIrrefFinder
    {
        public class SparseRow
        {
            //private Dictionary<int, double> Values;

            private List<int> Columns;//Columns[i]: column num of i'th
            private List<int> NextMember;//NextMember[i]: index of next member after i'th
            private List<double> Values;//Values[i]: value of i'th
            public int Head = -1;

            public int I;

            public static SparseRow From(int[] columns,double[] values)
            {
                Array.Sort(columns, values);
                Array.Sort(columns);

                var buf = new SparseRow();

                buf.Columns = new List<int>(columns);
                buf.Values = new List<double>(values);
                buf.NextMember = new List<int>(Enumerable.Range(0, columns.Length).Select(i => i + 1));
                buf.NextMember[buf.NextMember.Count - 1] = -1;
                buf.Head = 0;

                return buf;
            }

            /// <summary>
            /// Adds the addition into the target
            /// </summary>
            /// <param name="target"></param>
            /// <param name="addition"></param>
            /// <param name="factor"></param>
            public static void Add(SparseRow target, SparseRow addition, double factor)
            {
                var it = target.Head;
                var ia = addition.Head;

                while (true)
                {
                    var c1 = target.Columns[it];
                    var c2 = addition.Columns[ia];

                    while (c2 < c1)
                    {

                    }


                    it = target.NextMember[it];

                }

                throw new NotImplementedException();
            }
        }
    }
}
