using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    public static class Util
    {
        private static Random rnd = new Random(0);
        public static StringBuilder sb = new StringBuilder();


        public  static Force GetRandomForce()
        {
            var buf = new Force(
                100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()),
                100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()));

            return buf;
        }


        public static ISolver CreateInternalSolver(BuiltInSolverType type, CompressedColumnStorage ccs)
        {
            switch (type)
            {
                case BuiltInSolverType.CholeskyDecomposition:
                    return new CholeskySolver(ccs);
                    break;
                case BuiltInSolverType.ConjugateGradient:
                    return new PCG(new SSOR()) { A = ccs };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

    

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static int GetFreeDofsCount(Model model)
        {
            var buf = 0;

            foreach (var nde in model.Nodes)
            {
                if (nde.Constraints.DX == DofConstraint.Released)
                    buf++;

                if (nde.Constraints.DY == DofConstraint.Released)
                    buf++;

                if (nde.Constraints.DZ == DofConstraint.Released)
                    buf++;


                if (nde.Constraints.RX == DofConstraint.Released)
                    buf++;

                if (nde.Constraints.RY == DofConstraint.Released)
                    buf++;

                if (nde.Constraints.RZ == DofConstraint.Released)
                    buf++;
            }

            return buf;
        }
    }
}
