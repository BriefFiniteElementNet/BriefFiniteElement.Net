namespace BriefFiniteElementNet.Mathh
{
    /// <summary>
    /// Represents an internface for single or multy-variable polynomial
    /// it is a function with one output and one or multiple inputs
    /// </summary>
    /// <remarks>
    /// Example of multy variable polynomials are 'y*x^2 + 2 y − 4x + 7'
    /// </remarks>
    public interface IPolynomial
    {
        /// <summary>
        /// Evaluates the value of polynomial at specified <see cref="x"/>
        /// </summary>
        /// <param name="v">evaluation point coordinates</param>
        /// <returns>P(<see cref="x"/>)</returns>
        double Evaluate(params double[] v);

        /// <summary>
        /// Evaluates the <see cref="n"/>'th derivation of current polynomial at specified <see cref="x"/>.
        /// </summary>
        /// <param name="n">derivation degree</param>
        /// <param name="v">evaluation location</param>
        /// <returns>evaluation of derivation</returns>
        /// <remarks>
        /// buf[0,i] = Rond(F)^n/Rond(v[i]^n)
        /// </remarks>
        double[] EvaluateNthDerivative(int n, params double[] v);

        /// <summary>
        /// Gets the derivative of polynomial as another polynomial
        /// </summary>
        /// <param name="deg">derivation degree</param>
        /// <returns>derivation</returns>
        /// <remarks>
        /// buf[0,i] = Rond(F)/Rond(v[i])
        /// </remarks>
        double[] EvaluateDerivative(params double[] v);

        /// <summary>
        /// The number of variables
        /// </summary>
        /// <remarks>
        /// for example two for 'y*x^2 + 2 y − 4x + 7' and one for 'x^3+2x+1'
        /// </remarks>
        int VariableCount { get; }

        /// <summary>
        /// Gets the max degree of polynomial
        /// </summary>
        /// <remarks>
        /// example for 'y*x^2 + 2 y − 4x + 7' is y degree is 1, and x degree is 2
        /// </remarks>
        int[] Degree { get; }
    }
}
