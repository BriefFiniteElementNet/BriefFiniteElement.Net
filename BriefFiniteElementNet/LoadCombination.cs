using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a load combination which consists of a set of Loads and a Factor for each Load.
    /// </summary>
    public class LoadCombination : Dictionary<LoadCase, double>
    {
        /// <summary>
        /// Gets a load combination which have load factor of 1.0 for <see cref="LoadCase.DefaultLoadCase"/> 
        /// </summary>
        /// <value>
        /// The default load combination.
        /// </value>
        public static LoadCombination DefaultLoadCombination
        {
            get
            {
                var buf = new LoadCombination();
                buf[LoadCase.DefaultLoadCase] = 1.0;
                return buf;
            }
        }
    }
}
