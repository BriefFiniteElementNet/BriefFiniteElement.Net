namespace CSparse
{
    using BriefFiniteElementNet.Mathh.StiffnessPermutationMatrix.Csparsenet;
    using CSparse.Storage;
    using System;

    /// <summary>
    /// Helper methods for sparse direct solvers.
    /// </summary>
    internal static class GraphHelper
    {
        /// <summary>
        /// Depth-first-search of the graph of a matrix, starting at node j.
        /// </summary>
        /// <param name="j">starting node</param>
        /// <param name="Gp">graph to search (modified, then restored)</param>
        /// <param name="Gi">graph to search</param>
        /// <param name="top">stack[top..n-1] is used on input</param>
        /// <param name="xi">size n, stack containing nodes traversed</param>
        /// <param name="pstack">size n, work array</param>
        /// <param name="offset">the index of the first element in array pstack</param>
        /// <param name="pinv">mapping of rows to columns of G, ignored if null</param>
        /// <returns>new value of top, -1 on error</returns>
        public static int DepthFirstSearch(int j, int[] Gp, int[] Gi, int top, int[] xi,
            int[] pstack, int offset, int[] pinv)
        {
            int i, p, p2, jnew, head = 0;
            bool done;

            if (xi == null || pstack == null) return -1;

            xi[0] = j; // initialize the recursion stack
            while (head >= 0)
            {
                j = xi[head]; // get j from the top of the recursion stack
                jnew = pinv != null ? (pinv[j]) : j;
                if (!(Gp[j] < 0))
                {
                    //CS_MARK(Gp, j);
                    Gp[j] = -(Gp[j]) - 2; // mark node j as visited
                    pstack[offset + head] = (jnew < 0) ? 0 : // CS_UNFLIP(Gp[jnew]);
                        ((Gp[jnew] < 0) ? -Gp[jnew] - 2 : Gp[jnew]);
                }
                done = true; // node j done if no unvisited neighbors
                p2 = (jnew < 0) ? 0 : // CS_UNFLIP(Gp[jnew + 1]);
                    ((Gp[jnew + 1] < 0) ? -Gp[jnew + 1] - 2 : Gp[jnew + 1]);

                for (p = pstack[offset + head]; p < p2; p++) // examine all neighbors of j
                {
                    i = Gi[p]; // consider neighbor node i
                    if (Gp[i] < 0) continue; // skip visited node i
                    pstack[offset + head] = p; // pause depth-first search of node j
                    xi[++head] = i; // start dfs at node i
                    done = false; // node j is not done
                    break; // break, to start dfs (i)
                }
                if (done) // depth-first search at node j is done
                {
                    head--; // remove j from the recursion stack
                    xi[--top] = j; // and place in the output stack
                }
            }
            return (top);
        }

        /// <summary>
        /// Compute the elimination tree of A or A'A (without forming A'A).
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <param name="colptr">Column pointers of column-compressed matrix.</param>
        /// <param name="rowind">Row indices of column-compressed matrix.</param>
        /// <param name="ata">analyze A if false, A'A oterwise</param>
        /// <returns>elimination tree, null on error</returns>
        public static int[] EliminationTree(int m, int n, int[] colptr, int[] rowind, bool ata)
        {
            int i, k, p, inext;

            int[] parent = new int[n]; // allocate result

            int[] ancestor = new int[n];
            int[] prev = null;

            if (ata)
            {
                prev = new int[m];
                for (i = 0; i < m; i++) prev[i] = -1;
            }

            for (k = 0; k < n; k++)
            {
                parent[k] = -1; // node k has no parent yet
                ancestor[k] = -1; // nor does k have an ancestor
                for (p = colptr[k]; p < colptr[k + 1]; p++)
                {
                    i = ata ? (prev[rowind[p]]) : (rowind[p]);
                    for (; i != -1 && i < k; i = inext) // traverse from i to k
                    {
                        inext = ancestor[i]; // inext = ancestor of i
                        ancestor[i] = k; // path compression
                        if (inext == -1) parent[i] = k; // no anc., parent is k
                    }
                    if (ata) prev[rowind[p]] = k;
                }
            }
            return parent;
        }

        /// <summary>
        /// Postorders a tree of forest.
        /// </summary>
        /// <param name="parent">defines a tree of n nodes</param>
        /// <param name="n">length of parent</param>
        /// <returns>post[k]=i, null on error</returns>
        public static int[] TreePostorder(int[] parent, int n)
        {
            int j, k = 0;

            if (parent == null) return null; // check inputs

            int[] post = new int[n]; // allocate result
            int[] w = new int[n]; // get workspace
            int[] next = new int[n];
            int[] stack = new int[n];

            for (j = 0; j < n; j++) w[j] = -1; // empty linked lists
            for (j = n - 1; j >= 0; j--) // traverse nodes in reverse order*/
            {
                if (parent[j] == -1) continue; // j is a root
                next[j] = w[parent[j]]; // add j to list of its parent
                w[parent[j]] = j;
            }
            for (j = 0; j < n; j++)
            {
                if (parent[j] != -1) continue; // skip j if it is not a root
                k = TreeDepthFirstSearch(j, k, w, next, post, stack);
            }
            return post; // success; free w, return post
        }

        /*
        /// <summary>
        /// Column counts for Cholesky (LL'=A or LL'=A'A) and QR, given parent and post ordering.
        /// </summary>
        public static int[] ColumnCounts(SymbolicColumnStorage A, int[] parent, int[] post, bool ata)
        {
            int i, j, k, J, p, q, jleaf = 0;
            int[] ATp, ATi, colcount, delta, head = null, next = null;

            if (parent == null || post == null) return (null); // check inputs

            int m = A.RowCount;
            int n = A.ColumnCount;

            delta = colcount = new int[n]; // allocate result

            var AT = A.Transpose(); // AT = A'

            // w is ancestor
            int[] w = new int[n]; // get workspace

            int[] maxfirst = new int[n];
            int[] prevleaf = new int[n];
            int[] first = new int[n];

            for (k = 0; k < n; k++)
            {
                w[k] = -1; // clear workspace w [0..s-1]
            }
            Array.Copy(w, maxfirst, n);
            Array.Copy(w, prevleaf, n);
            Array.Copy(w, first, n);

            for (k = 0; k < n; k++) // find first [j]
            {
                j = post[k];
                delta[j] = (first[j] == -1) ? 1 : 0; // delta[j]=1 if j is a leaf
                for (; j != -1 && first[j] == -1; j = parent[j])
                {
                    first[j] = k;
                }
            }
            ATp = AT.ColumnPointers;
            ATi = AT.RowIndices;

            if (ata) // Init ata
            {
                head = new int[n + 1];
                next = new int[m];

                Array.Copy(w, head, n);
                head[n] = -1;

                for (k = 0; k < n; k++)
                {
                    w[post[k]] = k; // invert post
                }
                for (i = 0; i < m; i++)
                {
                    for (k = n, p = ATp[i]; p < ATp[i + 1]; p++)
                    {
                        k = Math.Min(k, w[ATi[p]]);
                    }
                    next[i] = head[k]; // place row i in linked list k
                    head[k] = i;
                }
            }

            for (i = 0; i < n; i++) w[i] = i; // each node in its own set
            for (k = 0; k < n; k++)
            {
                j = post[k]; // j is the kth node in postordered etree
                if (parent[j] != -1) delta[parent[j]]--; // j is not a root

                //int HEAD(k,j) (ata ? head [k] : j)
                //int NEXT(J)   (ata ? next [J] : -1)
                for (J = (ata ? head[k] : j); J != -1; J = (ata ? next[J] : -1)) // J=j for LL'=A case
                {
                    for (p = ATp[J]; p < ATp[J + 1]; p++)
                    {
                        i = ATi[p];
                        q = IsLeaf(i, j, first, maxfirst, prevleaf, w, ref jleaf);
                        if (jleaf >= 1) delta[j]++; // A(i,j) is in skeleton
                        if (jleaf == 2) delta[q]--; // account for overlap in q
                    }
                }
                if (parent[j] != -1) w[j] = parent[j];
            }
            for (j = 0; j < n; j++) // sum up delta's of each child
            {
                if (parent[j] != -1) colcount[parent[j]] += colcount[j];
            }
            return colcount; // success: free workspace
        }

        */

        /// <summary>
        /// Determines if j is a leaf of the skeleton matrix and find lowest common
        /// ancestor (lca).
        /// </summary>
        /// <returns>Least common ancestor (jprev,j)</returns>
        static int IsLeaf(int i, int j, int[] first, int[] maxfirst, int[] prevleaf,
            int[] ancestor, ref int jleaf)
        {
            int q, s, sparent, jprev;
            if (first == null || maxfirst == null || prevleaf == null || ancestor == null)
            {
                return (-1);
            }
            jleaf = 0;
            if (i <= j || first[j] <= maxfirst[i])
            {
                return (-1); // j not a leaf
            }
            maxfirst[i] = first[j]; // update max first[j] seen so far
            jprev = prevleaf[i]; // jprev = previous leaf of ith subtree
            prevleaf[i] = j;
            jleaf = (jprev == -1) ? 1 : 2; // j is first or subsequent leaf
            if (jleaf == 1) return (i); // if 1st leaf, q = root of ith subtree
            for (q = jprev; q != ancestor[q]; q = ancestor[q]) ;
            for (s = jprev; s != q; s = sparent)
            {
                sparent = ancestor[s]; // path compression
                ancestor[s] = q;
            }
            return (q);
        }

        /// <summary>
        /// Depth-first search and postorder of a tree rooted at node j
        /// </summary>
        /// <param name="j">postorder of a tree rooted at node j</param>
        /// <param name="k">number of nodes ordered so far</param>
        /// <param name="head">head[i] is first child of node i; -1 on output</param>
        /// <param name="next">next[i] is next sibling of i or -1 if none</param>
        /// <param name="post">postordering</param>
        /// <param name="stack">size n, work array</param>
        /// <returns>new value of k, -1 on error</returns>
        public static int TreeDepthFirstSearch(int j, int k, int[] head, int[] next, int[] post, int[] stack)
        {
            int i, p, top = 0;

            if (head == null || next == null || post == null || stack == null)
                return (-1); // check inputs

            stack[0] = j; // place j on the stack
            while (top >= 0) // while (stack is not empty)
            {
                p = stack[top]; // p = top of stack
                i = head[p]; // i = youngest child of p
                if (i == -1)
                {
                    top--; // p has no unordered children left
                    post[k++] = p; // node p is the kth postordered node
                }
                else
                {
                    head[p] = next[i]; // remove i from children of p
                    top++;
                    stack[top] = i; // start dfs on child node i
                }
            }
            return (k);
        }

        // xi [top...n-1] = nodes reachable from graph of G*P' via nodes in B(:,k).
        // xi [n...2n-1] used as workspace
        public static int Reach(int[] Gp, int[] Gi, int[] Bp, int[] Bi, int n, int k, int[] xi, int[] pinv)
        {
            if (xi == null) return (-1); // check inputs

            int p, top = n;

            for (p = Bp[k]; p < Bp[k + 1]; p++)
            {
                //if (!CS_MARKED(Gp, Bi[p]))
                if (!(Gp[Bi[p]] < 0)) // start a dfs at unmarked node i
                {
                    top = DepthFirstSearch(Bi[p], Gp, Gi, top, xi, xi, n, pinv);
                }
            }

            for (p = top; p < n; p++)
            {
                //CS_MARK(Gp, xi[p]);
                Gp[xi[p]] = -(Gp[xi[p]]) - 2; // restore G
            }

            return (top);
        }


        /*
        /// <summary>
        /// Find nonzero pattern of Cholesky L(k,1:k-1) using etree and triu(A(:,k))
        /// </summary>
        public static int EtreeReach(SymbolicColumnStorage A, int k, int[] parent, int[] s, int[] w)
        {
            int i, p, n, len;

            if (parent == null || s == null || w == null) return -1;   // check inputs

            int top = n = A.ColumnCount;
            int[] Ap = A.ColumnPointers;
            int[] Ai = A.RowIndices;

            //CS_MARK(w, k);
            w[k] = -w[k] - 2; // mark node k as visited

            for (p = Ap[k]; p < Ap[k + 1]; p++)
            {
                i = Ai[p]; // A(i,k) is nonzero
                if (i > k)
                {
                    continue; // only use upper triangular part of A
                }
                for (len = 0; !(w[i] < 0); i = parent[i]) // traverse up etree
                {
                    s[len++] = i; // L(k,i) is nonzero
                    //CS_MARK(w, i);       
                    w[i] = -w[i] - 2; // mark i as visited
                }
                while (len > 0)
                {
                    s[--top] = s[--len]; // push path onto stack
                }
            }
            for (p = top; p < n; p++)
            {
                //CS_MARK(w, s[p]);
                w[s[p]] = -w[s[p]] - 2; // unmark all nodes
            }
            //CS_MARK(w, k);
            w[k] = -w[k] - 2; // unmark node k

            return top; // s [top..n-1] contains pattern of L(k,:)
        }

        */
    }
}
