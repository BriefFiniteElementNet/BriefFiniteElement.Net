using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.IntelMklSolver
{
    /// <summary>
    /// Represents a direct solver for SPD (Symetric Positive Definite) matrixes, based on intel's Math Kernel Library (MKL)
    /// </summary>
    public class MklDirectPosdefSolver : ISolver
    {
        public CompressedColumnStorage A { get; set; }

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        private readonly bool _isInitialized;


        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Solve(double[] b, double[] x)
        {
            throw new NotImplementedException();
        }
    }
}