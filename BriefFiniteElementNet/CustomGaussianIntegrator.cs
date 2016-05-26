using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a very helpful class for integrating 3d function G as noted below.
    /// The integration problem can be represented as:
    ///                                                                                          
    ///                                                                                          
    ///                                                                                          
    ///        / α₂   / f₂(γ)   / g₁(η,γ)                                                                             
    ///       |      |         |                                                                   
    ///  I =  |      |         |       G( ξ,η,γ ) . dξ . dη . dγ                                                             
    ///       |      |         |                                                                      
    ///      / α₁   / f₁(γ)   / g₁(η,γ)                                                                                 
    ///
    /// this class will get all parameters above and compute the I...
    /// TODO: probably poor performance, improve performance...
    /// 
    /// </summary>
    /// <remarks>
    /// baghie document poshte safe 14 thesisam!
    /// </remarks>
    [Obsolete("use GaussianIntegrator instead")]
    public class CustomGaussianIntegrator
    {
        public Func<double, double, double, Matrix> G;//G parameter order: nu,gama,xi

        public Func<double, double, double> g2, g1;//g2,g1 parameter order: nu,gama

        public Func<double, double> f2, f1;// f2,f1 parameter order: gama

        public double a2, a1;

        //public List<double[]> eval_points ;//eval_points[i][0] : value, eval_points[i][1] : weight
        public double[] v;//Gaussian values
        public double[] w;//Gaussian weights


        public int H, W;//Height and Width of G matrix



        /// <summary>
        /// Computes the I.
        /// </summary>
        /// <returns>The I</returns>
        public Matrix Integrate()
        {
            if (v.Length != w.Length)
                throw new Exception();

            var I = new Matrix(H, W);

            for (var k = 0; k < v.Length; k++)
            {
                var gammaK = (a2 - a1)/2*v[k] + (a2 + a1)/2;
                var phi = new Matrix(H, W);

                for (var j = 0; j < v.Length; j++)
                {
                    var noj = (f2(gammaK) - f1(gammaK))/2*v[j] + (f2(gammaK) + f1(gammaK))/2;
                    var beta = new Matrix(H, W); 

                    for (var i = 0; i < v.Length; i++)
                    {
                        var xii = (g2(noj, gammaK) - g1(noj, gammaK))/2*v[i] + (g2(noj, gammaK) + g1(noj, gammaK))/2;

                        beta += (g2(noj, gammaK) - g1(noj, gammaK))/2*w[i]*G(noj, gammaK, xii);
                    }

                    phi += (f2(gammaK) - f1(gammaK))/2*w[j]*beta;
                }

                I += (a2 - a1)/2*w[k]*phi;
            }


            return I;
        }
    }
}
