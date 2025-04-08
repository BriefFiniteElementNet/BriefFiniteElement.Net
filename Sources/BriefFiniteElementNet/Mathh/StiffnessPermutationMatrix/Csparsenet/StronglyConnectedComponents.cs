namespace CSparse.Ordering
{
    using BriefFiniteElementNet.Mathh.StiffnessPermutationMatrix.Csparsenet;
    using CSparse.Storage;
    using System;

    /// <summary>
    /// Strongly connected components.
    /// </summary>
    /// <remarks>
    /// See Chapter 7.3 (Fill-reducing orderings: Block triangular form) 
    /// in "Direct Methods for Sparse Linear Systems" by Tim Davis.
    /// </remarks>
    public sealed class StronglyConnectedComponents
    {
        internal int[] p;   // row permutation (size m)
        internal int[] r;   // block k is rows r[k] to r[k+1]-1 in A(p,q) (size nb+1)
        internal int nb;    // number of blocks in fine dmperm decomposition

        /// <summary>
        /// Create a new Decomposition instance. 
        /// </summary>
        private StronglyConnectedComponents(int m, int n)
        {
            p = new int[m];
            r = new int[m + 6];
        }

        /// <summary>
        /// Gets the number of strongly connected components.
        /// </summary>
        public int Blocks => nb;

        /// <summary>
        /// Gets the block pointers (block k is nodes r[k] to r[k+1]-1).
        /// </summary>
        public int[] BlockPointers => r;

        /// <summary>
        /// Gets the node indices.
        /// </summary>
        public int[] Indices => p;

        /// <summary>
        /// Compute strongly connected components of a matrix.
        /// </summary>
        /// <param name="matrix">Column-compressed matrix (representing a directed graph).</param>
        /// <returns>Strongly connected components</returns>
        public static StronglyConnectedComponents Generate<T>(CompressedColumnStorage<T> matrix)
             where T : struct, IEquatable<T>, IFormattable
        {
            throw new NotImplementedException();
            //return Generate(SymbolicColumnStorage.Create(matrix, false), matrix.ColumnCount);
        }

        /*
        /// <summary>
        /// Find strongly connected components of A.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        internal static StronglyConnectedComponents Generate(SymbolicColumnStorage A, int n)
        {
            // matrix A temporarily modified, then restored

            int i, k, b, nb = 0, top;
            int[] xi, p, r, Ap, ATp;

            var AT = A.Transpose(); // AT = A'

            Ap = A.ColumnPointers;
            ATp = AT.ColumnPointers;

            xi = new int[2 * n + 1]; // get workspace

            var D = new StronglyConnectedComponents(n, 0); // allocate result
            p = D.p;
            r = D.r;

            top = n;
            for (i = 0; i < n; i++) // first dfs(A) to find finish times (xi)
            {
                if (!(Ap[i] < 0))
                {
                    // This will modify the column pointers (mark all nodes).
                    top = GraphHelper.DepthFirstSearch(i, A.ColumnPointers, A.RowIndices, top, xi, xi, n, null);
                }
            }
            for (i = 0; i < n; i++)
            {
                // This will restore all column pointers (unmark all nodes).
                Ap[i] = -(Ap[i]) - 2; //CS_MARK(Ap, i);
            }
            top = n;
            nb = n;
            for (k = 0; k < n; k++) // dfs(A') to find strongly connected comp
            {
                i = xi[k]; // get i in reverse order of finish times
                if (ATp[i] < 0)
                {
                    continue; // skip node i if already ordered
                }
                r[nb--] = top; // node i is the start of a component in p
                top = GraphHelper.DepthFirstSearch(i, AT.ColumnPointers, AT.RowIndices, top, p, xi, n, null);
            }
            r[nb] = 0; // first block starts at zero; shift r up
            for (k = nb; k <= n; k++) r[k - nb] = r[k];
            D.nb = nb = n - nb; // nb = # of strongly connected components
            for (b = 0; b < nb; b++) // sort each block in natural order
            {
                for (k = r[b]; k < r[b + 1]; k++) xi[p[k]] = b;
            }
            for (b = 0; b <= nb; b++)
            {
                xi[n + b] = r[b];
            }
            for (i = 0; i < n; i++)
            {
                p[xi[n + xi[i]]++] = i;
            }

            Array.Resize(ref D.r, nb + 1);

            return D;
            
        }
        */
    }
}
