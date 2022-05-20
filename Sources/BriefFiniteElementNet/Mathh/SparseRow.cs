using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Mathh
{
    /// <summary>
    /// Represents a class for a sparse row, containing indexes and values.
    /// </summary>
    public class SparseRow
    {
        public int Tag=-1;


        public int Size;

        public int Capacity
        {
            get
            {
                return Indexes.Length;
            }
            set
            {
                if (value < Capacity)
                {
                    return;
                }

                if (value != Indexes.Length)
                {
                    if (value > 0)
                    {
                        double[] newValues = new double[value];
                        int[] newIndexes = new int[value];


                        if (Size > 0)
                        {
                            Array.Copy(Values, 0, newValues, 0, Size);
                            Array.Copy(Indexes, 0, newIndexes, 0, Size);
                        }

                        Values = newValues;
                        Indexes = newIndexes;
                    }
                    else
                    {
                        Values = new double[0];
                        Indexes = new int[0];
                    }
                }
            }
        }

        public SparseRow(int capacity):this()
        {
            if (capacity < 0)
                throw new Exception();

            Capacity = capacity;
        }

        public SparseRow()
        {
            Indexes = new int[0];
            Values = new double[0];
        }


        private int _defaultCapacity = 10;

        public int[] Indexes;

        public double[] Values;

        public void Init(int capacity)
        {
            this.Indexes = new int[capacity];
            this.Values = new double[capacity];

            this.Capacity = capacity;
        }

        public void Add(int index,double value)
        {
            if (Size == Indexes.Length) EnsureCapacity(Size + 1);

            Indexes[Size] = index;
            Values[Size] = value;
            Size++;
        }

        private void EnsureCapacity(int min)
        {
            if (Indexes.Length < min)
            {
                int newCapacity = Indexes.Length == 0 ? _defaultCapacity : Indexes.Length * 2;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public void Multiply(double coef)
        {
            for (var i = 0; i < this.Size; i++)
                this.Values[i] *= coef;
        }

        /// <summary>
        /// Eliminates the specified index of <see cref="eliminated"/> row by adding appropriated multiply of <see cref="eliminator"/> to it.
        /// </summary>
        /// <param name="eliminator">The eliminator row.</param>
        /// <param name="eliminated">The eliminated row.</param>
        /// <param name="eliminateIndex">The index of eliminated member.</param>
        /// <returns>Eliminated version of <see cref="eliminated"/></returns>
        public static SparseRow Eliminate(SparseRow eliminator,SparseRow eliminated,int eliminateIndex,double epsilon=0)
        {
            var buf = new SparseRow(eliminated.Size + eliminator.Size);

            //based on https://stackoverflow.com/a/12993675/1106889
            //result = eliminator * coef + eliminated

            double coef;
            
            {
                var t1 = Array.BinarySearch(eliminator.Indexes, 0, eliminator.Size, eliminateIndex);
                var t2 = Array.BinarySearch(eliminated.Indexes, 0, eliminated.Size, eliminateIndex);

                if (t1 < 0 || t2 < 0)
                    throw new Exception();

                coef = -eliminated.Values[t2] / eliminator.Values[t1];
            }

            var c1 = 0;
            var c2 = 0;

            var i1s = eliminator.Indexes;
            var v1s = eliminator.Values;
            var l1 = eliminator.Size;

            var i2s = eliminated.Indexes;
            var v2s = eliminated.Values;
            var l2 = eliminated.Size;

            
            while (c1 < l1 && c2 < l2)
            {
                var i1 = i1s[c1];
                var i2 = i2s[c2];

                var v1 = v1s[c1];
                var v2 = v2s[c2];

                if (i1 > i2)
                {
                    buf.Add(i2/*or i1*/, v2);
                    c2++;
                }
                else if(i1 < i2)
                {
                    buf.Add(i1, v1 * coef);
                    c1++;
                }
                else
                {//common

                    if (i1 != i2)
                        throw new Exception();

                    var newValue = v1 * coef + v2;

                    if (i1 == eliminateIndex || Math.Abs(newValue) < epsilon)
                    {
                        //this is eliminated item, newValue should be zero and nothing to add
                        if (Math.Abs(newValue) > 1e-6)
                            throw new Exception("wrong result");
                    }
                    else
                    {
                        buf.Add(i1, newValue);
                    }

                    c1++;
                    c2++;
                }
            }


            //tail
            {
                for (; c1 < l1; c1++)
                {
                    var i1 = i1s[c1];
                    var v1 = v1s[c1];

                    buf.Add(i1, v1 * coef);
                }

                for (; c2 < l2; c2++)
                {
                    var i2 = i2s[c2];
                    var v2 = v2s[c2];

                    buf.Add(i2, v2);
                }
            }
            

            
            return buf;
        }

        /// <summary>
        /// Adds the specified sparse rows regarding defined multipliers.
        /// </summary>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a1Coef">The a1 coef.</param>
        /// <param name="a2Coef">The a2 coef.</param>
        /// <returns> a1 * a1Coef + a2 * a2Coef</returns>
        public static SparseRow Add(SparseRow a1, SparseRow a2, double a1Coef, double a2Coef)
        {
            var buf = new SparseRow(a2.Size + a1.Size);

            //based on https://stackoverflow.com/a/12993675/1106889
            //result = a1 * a1Coef + a2 * a2Coef;

            var c1 = 0;
            var c2 = 0;

            var i1s = a1.Indexes;
            var v1s = a1.Values;
            var l1 = a1.Size;

            var i2s = a2.Indexes;
            var v2s = a2.Values;
            var l2 = a2.Size;


            while (c1 < l1 && c2 < l2)
            {
                var i1 = i1s[c1];
                var i2 = i2s[c2];

                var v1 = v1s[c1];
                var v2 = v2s[c2];

                if (i1 > i2)
                {
                    buf.Add(i2/*or i1*/, v2 * a2Coef);
                    c2++;
                }
                else if (i1 < i2)
                {
                    buf.Add(i1, v1 * a1Coef);
                    c1++;
                }
                else
                {//common
                    var newValue = v1 * a1Coef + v2 * a2Coef;

                    buf.Add(i1, newValue);

                    c1++;
                    c2++;
                }
            }

            return buf;
        }


        /// <summary>
        /// Calculates the NNZ of specfied row.
        /// </summary>
        /// <param name="lastColumnIndex">Last index of the column.</param>
        /// <param name="tol">The epsilon.</param>
        /// <returns></returns>
        public int CalcNnz(int lastColumnIndex,double tol=1e-9)
        {
            var buf = 0;

            foreach (var tpl in this.EnumerateIndexed())
                if (tpl.Item1 != lastColumnIndex)
                    if (Math.Abs(tpl.Item2) > tol)
                        buf++;

            //var buf = Indexes.Count(i=>i!=lastColumnIndex);

            return buf;
        }

        public double GetRightSideValue(int lastColumnIndex)
        {
            var buf = Array.BinarySearch(Indexes, 0, Size, lastColumnIndex);

            if (buf < 0)
                return 0;
            else
                return Values[buf];
        }

        /// <summary>
        /// Determines whether the specified index exists int this row.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the specified index contains index; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsIndex(int index)
        {
            var buf = Array.BinarySearch(Indexes, 0, Size, index);
            return buf >= 0;
        }

        public double GetMember(int index)
        {
            var buf = Array.BinarySearch(Indexes, 0, Size, index);

            if (buf >= 0)
                return Values[buf];

            throw new Exception();
        }

        public bool IsZeroRow(double tol)
        {
            for (var i = 0; i < Size; i++)
                if (Math.Abs(Values[i]) > Math.Abs(tol))
                    return false;

            return true;
        }


        public void SetMember(int index, double value)
        {
            var buf = Array.BinarySearch(Indexes, 0, Size, index);

            if (buf >= 0)
                Values[buf] = value;
            else
                throw new Exception();
        }

        public IEnumerable<Tuple<int, double>> EnumerateIndexed()
        {
            for (var i = 0; i < Size; i++)
                yield return Tuple.Create(Indexes[i], Values[i]);
        }
    }
}
