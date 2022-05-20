using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    interface IBenchmarkCase
    {
        int Dimension { get; set; }

        BuiltInSolverType SolverType { get; set; }


        void DoTheBenchmark();

        string GetBenchmarkInfo();

        TextLogger Logger { get; set; }

        Model GetCaseModel();

    }
}
