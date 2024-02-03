using BriefFiniteElementNet.Mathh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Integration
{
    /// <summary>
    /// Stuff about calculating the integral of f.u where f is polynomial and u is heaviside (unit step function)
    /// refer to docs.
    /// </summary>
    public class StepFunctionIntegralCalculator
    {

        public IPolynomial Polynomial;


        /// <summary>
        /// Calculates the n'th integral of <see cref="Polynomial"/> at point<see cref="x0"/>
        /// refer to docs
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public double CalculateIntegralAt(double x0, int n)
        {
            if (n == 0 || n >= 5)
                throw new ArgumentException("0<n<5");



            throw new NotImplementedException();
        }

        private double F0(double x)
        {
            //return ((SingleVariablePolynomial)Polynomial).
            throw new Exception();
        }
        
    }
}
