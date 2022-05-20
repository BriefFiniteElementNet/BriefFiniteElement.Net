// -----------------------------------------------------------------------
// <copyright file="PCG.cs">
// Copyright (c) 2008 Lawrence Livermore National Security, LLC.
// Copyright (c) 2014 Christian Woltering, C# version
// </copyright>
// -----------------------------------------------------------------------

namespace BriefFiniteElementNet.Solver
{
    using System;
    using CSparse.Storage;

    using Vector = CSparse.Double.Vector;

    /// <summary>
    /// Preconditioned conjugate gradient (Orthomin) functions
    /// </summary>
    /// <remarks>
    /// From Hypre (LGPL): http://acts.nersc.gov/hypre/
    /// </remarks>
    public class PCG : IterativeSolver
    {
        double rel_residual_norm;

        public PCG()
            : this(new IdentityPreconditioner<double>())
        {
        }

        public PCG(IPreconditioner<double> M)
        {
            this.Preconditioner = M;
        }

        public override BuiltInSolverType SolverType
        {
            get { return BuiltInSolverType.ConjugateGradient; }
        }

        /// <inheritdoc />
        public override void Solve(double[] input, double[] result)
        {

            var M = this.Preconditioner;

            if (M == null || !M.IsInitialized)
            {
                throw new Exception("Invalid preconditioner state.");
                
            }

            

            int max_iter = MaxIterations;

            double rtol = RelativeTolerance;
            double atol = AbsoluteTolerance;
            double ctol = ConvergenceFactorTolerance;

            double atolf = 0.0;
            double rtol_1 = 0.0;
            bool two_norm = false;
            bool rel_change = false;
            bool recompute_residual = false;
            int recompute_residual_p = 0;
            bool stop_crit = false;
            double[] p = new double[input.Length];
            double[] s = new double[input.Length];
            double[] r = new double[input.Length];

            double alpha, beta;
            double gamma, gamma_old;
            double bi_prod, i_prod = 0.0, eps;
            double pi_prod, xi_prod;

            double i_prod_0 = 0.0;
            double cf_ave_0 = 0.0;
            double cf_ave_1 = 0.0;
            double weight;
            double ratio;

            double guard_zero_residual, sdotp;
            bool tentatively_converged = false;
            bool recompute_true_residual = false;

            int i = 0;

            // With relative change convergence test on, it is possible to attempt
            // another iteration with a zero residual. This causes the parameter
            // alpha to go NaN. The guard_zero_residual parameter is to circumvent
            // this. Perhaps it should be set to something non-zero (but small).
            guard_zero_residual = 0.0;

            // Start pcg solve

            // compute eps
            if (two_norm)
            {
                // bi_prod = <b,b>
                bi_prod = Vector.DotProduct(input, input);
            }
            else
            {
                // bi_prod = <C*b,b>
                //Vector.Clear(p);
                M.Solve(input, p);
                bi_prod = Vector.DotProduct(p, input);
            };

            eps = rtol * rtol; // note: this may be re-assigned below
            if (bi_prod > 0.0)
            {
                if (stop_crit && !rel_change && atolf <= 0)
                {  // pure absolute tolerance
                    eps = eps / bi_prod;
                    // Note: this section is obsolete.  Aside from backwards comatability
                    // concerns, we could delete the stop_crit parameter and related code,
                    // using tol & atolf instead.
                }
                else if (atolf > 0)  // mixed relative and absolute tolerance
                    bi_prod += atolf;
                else // DEFAULT (stop_crit and atolf exist for backwards compatibilty and are not in the reference manual)
                {
                    // convergence criteria:  <C*r,r>  <= max( a_tol^2, r_tol^2 * <C*b,b> )
                    //  note: default for a_tol is 0.0, so relative residual criteria is used unless
                    //  user specifies a_tol, or sets r_tol = 0.0, which means absolute
                    //  tol only is checked 
                    eps = Math.Max(rtol * rtol, atol * atol / bi_prod);

                }
            }
            else    // bi_prod==0.0: the rhs vector b is zero
            {
                // Set x equal to zero and return
                Vector.Copy(input, result);
                //message = "";
                //return SolverResult.Success;
                // In this case, for the original parcsr pcg, the code would take special
                // action to force iterations even though the exact value was known.
            }

            // r = b - Ax
            Vector.Copy(input, r);
            A.Multiply(-1.0, result, 1.0, r);

            // p = C*r
            //Vector.Clear(p);
            M.Solve(r, p);

            // gamma = <r,p>
            gamma = Vector.DotProduct(r, p);

            // Set initial residual norm
            if (ctol > 0.0)
            {
                if (two_norm)
                    i_prod_0 = Vector.DotProduct(r, r);
                else
                    i_prod_0 = gamma;
            }

            while (i < max_iter)
            {
                // the core CG calculations...
                i++;

                // At user request, periodically recompute the residual from the formula
                // r = b - A x (instead of using the recursive definition). Note that this
                // is potentially expensive and can lead to degraded convergence (since it
                // essentially a "restarted CG").
                recompute_true_residual = (recompute_residual_p > 0) && !((i % recompute_residual_p) == 0);

                // s = A*p
                A.Multiply(1.0, p, 0.0, s);

                // alpha = gamma / <s,p>
                sdotp = Vector.DotProduct(s, p);
                if (sdotp == 0.0)
                {
                    // ++ierr;
                    if (i == 1) i_prod = i_prod_0;
                    break;
                }
                alpha = gamma / sdotp;

                gamma_old = gamma;

                // x = x + alpha*p
                Vector.Axpy(alpha, p, result);

                // r = r - alpha*s
                if (!recompute_true_residual)
                {
                    Vector.Axpy(-alpha, s, r);
                }
                else
                {
                    //Recomputing the residual...
                    Vector.Copy(input, r);
                    A.Multiply(-1.0, result, 1.0, r);
                }

                // residual-based stopping criteria: ||r_new-r_old|| < rtol ||b||
                if (rtol_1 > 0 && two_norm)
                {
                    // use that r_new-r_old = alpha * s
                    double drob2 = alpha * alpha * Vector.DotProduct(s, s) / bi_prod;
                    if (drob2 < rtol_1 * rtol_1)
                    {
                        break;
                    }
                }

                // s = C*r
                Vector.Clear(s);
                M.Solve(r, s);

                // gamma = <r,s>
                gamma = Vector.DotProduct(r, s);

                // residual-based stopping criteria: ||r_new-r_old||_C < rtol ||b||_C
                if (rtol_1 > 0 && !two_norm)
                {
                    // use that ||r_new-r_old||_C^2 = (r_new ,C r_new) + (r_old, C r_old)
                    double r2ob2 = (gamma + gamma_old) / bi_prod;
                    if (r2ob2 < rtol_1 * rtol_1)
                    {
                        break;
                    }
                }

                // set i_prod for convergence test
                if (two_norm)
                    i_prod = Vector.DotProduct(r, r);
                else
                    i_prod = gamma;


                // check for convergence
                if (i_prod / bi_prod < eps)  // the basic convergence test
                    tentatively_converged = true;
                if (tentatively_converged && recompute_residual)
                // At user request, don't trust the convergence test until we've recomputed
                // the residual from scratch.  This is expensive in the usual case where an
                // the norm is the energy norm.
                // This calculation is coded on the assumption that r's accuracy is only a
                // concern for problems where CG takes many iterations.
                {
                    // r = b - Ax
                    Vector.Copy(input, r);
                    A.Multiply(-1.0, result, 1.0, r);

                    // set i_prod for convergence test
                    if (two_norm)
                        i_prod = Vector.DotProduct(r, r);
                    else
                    {
                        // s = C*r
                        Vector.Clear(s);
                        M.Solve(r, s);
                        // iprod = gamma = <r,s>
                        i_prod = Vector.DotProduct(r, s);
                    }
                    if (i_prod / bi_prod >= eps) tentatively_converged = false;
                }
                if (tentatively_converged && rel_change && (i_prod > guard_zero_residual))
                // At user request, don't treat this as converged unless x didn't change
                // much in the last iteration.
                {
                    pi_prod = Vector.DotProduct(p, p);
                    xi_prod = Vector.DotProduct(result, result);
                    ratio = alpha * alpha * pi_prod / xi_prod;
                    if (ratio >= eps) tentatively_converged = false;
                }
                if (tentatively_converged)
                {
                    // we've passed all the convergence tests, it's for real
                    break;
                }

                if ((gamma < 1.0e-292) && ((-gamma) < 1.0e-292))
                {
                    // ierr = 1, hypre_error(HYPRE_ERROR_CONV)
                    //message = "method did not converge as expected";
                    //return SolverResult.Failure;

                    throw new Exception("Numeric Solver: PCG method did not converge as expected");
                }
                // ... gamma should be >=0.  IEEE subnormal numbers are < 2**(-1022)=2.2e-308
                // (and >= 2**(-1074)=4.9e-324).  So a gamma this small means we're getting
                // dangerously close to subnormal or zero numbers (usually if gamma is small,
                // so will be other variables).  Thus further calculations risk a crash.
                // Such small gamma generally means no hope of progress anyway.

                // Optional test to see if adequate progress is being made.
                // The average convergence factor is recorded and compared
                // against the tolerance 'cf_tol'. The weighting factor is  
                // intended to pay more attention to the test when an accurate
                // estimate for average convergence factor is available.  
                if (ctol > 0.0)
                {
                    cf_ave_0 = cf_ave_1;
                    if (i_prod_0 < 1.0e-292)
                    {
                        // i_prod_0 is zero, or (almost) subnormal, yet i_prod wasn't small
                        // enough to pass the convergence test.  Therefore initial guess was good,
                        // and we're just calculating garbage - time to bail out before the
                        // next step, which will be a divide by zero (or close to it).
                        // ierr = 1, hypre_error(HYPRE_ERROR_CONV)
                        throw new Exception("Numeric Solver: PCG method did not converge as expected");
                        //message = "method did not converge as expected";
                        //return SolverResult.Failure;
                    }
                    cf_ave_1 = Math.Pow(i_prod / i_prod_0, 1.0 / (2.0 * i));

                    weight = Math.Abs(cf_ave_1 - cf_ave_0);
                    weight = weight / Math.Max(cf_ave_1, cf_ave_0);
                    weight = 1.0 - weight;

                    if (weight * cf_ave_1 > ctol) break;
                }

                // back to the core CG calculations

                // beta = gamma / gamma_old
                beta = gamma / gamma_old;

                // p = s + beta p
                if (!recompute_true_residual)
                {
                    Vector.Scale(beta, p);
                    Vector.Axpy(1.0, s, p);
                }
                else
                    Vector.Copy(s, p);
            }

            // Finish up with some outputs.

            numIterations = i;
            if (bi_prod > 0.0)
                rel_residual_norm = Math.Sqrt(i_prod / bi_prod);
            else // actually, we'll never get here...
                rel_residual_norm = 0.0;

            if (numIterations > max_iter)
                throw new Exception("Numeric Solver: Max number of {0} iterations exceed");

            //return (numIterations < max_iter) ? SolverResult.Success : SolverResult.Failure;
        }
    }
}
