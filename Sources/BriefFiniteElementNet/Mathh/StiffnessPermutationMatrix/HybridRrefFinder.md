
# Hybrid Rref Finder
Usually in FEM models, a large matrix which contains boundary conditions of the model
is versy sparse matrix. if there is no MPC element, then the matrix is in RREF form
by itself, but each MPC element will likely make a submatrix which is independent of other parts.


- First step is to enumerate parts. 
- second step to extract parts with more than one node, as dense
- Find rref of dense matrix
- remove old submatrix and add new submatrix.

## Enumerate Parts

group nodes in a way that related variables are in same group.

--------
 for example:
https://github.com/wo80/CSparse.NET/issues/49

x1=0.1
x2=0.3
x3=0
x4=0
x5=0
3x6+2x7=0
4x6-3*x7=2

I expect this grouping from above eq system:
x_i 	group index
x1 	1
x2 	2
x3 	3
x4 	4
x5 	5
x6 	6
x7 	6

x6 and x7 are relative so they will be in same group. Could you please give me some guidance?


--------------

### Adjacency Matrix

To do so, we need to find adjacency matrix. We have matrix $A$ with size n*m (rectangular). $S=A^T*A$ will give us an n*n matrix. 
$S_{i,j}$ is internal product i'th and j'th column of $A$. If A is symbolic matrix, then $A_{:,i}.A_{:,j}$ will be nonzero if there is at least one nonzero in same index of those columns. in other words, $S_{i,j}$ is nonzero if $x_i$ and $x_j$ are seen at least in one equation at same time. the $S$ matrix is symmetric means the graph is undirected.

### Strongly connected components

