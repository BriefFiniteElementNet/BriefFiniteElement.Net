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
    }
}
