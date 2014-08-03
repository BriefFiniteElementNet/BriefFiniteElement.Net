// -----------------------------------------------------------------------
// <copyright file="SparseCholesky.cs">
// Original CSparse code by Tim Davis, http://www.cise.ufl.edu/research/sparse/CXSparse/
// CSparse.NET code by Christian Woltering, http://csparse.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using BriefFiniteElementNet.CSparse.Ordering;
using BriefFiniteElementNet.CSparse.Storage;

namespace BriefFiniteElementNet.CSparse.Double.Factorization
{
    /// <summary>
    /// Sparse Cholesky decomposition.
    /// </summary>
    /// <remarks>
    /// See Chapter 4 (Cholesky factorization) in "Direct Methods for Sparse Linear Systems"
    /// by Tim Davis.
    /// </remarks>
    [Serializable]
    public class SparseCholesky : ISparseFactorization<double>
    {
        SymbolicFactorization symFactor;
        CompressedColumnStorage<double> L;
        int n;

        /// <summary>
        /// Creates a sparse Cholesky factorization.
        /// </summary>
        /// <param name="order">Ordering method to use (natural or A+A').</param>
        /// <param name="A">Column-compressed matrix, symmetric positive definite.</param>
        /// <remarks>
        /// Only upper triangular part is used.
        /// </remarks>
        public SparseCholesky(CompressedColumnStorage<double> A, ColumnOrdering order)
        {
            if ((int)order > 1) // AtA ordering not allowed
            {
                throw new ArgumentException("order");
            }

            this.n = A.ColumnCount;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            // Ordering and symbolic analysis
            SymbolicAnalysis(order, A);
            sp.Stop();
            TraceUtil.WritePerformanceTrace("symbolic factorization tooks: {0}", sp.Elapsed);
            // Numeric Cholesky factorization
            sp.Restart();
            Factorize(A);

            TraceUtil.WritePerformanceTrace("factorization tooks: {0}", sp.Elapsed);
        }

        /// <summary>
        /// Gets the number of nonzeros of the L.
        /// </summary>
        public int NonZerosCount
        {
            get { return L.NonZerosCount; }
        }

        /// <summary>
        /// Solves a linear system Ax=b, where A is symmetric positive definite.
        /// </summary>
        /// <param name="input">Right hand side b.</param>
        /// <param name="result">Solution vector x.</param>
        public void Solve(double[] input, double[] result)
        {
            Vector.Copy(input, result);

            // TODO: remove LinearSolve method with one argument
            // thus eleminating additional vector allocation. 
            Solve(result);
        }

        // TODO: implement SolveTranspose

        /// <summary>
        /// Solves a linear system Ax=b, where A is symmetric positive definite.
        /// </summary>
        /// <param name="b">Right hand side, overwritten with solution</param>
        public void Solve(double[] b)
        {
            if (b == null) throw new ArgumentNullException("b");

            double[] x = new double[n];

            Permutation.ApplyInverse(symFactor.pinv, b, x, n); // x = P*b

            var lx = L.Values;
            var lp = L.ColumnPointers;
            var li = L.RowIndices;

            // LinearSolve lower triangular system by forward elimination, x = L\x.
            for (int i = 0; i < n; i++)
            {
                x[i] /= lx[lp[i]];
                for (int p = lp[i] + 1; p < lp[i + 1]; p++)
                {
                    x[li[p]] -= lx[p] * x[i];
                }
            }

            // LinearSolve upper triangular system by backward elimination, x = L'\x.
            for (int i = n - 1; i >= 0; i--)
            {
                for (int p = lp[i] + 1; p < lp[i + 1]; p++)
                {
                    x[i] -= lx[p] * x[li[p]];
                }
                x[i] /= lx[lp[i]];
            }

            Permutation.Apply(symFactor.pinv, x, b, n); // b = P'*x
        }

        /// <summary>
        /// Sparse Cholesky update, L*L' + w*w'
        /// </summary>
        /// <param name="w">The update matrix.</param>
        /// <returns>False, if updated matrix is not positive definite, otherwise true.</returns>
        public bool Update(CompressedColumnStorage<double> w)
        {
            return UpDown(1, w);
        }

        /// <summary>
        /// Sparse Cholesky downdate, L*L' - w*w'
        /// </summary>
        /// <param name="w">The update matrix.</param>
        /// <returns>False, if updated matrix is not positive definite, otherwise true.</returns>
        public bool Downdate(CompressedColumnStorage<double> w)
        {
            return UpDown(-1, w);
        }

        /// <summary>
        /// Sparse Cholesky update/downdate, L*L' + sigma*w*w' 
        /// </summary>
        /// <param name="sigma">1 = update or -1 = downdate</param>
        /// <param name="w">The update matrix.</param>
        /// <returns>False, if updated matrix is not positive definite, otherwise true.</returns>
        private bool UpDown(int sigma, CompressedColumnStorage<double> w)
        {
            int n, p, f, j;
            double alpha, gamma, w1, w2;
            double beta = 1, beta2 = 1, delta;

            var parent = symFactor.parent;

            if (parent == null)
            {
                return false;
            }

            var lp = L.ColumnPointers;
            var li = L.RowIndices;
            var lx = L.Values;

            var cp = w.ColumnPointers;
            var ci = w.RowIndices;
            var cx = w.Values;

            n = L.ColumnCount;

            if ((p = cp[0]) >= cp[1])
            {
                return true; // return if C empty
            }

            var work = new double[n]; // get workspace

            f = ci[p];
            for (; p < cp[1]; p++)
            {
                // f = min (find (C))
                f = Math.Min(f, ci[p]);
            }

            for (p = cp[0]; p < cp[1]; p++)
            {
                work[ci[p]] = cx[p];
            }

            // Walk path f up to root.
            for (j = f; j != -1; j = parent[j])
            {
                p = lp[j];
                alpha = work[j] / lx[p]; // alpha = w(j) / L(j,j)
                beta2 = beta * beta + sigma * alpha * alpha;

                if (beta2 <= 0) break; // TODO: not positive definite, throw?

                beta2 = Math.Sqrt(beta2);
                delta = (sigma > 0) ? (beta / beta2) : (beta2 / beta);
                gamma = sigma * alpha / (beta2 * beta);
                lx[p] = delta * lx[p] + ((sigma > 0) ? (gamma * work[j]) : 0);
                beta = beta2;

                for (p++; p < lp[j + 1]; p++)
                {
                    w1 = work[li[p]];
                    work[li[p]] = w2 = w1 - alpha * lx[p];
                    lx[p] = delta * lx[p] + gamma * ((sigma > 0) ? w1 : w2);
                }
            }

            return (beta2 > 0);
        }

        /// <summary>
        /// Compute the Numeric Cholesky factorization, L = chol (A, [pinv parent cp]).
        /// </summary>
        /// <returns>Numeric Cholesky factorization</returns>
        private void Factorize(CompressedColumnStorage<double> A)
        {
            double d, lki;
            int top, i, p, k;

            int n = A.ColumnCount;

            // Allocate workspace.
            var c = new int[n];
            var s = new int[n];
            var x = new double[n];

            var colp = symFactor.cp;
            var pinv = symFactor.pinv;
            var parent = symFactor.parent;

            var C = pinv != null ? PermuteSym(A, pinv, true) : A;

            var cp = C.ColumnPointers;
            var ci = C.RowIndices;
            var cx = C.Values;

            this.L = CompressedColumnStorage<double>.Create(n, n, colp[n]);

            var lp = L.ColumnPointers;
            var li = L.RowIndices;
            var lx = L.Values;

            var lst = new List<int>();

            for (k = 0; k < n; k++)
            {
                lp[k] = c[k] = colp[k];
            }
            for (k = 0; k < n; k++) // compute L(k,:) for L*L' = C
            {
                // Find nonzero pattern of L(k,:)
                top = GraphHelper.EtreeReach(SymbolicColumnStorage.Create(C, false), k, parent, s, c);
                x[k] = 0;                           // x (0:k) is now zero

                var tmp = cp[k + 1];
                for (p = cp[k]; p < tmp; p++) // x = full(triu(C(:,k)))
                {
                    if (ci[p] <= k) x[ci[p]] = cx[p];
                }
                d = x[k]; // d = C(k,k)
                x[k] = 0; // clear x for k+1st iteration

               

                // Triangular solve
                for (; top < n; top++) // solve L(0:k-1,0:k-1) * x = C(:,k)
                {
                    i = s[top];  // s [top..n-1] is pattern of L(k,:)
                    lki = x[i] / lx[lp[i]]; // L(k,i) = x (i) / L(i,i)
                    x[i] = 0;               // clear x for k+1st iteration


                    var cci = c[i];
                    for (p = lp[i] + 1; p < cci; p++)
                    {
                        x[li[p]] -= lx[p] * lki;
                    }

                    
                    

                    d -= lki * lki; // d = d - L(k,i)*L(k,i)
                    p = c[i]++;
                    li[p] = k; // store L(k,i) in column i
                    lx[p] = lki;
                }
                // Compute L(k,k)
                if (d <= 0)
                {
                    throw new Exception("not pos def"); // TODO: ex
                }

                p = c[k]++;
                li[p] = k; // store L(k,k) = sqrt (d) in column k
                lx[p] = Math.Sqrt(d);
            }

            lp[n] = colp[n]; // finalize L
        }

        /// <summary>
        /// Ordering and symbolic analysis for a Cholesky factorization
        /// </summary>
        /// <param name="order">Column ordering.</param>
        /// <param name="A">Matrix to factorize.</param>
        private void SymbolicAnalysis(ColumnOrdering order, CompressedColumnStorage<double> A)
        {
            int n = A.ColumnCount;

            var sym = this.symFactor = new SymbolicFactorization();

            // P = amd(A+A') or natural
            var p = AMD.Generate(A, order);

            // Find inverse permutation.
            sym.pinv = Permutation.Invert(p);

            // C = spones(triu(A(P,P)))
            var C = PermuteSym(A, sym.pinv, false);

            // Find etree of C.
            sym.parent = GraphHelper.EliminationTree(n, n, C.ColumnPointers, C.RowIndices, false);
            
            // Postorder the etree.
            var post = GraphHelper.TreePostorder(sym.parent, n);

            // Find column counts of chol(C)
            var c = GraphHelper.ColumnCounts(SymbolicColumnStorage.Create(C, false), sym.parent, post, false);

            sym.cp = new int[n + 1];

            // Find column pointers for L
            sym.unz = sym.lnz = Helper.CumulativeSum(sym.cp, c, n);
        }

        /// <summary>
        /// Permutes a symmetric sparse matrix. C = PAP' where A and C are symmetric.
        /// </summary>
        /// <param name="A">column-compressed matrix (only upper triangular part is used)</param>
        /// <param name="pinv">size n, inverse permutation</param>
        /// <param name="values">allocate pattern only if false, values and pattern otherwise</param>
        /// <returns>Permuted matrix, C = PAP'</returns>
        private CompressedColumnStorage<double> PermuteSym(CompressedColumnStorage<double> A,
            int[] pinv, bool values)
        {
            int i, j, p, q, i2, j2;

            int n = A.ColumnCount;

            var ap = A.ColumnPointers;
            var ai = A.RowIndices;
            var ax = A.Values;

            values = values && (ax != null);

            var result = A.Clone(values);

            var cp = result.ColumnPointers;
            var ci = result.RowIndices;
            var cx = result.Values;

            int[] w = new int[n]; // get workspace

            for (j = 0; j < n; j++) // count entries in each column of C
            {
                j2 = pinv != null ? pinv[j] : j; // column j of A is column j2 of C
                for (p = ap[j]; p < ap[j + 1]; p++)
                {
                    i = ai[p];
                    if (i > j) continue; // skip lower triangular part of A
                    i2 = pinv != null ? pinv[i] : i; // row i of A is row i2 of C
                    w[Math.Max(i2, j2)]++; // column count of C
                }
            }

            Helper.CumulativeSum(cp, w, n); // compute column pointers of C

            for (j = 0; j < n; j++)
            {
                j2 = pinv != null ? pinv[j] : j; // column j of A is column j2 of C
                for (p = ap[j]; p < ap[j + 1]; p++)
                {
                    i = ai[p];
                    if (i > j) continue; // skip lower triangular part of A
                    i2 = pinv != null ? pinv[i] : i; // row i of A is row i2 of C
                    ci[q = w[Math.Max(i2, j2)]++] = Math.Min(i2, j2);
                    if (cx != null)
                    {
                        cx[q] = ax[p];
                    }
                }
            }

            return result;
        }
    }
}
