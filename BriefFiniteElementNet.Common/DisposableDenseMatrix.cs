using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Common
{
    /// <summary>
    /// Represents a class for disposable, poolable dense matrix.
    /// It uses a MatrixPool to prevent allocate and free values array each time and improves performance noticeably
    /// </summary>
    /// <remarks>
    /// This is disposable, when disposed will return core array into the pool
    /// </remarks>
    public class DisposableDenseMatrix : CSparse.Double.DenseMatrix, IDisposable
    {

        ArrayPool<double> Pool;


        public DisposableDenseMatrix(int rows, int columns) : base(rows, columns)
        {
        }

        public DisposableDenseMatrix(int rows, int columns, double[] values) : base(rows, columns, values)
        {
        }

        public DisposableDenseMatrix(int rows, int columns, ArrayPool<double> pool) : this(rows, columns, pool.Allocate(rows * columns), pool)
        {

        }

        public DisposableDenseMatrix(int rows, int columns, double[] values, ArrayPool<double> pool) : base(rows, columns, values)
        {
            this.Pool = pool;
        }

        public void Dispose()
        {
            if (Pool != null)
            {
                Pool.Free(this.Values);
                this.Values = null;
            }
        }
    }
}
