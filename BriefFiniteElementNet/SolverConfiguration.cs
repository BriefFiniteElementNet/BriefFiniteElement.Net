using System;
using System.Collections.Generic;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a configuration for creating appropriated <see cref="ISolver"/>
    /// </summary>
    public class SolverConfiguration
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SolverConfiguration"/> class.
        /// </summary>
        public SolverConfiguration()
        {
            this.loadCases = new List<LoadCase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolverConfiguration"/> class.
        /// </summary>
        /// <param name="loadCases">The load cases.</param>
        public SolverConfiguration(params LoadCase[] loadCases)
        {
            this.loadCases = new List<LoadCase>(loadCases);
        }



        #endregion

        #region Fields and Properties

        private List<LoadCase> loadCases;

        /// <summary>
        /// Gets or sets the load cases.
        /// </summary>
        /// <value>
        /// The load cases who structure displacement should calculated for them.
        /// </value>
        public List<LoadCase> LoadCases
        {
            get { return loadCases; }
            set { loadCases = value; }
        }

        /// <summary>
        /// Gets or sets the solver generator.
        /// </summary>
        /// <value>
        /// The solver generator that generates a <see cref="ISolver"/> for every <see cref="CompressedColumnStorage"/> matrix.
        /// </value>
        [Obsolete("use SolverConfiguration.SolverFactory instead")]
        public Func<CompressedColumnStorage, ISolver> SolverGenerator
        {
            get { return _solverGenerator; }
            set { _solverGenerator = value; }
        }

        [Obsolete("use SolverConfiguration.SolverFactory instead")]
        private Func<CompressedColumnStorage, ISolver> _solverGenerator;


        /// <summary>
        /// The solver factory.
        /// </summary>
        public ISolverFactory SolverFactory;

        #endregion
    }
}
