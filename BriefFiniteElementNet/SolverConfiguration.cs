using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a configuration for creating appropriated <see cref="ISolver"/>
    /// </summary>
    public class SolverConfiguration
    {
        #region Constructors
        /*
        /// <summary>
        /// Initializes a new instance of the <see cref="SolverConfiguration"/> class.
        /// </summary>
        /// <param name="solverType">Type of the solver.</param>
        [Obsolete]
        public SolverConfiguration(BuiltInSolverType solverType):this()
        {
            this.solverType = solverType;
        }*/

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
        public Func<CompressedColumnStorage, ISolver> SolverGenerator
        {
            get { return _solverGenerator; }
            set { _solverGenerator = value; }
        }


        private Func<CompressedColumnStorage, ISolver> _solverGenerator;


        /*
        private LoadCase settlementsLoadCase;

        /// <summary>
        /// Gets or sets the settlements load case.
        /// </summary>
        /// <value>
        /// The load case for treating settlements.
        /// </value>
        [Obsolete]
        public LoadCase SettlementsLoadCase
        {
            get { return settlementsLoadCase; }
            set { settlementsLoadCase = value; }
        }*/




        //private BuiltInSolverType solverType;

        /*
        /// <summary>
        /// Gets or sets the type of the solver.
        /// </summary>
        /// <value>
        /// The type of the solver who should be used in solving process.
        /// </value>
        [Obsolete("Pass solver instead of using this")]
        public BuiltInSolverType SolverType
        {
            get { return solverType; }
            set { solverType = value; }
        }
        */
        


        //private ISolver _solver;

        /// <summary>
        /// Gets or sets the custom solver.
        /// </summary>
        /// <value>
        /// A custom solver to be used for solving equations.
        /// </value>
        /*
        [Obsolete("Use Solver property instead")]
        public ISolver CustomSolver
        {
            get { return _solver; }
            set { _solver = value; }
        }*/

        /*
        [Obsolete("Use SolverGenerator property instead")]
        public ISolver Solver
        {
            get { return _solver; }
            set { _solver = value; }
        }
        */
   

        #endregion
    }
}
