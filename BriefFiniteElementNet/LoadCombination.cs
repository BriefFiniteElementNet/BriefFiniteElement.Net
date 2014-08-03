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

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCombination"/> class.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        private LoadCombination(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
    }
}
