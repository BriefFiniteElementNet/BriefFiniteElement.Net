using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Controls;
//using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;
using Examples.CSharp;


namespace BriefFiniteElementNet.CodeProjectExamples
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            SimpleTruss3D.Run();
            //PartiNonUniformLoadExamples.test1();
            //InternalForceExample.Run();
            //new BarIncliendFrameExample().Run();
            //new UniformLoadCoordSystem().run();
            //DocSnippets.Test2();
            //new UniformLoadCoordSystem().run();
            //Example1();
            //Example2();
            //DocSnippets.Test1();
            //TestMatrixMult();
        }

      
    }
}
