using BriefFiniteElementNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.CudaSolver
{


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
            //Alea.CudaToolkit.CuSparse
            //var t = new CudaSolveSparse();
            /*
            var a = new CudaDeviceVariable<double>(A.Values.Length);
            var rowPtr = new CudaDeviceVariable<int>(A.ColumnPointers.Length);
            var colIndice = new CudaDeviceVariable<int>(A.RowIndices.Length);
            var b = new CudaDeviceVariable<double>(A.RowCount);
            var x = new CudaDeviceVariable<double>(A.RowCount);

            var mtx = new CudaSparseMatrixDescriptor(cusparseMatrixType.Symmetric, cusparseFillMode.Upper, cusparseDiagType.NonUnit, cusparseIndexBase.Zero);


            var res = new CudaUtils().Csrlsvchol(A.ColumnCount, A.NonZerosCount, mtx, a, rowPtr, colIndice, b, 1e-5f, 0, x);

            /*
            int singularity = 0;
            this.res = CudaSolveNativeMethods.Sparse.cusolverSpCcsrlsvchol(t._handle, m, nnz, descrA.Descriptor, csrValA.DevicePointer, csrRowPtrA.DevicePointer, csrColIndA.DevicePointer, b.DevicePointer, tol, reorder, x.DevicePointer, ref singularity);
            if (this.res != cusolverStatus.Success)
                throw new CudaSolveException(this.res);
            else
                return singularity;
            */
            //cusolver64_80.dll
            //t.Csrlsvchol()
            throw new NotImplementedException();
        }

        public void Solve(double[] b, double[] x)
        {
            throw new NotImplementedException();
        }
    }
}
