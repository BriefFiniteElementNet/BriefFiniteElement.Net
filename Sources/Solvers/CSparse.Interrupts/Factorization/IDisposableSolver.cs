
namespace CSparse.Factorization
{
    using System;

    public interface IDisposableSolver<T> : IDisposable, ISolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
    }
}
