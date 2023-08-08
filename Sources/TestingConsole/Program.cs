using BriefFiniteElementNet.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BriefFiniteElementNet.Validation.GithubIssues.Issue152.Run();
            //EulerBernouly2nodeChecker.Check2NodeShapeFunctionYDir();
            //EulerBernouly2nodeChecker.Check2NodeShapeFunctionZDir();
            //EulerBernouly2nodeChecker.CheckTrussShapeFunction();

            //TimoshenkoBeamChecker.Test2();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue158.Run();
            BriefFiniteElementNet.Validation.GithubIssues.Issue160.Run();
            //EulerBernouly2nodeChecker.TestConsoleBeam();

            Console.Write("Done");

            Console.ReadKey();
        }
    }
}
