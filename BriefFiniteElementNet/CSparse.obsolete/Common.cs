
using System;

namespace BriefFiniteElementNet.CSparse
{
    /// <summary>
    /// Functions to help simplify the generic code.
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// Sets the value of <c>1.0</c> for type T.
        /// </summary>
        /// <typeparam name="T">The type to return the value of 1.0 of.</typeparam>
        /// <returns>The value of <c>1.0</c> for type T.</returns>
        public static T OneOf<T>()
        {
            

            if (typeof(T) == typeof(double))
            {
                return (T)(object)1.0d;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the value of <c>0.0</c> for type T.
        /// </summary>
        /// <typeparam name="T">The type to return the value of 0.0 of.</typeparam>
        /// <returns>The value of <c>0.0</c> for type T.</returns>
        public static T ZeroOf<T>()
        {
           

            if (typeof(T) == typeof(double))
            {
                return (T)(object)0.0d;
            }

            throw new NotSupportedException();
        }
    }
}
