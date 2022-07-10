
namespace CSparse.Complex.Solver
{
    using CSparse.Interop.MKL.Feast;
    using CSparse.Storage;
    using System.Numerics;

    public class FeastResult : FeastResult<Complex>
    {
        /// <inheritdoc />
        public FeastResult(int info, int k, int size, int loops, double error, int m, double[] e, DenseColumnMajorStorage<Complex> x, double[] r)
            : base(info, k, size, loops, error, m, e, x, r)
        {
        }
    }
}
