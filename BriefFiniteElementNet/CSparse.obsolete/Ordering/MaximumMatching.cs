// -----------------------------------------------------------------------
// <copyright file="MaximumMatching.cs">
// Original CSparse code by Tim Davis, http://www.cise.ufl.edu/research/sparse/CXSparse/
// CSparse.NET code by Christian Woltering, http://csparse.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

using System;
using BriefFiniteElementNet.CSparse.Storage;

namespace BriefFiniteElementNet.CSparse.Ordering
{
    /// <summary>
    /// Maximum matching of any matrix A (also called maximum transveral). 
    /// </summary>
    /// <remarks>
    /// See Chapter 7.2 (Fill-reducing orderings: Maximum matching) in 
    /// "Direct Methods for Sparse Linear Systems" by Tim Davis.
    /// </remarks>
    internal static class MaximumMatching
    {
        /// <summary>
        /// Find a maximum transveral (zero-free diagonal). Seed optionally selects a
        /// randomized algorithm.
        /// </summary>
        /// <param name="A">column-compressed matrix</param>
        /// <param name="seed">0: natural, -1: reverse, randomized otherwise</param>
        /// <returns>row and column matching, size m+n</returns>
        public static int[] Generate(SymbolicColumnStorage A, int seed)
        {
            int i, j, k, p, n2 = 0, m2 = 0;
            int[] jimatch, w, cheap, js, iss, ps, Cp, q;

            int n = A.ColumnCount;
            int m = A.RowCount;
            int[] Ap = A.ColumnPointers;
            int[] Ai = A.RowIndices;

            //[jmatch [0..m-1]; imatch [0..n-1]]
            w = jimatch = new int[m + n]; // allocate result

            for (k = 0, j = 0; j < n; j++) // count nonempty rows and columns
            {
                n2 += (Ap[j] < Ap[j + 1]) ? 1 : 0;
                for (p = Ap[j]; p < Ap[j + 1]; p++)
                {
                    w[Ai[p]] = 1;
                    k += (j == Ai[p]) ? 1 : 0; // count entries already on diagonal
                }
            }


            if (k == Math.Min(m, n)) // quick return if diagonal zero-free
            {
                for (i = 0; i < k; i++) jimatch[i] = i;
                for (; i < m; i++) jimatch[i] = -1;
                for (j = 0; j < k; j++) jimatch[m + j] = j;
                for (; j < n; j++) jimatch[m + j] = -1;

                return jimatch;
            }

            for (i = 0; i < m; i++) m2 += w[i];

            SymbolicColumnStorage C = null;

            if (m2 < n2)
            {
                // transpose if needed
                C = A.Transpose();
            }
            else
            {
                C = A.Clone();
            }

            if (C == null) return jimatch;

            n = C.ColumnCount;
            m = C.RowCount;
            Cp = C.ColumnPointers;

            int jmatch_offset = (m2 < n2) ? n : 0;
            int imatch_offset = (m2 < n2) ? 0 : m;

            w = new int[n]; // get workspace

            cheap = new int[n];
            js = new int[n];
            iss = new int[n];
            ps = new int[n];

            for (j = 0; j < n; j++) cheap[j] = Cp[j]; // for cheap assignment
            for (j = 0; j < n; j++) w[j] = -1; // all columns unflagged
            for (i = 0; i < m; i++) jimatch[jmatch_offset + i] = -1; // nothing matched yet

            q = Permutation.Create(n, seed); // q = random permutation
            for (k = 0; k < n; k++) // augment, starting at column q[k]
            {
                Augment(q[k], C.ColumnPointers, C.RowIndices, jimatch, jmatch_offset, cheap, w, js, iss, ps);
            }

            for (j = 0; j < n; j++)
            {
                jimatch[imatch_offset + j] = -1; // find row match
            }

            for (i = 0; i < m; i++)
            {
                if (jimatch[jmatch_offset + i] >= 0)
                {
                    jimatch[imatch_offset + jimatch[jmatch_offset + i]] = i;
                }
            }

            return jimatch;
        }

        /// <summary>
        /// Find an augmenting path starting at column k and extend the match if found.
        /// </summary>
        private static void Augment(int k, int[] colptr, int[] rowind, int[] jmatch, int jmatch_offset,
            int[] cheap, int[] w, int[] js, int[] iss, int[] ps)
        {
            bool found = false;
            int p, i = -1, head = 0, j;

            js[0] = k; // start with just node k in jstack
            while (head >= 0)
            {
                // Start (or continue) depth-first-search at node j
                j = js[head]; // get j from top of jstack
                if (w[j] != k) // 1st time j visited for kth path
                {
                    w[j] = k; // mark j as visited for kth path
                    for (p = cheap[j]; p < colptr[j + 1] && !found; p++)
                    {
                        i = rowind[p]; // try a cheap assignment (i,j)
                        found = (jmatch[jmatch_offset + i] == -1);
                    }
                    cheap[j] = p; // start here next time j is traversed*/
                    if (found)
                    {
                        iss[head] = i; // column j matched with row i
                        break; // end of augmenting path
                    }
                    ps[head] = colptr[j]; // no cheap match: start dfs for j
                }

                // Depth-first-search of neighbors of j
                for (p = ps[head]; p < colptr[j + 1]; p++)
                {
                    i = rowind[p]; // consider row i
                    if (w[jmatch[jmatch_offset + i]] == k)
                    {
                        continue; // skip jmatch [i] if marked
                    }
                    ps[head] = p + 1; // pause dfs of node j
                    iss[head] = i; // i will be matched with j if found
                    head++;
                    js[head] = jmatch[jmatch_offset + i]; // start dfs at column jmatch [i]
                    break;
                }
                if (p == colptr[j + 1])
                {
                    head--; // node j is done; pop from stack
                }
            }
            // augment the match if path found:
            if (found)
            {
                for (p = head; p >= 0; p--)
                {
                    jmatch[jmatch_offset + iss[p]] = js[p];
                }
            }
        }
    }
}
