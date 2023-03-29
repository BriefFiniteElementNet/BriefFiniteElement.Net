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
            EulerBernouly2nodeChecker.Check2NodeShapeFunctionYDir();
            EulerBernouly2nodeChecker.Check2NodeShapeFunctionZDir();

            Console.Write("Done");
        }
    }
}
