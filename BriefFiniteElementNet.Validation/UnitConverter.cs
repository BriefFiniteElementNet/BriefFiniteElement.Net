using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public static class UnitConverter
    {
        public static double Ksi2Pas(double value)
        {
            return 6894757.293178*value;
        }

        public static double Pas2Ksi(double value)
        {
            return value/6894757.293178;
        }

        public static double KipsIn2Pas(double value)
        {
            return 6894757.28*value;
        }

        public static double Pas2KipsIn(double value)
        {
            return value/6894757.293178;
        }

        public static double In2M(double value)
        {
            return value*0.0254;
        }

        public static double M2In(double value)
        {
            return value/0.0254;
        }

        public static double Kip2N(double value)
        {
            return value * 4448.2216;
        }

        public static double N2Kip(double value)
        {
            return value / 4448.2216;
        }

        public static double Lb2N(double value)
        {
            return value*4.44822162825;
        }

        public static double N2Lb(double value)
        {
            return value / 4.44822162825;
        }

        public static double Psi2Pas(double value)
        {
            return value * 6894.76;
        }

        public static double Pas2Psi(double value)
        {
            return value / 6894.76;
        }
    }
}