using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Defines a generalized method that a object consist of numeric objects create a type-specific method for determining fuzzy equality of instances
    /// Usable for eliminating round off errors.
    /// </summary>
    /// <typeparam name="T">The type of the object to compare</typeparam>
    public interface IFuzzyEquatable<T>
    {
        /// <summary>
        /// Determines that does the object equals this object with regard to <see cref="Threshold" /> </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        bool FuzzyEquals(Vector other);

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>
        /// The threshold for compairing the numbers.
        /// </value>
        double Threshold { get; set; }
    }
}
