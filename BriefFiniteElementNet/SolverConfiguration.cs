using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public class SolverConfiguration
    {
        #region Constructors

        public SolverConfiguration()
        {
            this.loadCases = new List<LoadCase>();
        }

        public SolverConfiguration(params LoadCase[] loadCases)
        {
            this.loadCases = new List<LoadCase>(loadCases);
        }

        public SolverConfiguration(LoadCase settlementsLoadCase):this()
        {
            this.settlementsLoadCase = settlementsLoadCase;
        }

        public SolverConfiguration(LoadCase settlementsLoadCase, params LoadCase[] loadCases)
        {
            this.loadCases = new List<LoadCase>(loadCases);
            this.settlementsLoadCase = settlementsLoadCase;
        }

        #endregion


        private List<LoadCase> loadCases;

        public List<LoadCase> LoadCases
        {
            get { return loadCases; }
            set { loadCases = value; }
        }



        public LoadCase SettlementsLoadCase
        {
            get { return settlementsLoadCase; }
            set { settlementsLoadCase = value; }
        }


        private LoadCase settlementsLoadCase;
    }
}
