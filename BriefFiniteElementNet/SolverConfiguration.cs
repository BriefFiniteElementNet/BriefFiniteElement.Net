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

        public SolverConfiguration(List<LoadCase> loadCases, ISolverFactory solverFactory)
        {
            this.loadCases = loadCases;
            SolverFactory = solverFactory;
        }


        public SolverConfiguration(ISolverFactory solverFactory)
        {
            SolverFactory = solverFactory;
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
            private set { loadCases = value; }
        }
      /**/
        [Obsolete("use SolverConfiguration.SolverFactory instead")]
        //private Func<SparseMatrix, ISolver> _solverGenerator;
       
        private ISolverFactory solverFactory;

        /// <summary>
        /// The solver factory.
        /// </summary>
        public ISolverFactory SolverFactory
        {
            get { return solverFactory; }
            set
            {
                solverFactory = value;
            }
        }/**/

        #endregion
    }
}
