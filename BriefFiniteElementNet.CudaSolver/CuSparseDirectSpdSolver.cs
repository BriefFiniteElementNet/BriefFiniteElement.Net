using BriefFiniteElementNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.CudaSolver
{
    using ManagedCuda.CudaSolve;
    using ManagedCuda;
    using ManagedCuda.CudaSparse;

    /// <summary>
    /// Represents a direct solver for SPD (Symetric Positive Definite) matrixes, based on nvidia's CUSparse
    /// </summary>
    [Obsolete("Under development")]
    public class CuSparseDirectSpdSolver : ISolver, IDisposable
    {
        public global::CSparse.Double.CompressedColumnStorage A
        { get; set; }

        public bool IsInitialized
        { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        

        public void Initialize()
        {
            var t = new CudaSolveSparse();
            var mtx = new CudaSparseMatrixDescriptor(cusparseMatrixType.Symmetric, cusparseFillMode.Upper, cusparseDiagType.NonUnit, cusparseIndexBase.Zero);

            var a = new CudaDeviceVariable<double>(A.Values.Length);
            var rowPtr = new CudaDeviceVariable<int>(A.ColumnPointers.Length);
            var colIndice = new CudaDeviceVariable<int>(A.RowIndices.Length);
            var b = new CudaDeviceVariable<double>(A.RowCount);
            var x = new CudaDeviceVariable<double>(A.RowCount);

            var res = t.Csrlsvchol(0, 0, mtx, a, rowPtr, colIndice, b, 1e-5f, 0, x);
            

            //t.Csrlsvchol()
            throw new NotImplementedException();
        }

        public void Solve(double[] b, double[] x)
        {
            throw new NotImplementedException();
        }
    }
}
