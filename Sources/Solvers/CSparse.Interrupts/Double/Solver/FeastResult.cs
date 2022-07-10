
namespace CSparse.Double.Solver
{
    using CSparse.Interop.MKL.Feast;
    using CSparse.Storage;

    public class FeastResult : FeastResult<double>
    {
        /// <inheritdoc />
        public FeastResult(int info, int k, int size, int loops, double error, int m, double[] e, DenseColumnMajorStorage<double> x, double[] r)
            : base(info, k, size, loops, error, m, e, x, r)
        {
        }
    }
}
