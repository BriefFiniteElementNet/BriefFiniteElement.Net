// -----------------------------------------------------------------------
// <copyright file="SparseLU.cs">
// Original CSparse code by Tim Davis, http://www.cise.ufl.edu/research/sparse/CXSparse/
// CSparse.NET code by Christian Woltering, http://csparse.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace CSparse.Double.Factorization
{
    using System;
    using CSparse.Ordering;
    using CSparse.Storage;

    /// <summary>
    /// Sparse LU decomposition.
    /// </summary>
    /// <remarks>
    /// See Chapter 6 (LU factorization) in "Direct Methods for Sparse Linear Systems"
    /// by Tim Davis.
    /// </remarks>
    public class SparseLU : ISparseFactorization<double>
    {
        SymbolicFactorization symFactor;
        public CompressedColumnStorage<double> L, U;
        int[] pinv; // partial pivoting
        int n;

        /// <summary>
        /// Creates a LU factorization.
        /// </summary>
        /// <param name="A">Column-compressed matrix.</param>
        /// <param name="order">Ordering method to use.</param>
        /// <param name="tol">Partial pivoting tolerance (form 0.0 to 1.0).</param>
        public SparseLU(CompressedColumnStorage<double> A, ColumnOrdering order, double tol)
        {
            // TODO: check tol >= 0 && tol <= 1

            this.n = A.ColumnCount;

            // Ordering and symbolic analysis
            SymbolicAnalysis(order, A);

            // Numeric LU factorization
            Factorize(A, tol);
        }

        //public void GetStatistics(out int nnzL, out int nnzU, out long memory)
        //{
        //    memory = (symFactor != null) ? symFactor.GetMemory() : 0;

        //    int isize = Constants.SizeOfInt;
        //    int dsize = Constants.SizeOfDouble;

        //    nnzL = L.NonZerosCount;
        //    nnzU = U.NonZerosCount;

        //    memory += L.ColumnCount * isize;
        //    memory += nnzL * isize;
        //    memory += nnzL * dsize;

        //    memory += U.ColumnCount * isize;
        //    memory += nnzU * isize;
        //    memory += nnzU * dsize;

        //    memory += pinv.Length * isize;
        //}

        /// <summary>
        /// Gets the number of nonzeros in both L and U factors together.
        /// </summary>
        public int NonZerosCount
        {
            get { return (L.NonZerosCount + U.NonZerosCount - n); }
        }

        /// <summary>
        /// Solves a linear system Ax=b, where A is square and nonsingular.
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
        /// Solves a linear system Ax=b, where A is square and nonsingular.
        /// </summary>
        /// <param name="b">Right hand side, overwritten with solution</param>
        public void Solve(double[] b)
        {
            if (b == null) throw new ArgumentNullException("b");

            var x = new double[n];

            Permutation.ApplyInverse(pinv, b, x, n); // x = b(p)

            // LinearSolve lower triangular system by forward elimination, x = L\x.
            var lux = L.Values;
            var lup = L.ColumnPointers;
            var lui = L.RowIndices;

            for (int i = 0; i < n; i++)
            {
                x[i] /= lux[lup[i]];
                for (int p = lup[i] + 1; p < lup[i + 1]; p++)
                {
                    x[lui[p]] -= lux[p] * x[i];
                }
            }

            // LinearSolve upper triangular system by backward elimination, x = U\x.
            lux = U.Values;
            lup = U.ColumnPointers;
            lui = U.RowIndices;

            for (int i = n - 1; i >= 0; i--)
            {
                x[i] /= lux[lup[i + 1] - 1];
                for (int p = lup[i]; p < lup[i + 1] - 1; p++)
                {
                    x[lui[p]] -= lux[p] * x[i];
                }
            }

            Permutation.ApplyInverse(symFactor.q, x, b, n); // b(q) = x
        }
        
        /// <summary>
        /// [L,U,pinv] = lu(A, [q lnz unz]). lnz and unz can be guess.
        /// </summary>
        private void Factorize(CompressedColumnStorage<double> A, double tol)
        {
            int[] q = symFactor.q;

            int i;
            int lnz = symFactor.lnz;
            int unz = symFactor.unz;

            this.L = CompressedColumnStorage<double>.Create(n, n, lnz);
            this.U = CompressedColumnStorage<double>.Create(n, n, unz);
            this.pinv = new int[n];

            // Workspace
            double[] x = new double[n];
            int[] xi = new int[2 * n];

            for (i = 0; i < n; i++)
            {
                // No rows pivotal yet.
                pinv[i] = -1;
            }

            lnz = unz = 0;

            int ipiv, top, p, col;
            double pivot;
            double a, t;

            int[] li, ui;
            int[] lp = L.ColumnPointers;
            int[] up = U.ColumnPointers;
            double[] lx, ux;

            // Now compute L(:,k) and U(:,k)
            for (int k = 0; k < n; k++)
            {
                // Triangular solve
                lp[k] = lnz; // L(:,k) starts here
                up[k] = unz; // U(:,k) starts here
                if ((lnz + n > L.Values.Length && !L.Resize(2 * L.Values.Length + n)) ||
                    (unz + n > U.Values.Length && !U.Resize(2 * U.Values.Length + n)))
                {
                    throw new OutOfMemoryException();
                }

                li = L.RowIndices;
                ui = U.RowIndices;
                lx = L.Values;
                ux = U.Values;
                col = q != null ? (q[k]) : k;
                top = SolveSp(L, A, col, xi, x, pinv, true);  // x = L\A(:,col)

                // Find pivot
                ipiv = -1;
                a = -1;
                for (p = top; p < n; p++)
                {
                    i = xi[p]; // x(i) is nonzero
                    if (pinv[i] < 0) // Row i is not yet pivotal
                    {
                        if ((t = Math.Abs(x[i])) > a)
                        {
                            a = t; // Largest pivot candidate so far
                            ipiv = i;
                        }
                    }
                    else // x(i) is the entry U(pinv[i],k)
                    {
                        ui[unz] = pinv[i];
                        ux[unz++] = x[i];
                    }
                }

                if (ipiv == -1 || a <= 0.0)
                {
                    throw new Exception();
                }

                if (pinv[col] < 0 && Math.Abs(x[col]) >= a * tol)
                {
                    ipiv = col;
                }

                // Divide by pivot
                pivot = x[ipiv]; // the chosen pivot
                ui[unz] = k; // last entry in U(:,k) is U(k,k)
                ux[unz++] = pivot;
                pinv[ipiv] = k; // ipiv is the kth pivot row
                li[lnz] = ipiv; // first entry in L(:,k) is L(k,k) = 1
                lx[lnz++] = 1.0;
                for (p = top; p < n; p++) // L(k+1:n,k) = x / pivot
                {
                    i = xi[p];
                    if (pinv[i] < 0) // x(i) is an entry in L(:,k)
                    {
                        li[lnz] = i; // save unpermuted row in L
                        lx[lnz++] = x[i] / pivot; // scale pivot column
                    }
                    x[i] = 0.0; // x [0..n-1] = 0 for next k
                }
            }

            // Finalize L and U
            lp[n] = lnz;
            up[n] = unz;
            li = L.RowIndices; // fix row indices of L for final pinv
            for (p = 0; p < lnz; p++)
            {
                li[p] = pinv[li[p]];
            }

            // Remove extra space from L and U
            L.Resize(0);
            U.Resize(0);
        }

        /// <summary>
        /// Symbolic ordering and analysis for LU.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="A"></param>
        private void SymbolicAnalysis(ColumnOrdering order, CompressedColumnStorage<double> A)
        {
            var sym = this.symFactor = new SymbolicFactorization();

            // Fill-reducing ordering
            sym.q = AMD.Generate(A, order);
            
            // Guess nnz(L) and nnz(U)
            sym.unz = sym.lnz = 4 * (A.ColumnPointers[n]) + n;
        }

        /// <summary>
        /// LinearSolve Gx=b(:,k), where G is either upper (lo=false) or lower (lo=true)
        /// triangular.
        /// </summary>
        /// <param name="G">lower or upper triangular matrix in column-compressed form</param>
        /// <param name="B">right hand side, b=B(:,k)</param>
        /// <param name="k">use kth column of B as right hand side</param>
        /// <param name="xi">size 2*n, nonzero pattern of x in xi[top..n-1]</param>
        /// <param name="x">size n, x in x[xi[top..n-1]]</param>
        /// <param name="pinv">mapping of rows to columns of G, ignored if null</param>
        /// <param name="lo">true if lower triangular, false if upper</param>
        /// <returns>top, -1 in error</returns>
        private int SolveSp(CompressedColumnStorage<double> G, CompressedColumnStorage<double> B,
            int k, int[] xi, double[] x, int[] pinv, bool lo)
        {
            if (xi == null || x == null) return -1;

            var gp = G.ColumnPointers;
            var gi = G.RowIndices;
            var gx = G.Values;

            var bp = B.ColumnPointers;
            var bi = B.RowIndices;
            var bx = B.Values;

            int n = G.ColumnCount;

            // xi[top..n-1]=Reach(B(:,k))
            int top = GraphHelper.Reach(gp, gi, bp, bi, n, k, xi, pinv);

            int j, J, p, q, px;

            for (p = top; p < n; p++)
            {
                x[xi[p]] = 0; // clear x
            }

            for (p = bp[k]; p < bp[k + 1]; p++)
            {
                x[bi[p]] = bx[p]; // scatter B
            }

            for (px = top; px < n; px++)
            {
                j = xi[px]; // x(j) is nonzero
                J = pinv != null ? (pinv[j]) : j; // j maps to col J of G
                if (J < 0) continue; // column J is empty
                x[j] /= gx[lo ? (gp[J]) : (gp[J + 1] - 1)]; // x(j) /= G(j,j)
                p = lo ? (gp[J] + 1) : (gp[J]); // lo: L(j,j) 1st entry
                q = lo ? (gp[J + 1]) : (gp[J + 1] - 1); // up: U(j,j) last entry
                for (; p < q; p++)
                {
                    x[gi[p]] -= gx[p] * x[j]; // x(i) -= G(i,j) * x(j)
                }
            }

            // Return top of stack.
            return top;
        }
    }
}
