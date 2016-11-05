using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the possible behaviors of Bar Element
    /// </summary>
    [Flags]
    public enum BarElementBehaviour
    {
        /// <summary>
        /// The truss behavior
        /// </summary>
        Truss = 1,
        /// <summary>
        /// The beam in local Y direction behavior, following Euler-Bernoulli theory
        /// </summary>
        BeamYEulerBernoulli = 2,
        /// <summary>
        /// The beam in local Y direction behavior, following Timoshenko theory
        /// </summary>
        BeamYTimoshenko = 4,
        /// <summary>
        /// The beam in local Y direction behavior, following Euler-Bernoulli theory
        /// </summary>
        BeamZEulerBernoulli = 8,
        /// <summary>
        /// The beam in local Z direction behavior, following Timoshenko theory
        /// </summary>
        BeamZTimoshenko = 16,
        /// <summary>
        /// The shaft (only torsion), normal
        /// </summary>
        Shaft = 32,
        /// <summary>
        /// The shaft (only torsion), following timoshenko theory for thick bars
        /// </summary>
        ThickShaft = 64,
    }
}
