using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var g = new BarGrid();

            g.Run(16);

            return;

            for (int i = 2;i < 20; i++)
            {
                g.Run(i);
            }
            

            Console.ReadKey();
        }
    }
}
