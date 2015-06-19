using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a string comparer which ignores case and white-space characters (Space, tab, linefeed, carriage-return, formfeed, vertical-tab and newline)
    /// </summary>
    internal class FemNetStringCompairer : IEqualityComparer<string>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(string x, string y)
        {
            return FemNetStringCompairer.IsEqual(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(string obj)
        {
            if (ReferenceEquals(obj, null))
                return 1;

            var obj2 = IsClean(obj) ? obj : Clean(obj);
            return obj2.GetHashCode();
        }

        /// <summary>
        /// Cleans the specified string. Remove all white space charcters, convert all uppercase characters to lowercase.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Cleaned string</returns>
        public static string Clean(string input)
        {
            var j = 0;
            var inputlen = input.Length;
            var newarr = new char[inputlen];

            for (int i = 0; i < inputlen; ++i)
            {
                char tmp = input[i];

                if (!char.IsWhiteSpace(tmp))
                {
                    if (char.IsUpper(tmp))
                        newarr[j] = char.ToLower(tmp);
                    else
                        newarr[j] = tmp;

                    ++j;
                }

            }

            return new String(newarr, 0, j);
        }

        /// <summary>
        /// Determines whether the specified input is clean. checks for any white space character or uppercase characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>True if specified string is clean; otherwhise, false.</returns>
        public static bool IsClean(string input)
        {
            for (var i = 0; i < input.Length; i++)
                if (char.IsUpper(input[i]) || char.IsWhiteSpace(input[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified x is equal.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static bool IsEqual(string x, string y)
        {
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;


            var y2 = IsClean(y) ? y : Clean(y);
            var x2 = IsClean(x) ? x : Clean(x);

            return x2.Equals(y2);
        }
    }
}
