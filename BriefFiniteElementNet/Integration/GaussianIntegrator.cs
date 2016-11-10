using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Integration
{
    /// <summary>
    /// Represents a very helpful class for integrating 3d function G as noted below.
    /// The integration problem can be represented as:
    ///                                                                                          
    ///                                                                                          
    ///                                                                                          
    ///        / A₂   / F₂(γ)   / G₂(η,γ)                                                                             
    ///       |      |         |                                                                   
    ///  I =  |      |         |       H( ξ,η,γ ) . dξ . dη . dγ                                                             
    ///       |      |         |                                                                      
    ///      / A₁   / F₁(γ)   / G₁(η,γ)                                                                                 
    ///
    /// where:
    ///     A₁,A₂: numbers
    ///     F₁(γ), F₂(γ): functions with one input and one output (all double)
    ///     G₁(η,γ), G₂(η,γ) : functions with two input and one output (all double)
    /// 
    /// this class will get all parameters above and compute the I...
    /// </summary>
    /// <remarks>
    /// This can integrate in either 2D (ξ-η) and 1D (ξ). see documentation for how...</remarks>
    public class GaussianIntegrator
    {

        /// <summary>
        /// The Gaussian point count for integrating in ξ dimension.
        /// </summary>
        public int XiPointCount;

        /// <summary>
        /// The Gaussian point count for integrating in η dimension.
        /// </summary>
        public int EtaPointCount;

        /// <summary>
        /// The Gaussian point count for integrating in γ dimension.
        /// </summary>
        public int GammaPointCount;


        /// <summary>
        /// The G2 function, parameters: G2(η,γ)
        /// </summary>
        public Func<double, double, double> G2;

        /// <summary>
        /// The G1 function, parameters: G1(η,γ)
        /// </summary>
        public Func<double, double, double> G1;


        /// <summary>
        /// The F1 function, parameters: F1(γ)
        /// </summary>
        /// <value>
        /// lower level of η integration
        /// </value>
        public Func<double, double> F1;

        /// <summary>
        /// The F2 function, parameters: F2(γ)
        /// </summary>
        /// <value>
        /// upper level of η integration
        /// </value>
        public Func<double, double> F2;


        /// <summary>
        /// The A1
        /// </summary>
        /// <value>
        /// lower level of Gamma integration
        /// </value>
        public double A1;

        /// <summary>
        /// The A2
        /// </summary>
        /// <value>
        /// upper level of Gamma integration
        /// </value>
        public double A2;


        /// <summary>
        /// The H function to be integrated
        /// </summary>
        public IMatrixFunction H;

        /// <summary>
        /// Computes the I.
        /// </summary>
        /// <returns>The I</returns>
        public Matrix Integrate()
        {
            if (XiPointCount < 1 || EtaPointCount < 1 || GammaPointCount < 1)
                throw new NotSupportedException();

            double a1 = A1, a2 = A2;

            var f1 = F1;
            var f2 = F2;

            var g1 = G1;
            var g2 = G2;

            var vk = GaussPoints.GetGaussianValues(GammaPointCount);
            var wk = GaussPoints.GetGaussianWeights(GammaPointCount);

            var vj = GaussPoints.GetGaussianValues(EtaPointCount);
            var wj = GaussPoints.GetGaussianWeights(EtaPointCount);

            var vi = GaussPoints.GetGaussianValues(XiPointCount);
            var wi = GaussPoints.GetGaussianWeights(XiPointCount);

            Matrix I = null;//we do not know dimensions yet!

            for (var k = 0; k < GammaPointCount; k++)
            {
                var gammaK = (a2 - a1)/2*vk[k] + (a2 + a1)/2;
                Matrix phi = null;// = new Matrix(H, W);

                for (var j = 0; j < EtaPointCount; j++)
                {
                    var noj = (f2(gammaK) - f1(gammaK))/2*vj[j] + (f2(gammaK) + f1(gammaK))/2;
                    Matrix beta = null;//= new Matrix(H, W); 

                    for (var i = 0; i < XiPointCount; i++)
                    {
                        var xii = (g2(noj, gammaK) - g1(noj, gammaK))/2*vi[i] + (g2(noj, gammaK) + g1(noj, gammaK))/2;

                        var val = H.GetMatrix(xii, noj, gammaK);

                        //initiate I, phi and beta
                        if (beta == null)
                            beta = new Matrix(val.RowCount, val.ColumnCount);

                        if (phi == null)
                            phi = new Matrix(val.RowCount, val.ColumnCount);

                        if (I == null)
                            I = new Matrix(val.RowCount, val.ColumnCount);


                        val.MultiplyByConstant((g2(noj, gammaK) - g1(noj, gammaK)) / 2 * wi[i]);
                        beta += val;
                    }

                    phi += (f2(gammaK) - f1(gammaK))/2*wj[j]*beta;
                }

                I += (a2 - a1)/2*wk[k]*phi;
            }

            return I;
        }


        public static GaussianIntegrator CreateFor1DProblem(Func<double, double> function, double x0, double x1,
            int sampling)
        {
            var buf = new GaussianIntegrator();

            buf.A1 = x0;
            buf.A2 = x1;

            buf.F1 = gama => 0;
            buf.F2 = gama => 1;

            buf.G1 = (eta, gama) => 0;
            buf.G2 = (eta, gama) => 1;

            buf.GammaPointCount = sampling;
            buf.XiPointCount = buf.EtaPointCount = 1;

            buf.H = new FunctionMatrixFunction((xi, eta, gama) => new Matrix(new[] {function(gama)}));

            return buf;
        }
    }
}
