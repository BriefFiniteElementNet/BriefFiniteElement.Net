using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a configuration for creating appropriated <see cref="ISolver"/>
    /// </summary>
    public class SolverConfiguration
    {
        #region Constructors

        public SolverConfiguration(SolverType solverType):this()
        {
            this.solverType = solverType;
        }

        public SolverConfiguration()
        {
            this.loadCases = new List<LoadCase>();
        }

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



        private LoadCase settlementsLoadCase;

        /// <summary>
        /// Gets or sets the settlements load case.
        /// </summary>
        /// <value>
        /// The load case for treating settlements.
        /// </value>
        public LoadCase SettlementsLoadCase
        {
            get { return settlementsLoadCase; }
            set { settlementsLoadCase = value; }
        }




        private SolverType solverType;

        /// <summary>
        /// Gets or sets the type of the solver.
        /// </summary>
        /// <value>
        /// The type of the solver who should be used in solving process.
        /// </value>
        public SolverType SolverType
        {
            get { return solverType; }
            set { solverType = value; }
        }

        


        private ISolver customSolver;

        /// <summary>
        /// Gets or sets the custom solver.
        /// </summary>
        /// <value>
        /// A custom solver to be used for solving equations.
        /// </value>
        public ISolver CustomSolver
        {
            get { return customSolver; }
            set { customSolver = value; }
        }

        #endregion
    }
}
