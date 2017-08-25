using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a load combination which consists of a set of Loads and a Factor for each Load.
    /// </summary>
    [Serializable]
    public sealed class LoadCombination : Dictionary<LoadCase, double>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCombination"/> class.
        /// </summary>
        public LoadCombination()
        {
        }



        #region Deserialization Constructor

        private LoadCombination(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}
