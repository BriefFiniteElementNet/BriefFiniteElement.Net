// -----------------------------------------------------------------------
// <copyright file="SparseQR.cs">
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
    /// Sparse QR decomposition.
    /// </summary>
    /// <remarks>
    /// See Chapter 5 (Orthogonal methods) in "Direct Methods for Sparse Linear Systems"
    /// by Tim Davis.
    /// </remarks>
    public class SparseQR : ISparseFactorization<double>
    {
        SymbolicFactorization symFactor;
        public CompressedColumnStorage<double> Q, R;
        double[] beta;
        int m, n;

        /// <summary>
        /// Creates a sparse QR factorization.
        /// </summary>
        /// <param name="order">Ordering method to use.</param>
        /// <param name="A">Column-compressed matrix.</param>
        public SparseQR(CompressedColumnStorage<double> A, ColumnOrdering order)
        {
            this.m = A.RowCount;
            this.n = A.ColumnCount;

            if (this.m >= this.n)
            {
                // Ordering and symbolic analysis
                SymbolicAnalysis(order, A);

                // Numeric QR factorization
                Factorize(A);
            }
            else
            {
                // Ax=b is underdetermined
                var AT = A.Transpose();

                // Ordering and symbolic analysis
                SymbolicAnalysis(order, AT);

                // Numeric QR factorization of A'
                Factorize(AT);
            }
        }

        /// <summary>
        /// Gets the number of nonzeros in both Q and R factors together.
        /// </summary>
        public int NonZerosCount
        {
            get { return (Q.NonZerosCount + R.NonZerosCount - m); }
        }

        /// <summary>
        /// Solves a linear system Ax=b.
        /// </summary>
        /// <param name="input">Right hand side b.</param>
        /// <param name="result">Solution vector x.</param>
        public void Solve(double[] input, double[] result)
        {
            var temp = Vector.Clone(input);

            // TODO: remove LinearSolve method with one argument
            // thus eleminating additional vector allocation. 
            Solve(temp);

            Vector.Copy(temp, result, result.Length);
        }

        // TODO: implement SolveTranspose

        /// <summary>
        /// LinearSolve a least-squares problem (min ||Ax-b||_2, where A is m-by-n
        /// with m >= n) or underdetermined system (Ax=b, where m &lt; n)
        /// </summary>
        /// <param name="b">size max(m,n), b (size m) on input, x(size n) on output</param>
        /// <returns>true if successful, false on error</returns>
        public void Solve(double[] b)
        {
            if (b == null) throw new ArgumentNullException("b");

            var x = new double[symFactor.m2];

            var up = R.ColumnPointers;
            var ui = R.RowIndices;
            var ux = R.Values;

            if (m >= n)
            {
                // x(0:m-1) = b(p(0:m-1)
                Permutation.ApplyInverse(symFactor.pinv, b, x, m);

                // Apply Householder reflection to x.
                for (int k = 0; k < n; k++)
                {
                    ApplyHouseholder(Q, k, beta[k], x);
                }

                // LinearSolve upper triangular system, x = R\x.
                for (int i = n - 1; i >= 0; i--)
                {
                    x[i] /= ux[up[i + 1] - 1];
                    for (int p = up[i]; p < up[i + 1] - 1; p++)
                    {
                        x[ui[p]] -= ux[p] * x[i];
                    }
                }

                // b(q(0:n-1)) = x(0:n-1)
                Permutation.ApplyInverse(symFactor.q, x, b, n);
            }
            else
            {
                // x(q(0:m-1)) = b(0:m-1)
                Permutation.Apply(symFactor.q, b, x, m);

                // TODO: int n = R.ColumnCount;

                // LinearSolve lower triangular system, x = R'\x.
                for (int i = 0; i < n; i++)
                {
                    for (int p = up[i]; p < up[i + 1] - 1; p++)
                    {
                        x[i] -= ux[p] * x[ui[p]];
                    }
                    x[i] /= ux[up[i + 1] - 1];
                }

                // Apply Householder reflection to x.
                for (int k = m - 1; k >= 0; k--)
                {
                    ApplyHouseholder(Q, k, beta[k], x);
                }

                // b(0:n-1) = x(p(0:n-1))
                Permutation.Apply(symFactor.pinv, x, b, n);
            }
        }

        /// <summary>
        /// Create a Householder reflection [v,beta,s]=house(x), overwrite x with v,
        /// where (I-beta*v*v')*x = s*e1 and e1 = [1 0 ... 0]'.
        /// </summary>
        /// <remarks>
        /// Note that this CXSparse version is different than CSparse.  See Higham,
        /// Accuracy and Stability of Num Algorithms, 2nd ed, 2002, page 357.
        /// </remarks>
        private double CreateHouseholder(double[] x, int offset, ref double beta, int n)
        {
            double s = 0;
            int i;
            if (x == null) return -1; // check inputs

            // s = norm(x)
            for (i = 0; i < n; i++)
            {
                s += x[offset + i] * x[offset + i];
            }

            s = Math.Sqrt(s);
            if (s == 0)
            {
                beta = 0;
                x[offset] = 1;
            }
            else
            {
                // s = sign(x[0]) * norm (x) ;
                if (x[offset] != 0)
                {
                    s *= x[offset] / Math.Abs(x[offset]);
                }
                x[offset] += s;
                beta = 1 / (s * x[offset]);
            }
            return (-s);
        }

        /// <summary>
        /// Apply the ith Householder vector to x.
        /// </summary>
        private bool ApplyHouseholder(CompressedColumnStorage<double> V, int i, double beta, double[] x)
        {
            int p;
            double tau = 0;

            if (x == null) return false; // check inputs

            var vp = V.ColumnPointers;
            var vi = V.RowIndices;
            var vx = V.Values;

            for (p = vp[i]; p < vp[i + 1]; p++) // tau = v'*x
            {
                tau += vx[p] * x[vi[p]];
            }
            tau *= beta; // tau = beta*(v'*x)
            for (p = vp[i]; p < vp[i + 1]; p++) // x = x - v*tau
            {
                x[vi[p]] -= vx[p] * tau;
            }
            return true;
        }

        /// <summary>
        /// Sparse QR factorization [V,beta,pinv,R] = qr(A)
        /// </summary>
        private void Factorize(CompressedColumnStorage<double> A)
        {
            int i, p, p1, top, len, col;

            int m = A.RowCount;
            int n = A.ColumnCount;

            var ap = A.ColumnPointers;
            var ai = A.RowIndices;
            var ax = A.Values;

            int[] q = symFactor.q;
            int[] parent = symFactor.parent;
            int[] pinv = symFactor.pinv;
            int m2 = symFactor.m2;

            int vnz = symFactor.lnz;
            int rnz = symFactor.unz;

            int[] leftmost = symFactor.leftmost;

            int[] w = new int[m2 + n]; // get int workspace
            double[] x = new double[m2]; // get double workspace

            int s = m2; // offset into w
            
            // Allocate result V, R and beta
            var V = this.Q = CompressedColumnStorage<double>.Create(m2, n, vnz);
            var R = this.R = CompressedColumnStorage<double>.Create(m2, n, rnz);
            var b = this.beta = new double[n];

            var rp = R.ColumnPointers;
            var ri = R.RowIndices;
            var rx = R.Values;

            var vp = V.ColumnPointers;
            var vi = V.RowIndices;
            var vx = V.Values;

            for (i = 0; i < m2; i++)
            {
                w[i] = -1; // clear w, to mark nodes
            }

            rnz = 0; vnz = 0;
            for (int k = 0; k < n; k++) // compute V and R
            {
                rp[k] = rnz;      // R(:,k) starts here
                vp[k] = p1 = vnz; // V(:,k) starts here
                w[k] = k;         // add V(k,k) to pattern of V
                vi[vnz++] = k;
                top = n;
                col = q != null ? q[k] : k;
                for (p = ap[col]; p < ap[col + 1]; p++) // find R(:,k) pattern
                {
                    i = leftmost[ai[p]]; // i = min(find(A(i,q)))
                    for (len = 0; w[i] != k; i = parent[i]) // traverse up to k
                    {
                        //len++;
                        w[s + len++] = i;
                        w[i] = k;
                    }
                    while (len > 0)
                    {
                        --top;
                        --len;
                        w[s + top] = w[s + len]; // push path on stack
                    }
                    i = pinv[ai[p]]; // i = permuted row of A(:,col)
                    x[i] = ax[p];    // x (i) = A(:,col)
                    if (i > k && w[i] < k) // pattern of V(:,k) = x (k+1:m)
                    {
                        vi[vnz++] = i; // add i to pattern of V(:,k)
                        w[i] = k;
                    }
                }
                for (p = top; p < n; p++) // for each i in pattern of R(:,k)
                {
                    i = w[s + p]; // R(i,k) is nonzero
                    ApplyHouseholder(V, i, b[i], x); // apply (V(i),Beta(i)) to x
                    ri[rnz] = i; // R(i,k) = x(i)
                    rx[rnz++] = x[i];
                    x[i] = 0;
                    if (parent[i] == k)
                    {
                        vnz = V.Scatter(i, 0, w, null, k, V, vnz);
                    }
                }
                for (p = p1; p < vnz; p++) // gather V(:,k) = x
                {
                    vx[p] = x[vi[p]];
                    x[vi[p]] = 0;
                }
                ri[rnz] = k; // R(k,k) = norm (x)
                rx[rnz++] = CreateHouseholder(vx, p1, ref b[k], vnz - p1); // [v,beta]=house(x)
            }

            rp[n] = rnz; // finalize R
            vp[n] = vnz; // finalize V
        }

        /// <summary>
        /// Symbolic ordering and analysis for QR.
        /// </summary>
        private void SymbolicAnalysis(ColumnOrdering order, CompressedColumnStorage<double> A)
        {
            int m = A.RowCount;
            int n = A.ColumnCount;

            var sym = this.symFactor = new SymbolicFactorization();

            // Fill-reducing ordering
            sym.q = AMD.Generate(A, order);

            var C = order > 0 ? Permute(A, null, sym.q) : SymbolicColumnStorage.Create(A);

            // etree of C'*C, where C=A(:,q)
            sym.parent = GraphHelper.EliminationTree(m, n, C.ColumnPointers, C.RowIndices, true);
            int[] post = GraphHelper.TreePostorder(sym.parent, n);
            sym.cp = GraphHelper.ColumnCounts(C, sym.parent, post, true); // col counts chol(C'*C)

            bool ok = C != null && sym.parent != null && sym.cp != null && CountV(C);

            if (ok)
            {
                sym.unz = 0;
                for (int k = 0; k < n; k++)
                {
                    sym.unz += sym.cp[k];
                }
            }
        }

        /// <summary>
        /// Compute nnz(V) = S.lnz, S.pinv, S.leftmost, S.m2 from A and S.parent
        /// </summary>
        private bool CountV(SymbolicColumnStorage A)
        {
            int i, k, p, pa;
            int[] head, tail, nque, pinv, leftmost, w, parent = symFactor.parent;

            int m = A.RowCount;
            int n = A.ColumnCount;
            int[] Ap = A.ColumnPointers;
            int[] Ai = A.RowIndices;

            symFactor.pinv = pinv = new int[m + n]; // allocate pinv,
            symFactor.leftmost = leftmost = new int[m]; // and leftmost

            w = new int[m]; // get workspace
            head = new int[n];
            tail = new int[n];
            nque = new int[n]; // Initialized to 0's

            for (k = 0; k < n; k++) head[k] = -1; // queue k is empty
            for (k = 0; k < n; k++) tail[k] = -1;
            for (i = 0; i < m; i++) leftmost[i] = -1;
            for (k = n - 1; k >= 0; k--)
            {
                for (p = Ap[k]; p < Ap[k + 1]; p++)
                {
                    leftmost[Ai[p]] = k; // leftmost[i] = min(find(A(i,:)))
                }
            }
            for (i = m - 1; i >= 0; i--) // scan rows in reverse order
            {
                pinv[i] = -1; // row i is not yet ordered
                k = leftmost[i];
                if (k == -1) continue; // row i is empty
                if (nque[k]++ == 0) tail[k] = i; // first row in queue k
                w[i] = head[k]; // put i at head of queue k
                head[k] = i;
            }
            symFactor.lnz = 0;
            symFactor.m2 = m;
            for (k = 0; k < n; k++) // find row permutation and nnz(V)
            {
                i = head[k]; // remove row i from queue k
                symFactor.lnz++; // count V(k,k) as nonzero
                if (i < 0) i = symFactor.m2++; // add a fictitious row
                pinv[i] = k; // associate row i with V(:,k)
                if (--nque[k] <= 0) continue; // skip if V(k+1:m,k) is empty
                symFactor.lnz += nque[k]; // nque [k] is nnz (V(k+1:m,k))
                if ((pa = parent[k]) != -1) // move all rows to parent of k
                {
                    if (nque[pa] == 0) tail[pa] = tail[k];
                    w[tail[k]] = head[pa];
                    head[pa] = w[i];
                    nque[pa] += nque[k];
                }
            }
            for (i = 0; i < m; i++)
            {
                if (pinv[i] < 0)
                {
                    pinv[i] = k++;
                }
            }

            return true;
        }

        /// <summary>
        /// Permutes a sparse matrix, C = PAQ.
        /// </summary>
        /// <param name="A">m-by-n, column-compressed matrix</param>
        /// <param name="pinv">a permutation vector of length m</param>
        /// <param name="q">a permutation vector of length n</param>
        /// <returns>C = PAQ, null on error</returns>
        private SymbolicColumnStorage Permute(CompressedColumnStorage<double> A, int[] pinv, int[] q)
        {
            int t, j, k, nz = 0;

            int m = A.RowCount;
            int n = A.ColumnCount;

            var ap = A.ColumnPointers;
            var ai = A.RowIndices;

            var result = new SymbolicColumnStorage(m, n, ap[n]);

            var cp = result.ColumnPointers;
            var ci = result.RowIndices;

            for (k = 0; k < n; k++)
            {
                // Column k of C is column q[k] of A
                cp[k] = nz;
                j = q != null ? (q[k]) : k;
                for (t = ap[j]; t < ap[j + 1]; t++)
                {
                    // Row i of A is row pinv[i] of C
                    ci[nz++] = pinv != null ? (pinv[ai[t]]) : ai[t];
                }
            }

            // Finalize the last column of C
            cp[n] = nz;

            return result;
        }
    }
}
