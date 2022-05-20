Intro
####

There is a unified way for calculating the displacement permutation matrix and that is described in the documentation, currently here:
Solving Procedure:
https://bfenet.readthedocs.io/en/latest/miscellaneoustopics/solveprocedure.html

This procedure needs the RREF (reduced row echeleon) form of extra equations matrix. When [asking the csparse.net](https://github.com/wo80/CSparse.NET/issues/7) turned out that there can be another way for doing this using QR decomoposition
already implemented in CSparse.Net, as there is no RREF implementation currently available for .net. the procedure is described here:

imagine we have a model with `n` dof, then stiffness matrix is `n*n` also vector and disp vector's length are `n`. Whe will have several extra equations related to boundary conditions
 and MPC (multi point constraint) elements like rigid diaphragm. for example `u1 = 0` shows 1'st dof is connected to ground and displacement is expected to be zero. 
 it shows a boundary condition. we can make another matrix with size `m * n+1` where `m` is number of extra equations (boundary conditions plus eq.s from MPC element).
 the last column is right side of equations. for more on this have a look at solving procedure.

 we call this extra equations matrix as CE (Constraint Equations) matrix. from now on we can use SparseQr to convert CE matrix to displacement permutation matrix. 
 if there have been RREF feature available for CRS sparse matrices in .NET then i wouldn't use this somehow complicated procedure! 

First thing to go on is to find non usable equations (or rows) in CE matrix. I call these rows as Dependent Rows which i mean these are non-usable rows. For example we have 
3 equations:

`
x1=x2
x2=x3
x3=x1
`

if x1=x2 and x2=x3 then obviously x3=x1, or i.e. one of these three equations is useless and can be resulted from other equations. to find these rows wo80 suggested:
QR decompose the CE matrix, extract the private field R with reflection and if `ABS(qr[i,i]) < epsilon` then row `i` is useless. but we should consider permutation and ordering,
so if `ABS(qr[i,i]) < epsilon` then row `qr.q[i]` in original CE matrix is useless.

we can create another `int[]` vector with length : qr.r.RowCount and name `leades`. the i'th member is: if i'th row of CE is non usable then it is -1, otherwise it
 is column number of first nonzero of row, not sure where it can be used!

next to split R matrix into two parts, we create two permute matrices P1 and P2. 

`
var rdep = p1.Multiply(r).Multiply(p2);
var rinDep = p1.Multiply(r).Multiply(p2p);
`

Lets say we have an problem of `n` dof, K is `nxn` and `D` and `F` vectors are `nx1`. 
we have `m` extra equations related to boundary conditions and MPC elements, and can form a mtrix CE (Constrain Equations) with size `m by n+1`, where the last column is right hand side of constrain equations.
Main problem is `F=K.D` where size is `n`. if we have CE matrix with row count `m` and rank `o` (mean o of m equations are usable ones), then we can reduce the problem
into smaller one, `Fr = Kr . Dr` where `Kr` is posdef and directly possible to factorize with Cholsky. Also size would be `n-o` which means we reduced the matrix dimensions
 or equation by `o` (count of real extra equations). well how we can find Fr, Kr and Dr and how we can then find the F and D as the result?

 lets say the CE matrix is: `C.X = B` (where C: m by n, X:n by 1 and B: m by 1)


to see what is rdep and rindep look csparse.net at issue#7 noted earlier. after split `R` to `rdep` and `rIndep` we should solve the triangular system Rdep for Rindep then gives us another matrix.

Forgot to say we are using master slave method for applygin boundary conditions.

 algorithm name: Find displacement permute from CE matrix

inputs: CE matrix
output: Displacement Permute (DP) matrix size `n*n`, Displacement Permute Right Hand Side vector size `n*1`
variable desc: 

- n: size of original problem
- m: count of extra equations
- o: count of extra equations minus count of nonusable equations, i.e usable extra equation count, i.e. number of slave Dofs.

body: regarding [issue#7 is csparse.net](https://github.com/wo80/CSparse.NET/issues/7).

steps: 

1- find `o` and identify slave dofs for example x1, x5 and x7 are slave dofs (or fixed to ground with specific displacement as settlement or zero displacement)
2- decompose CE with QR into r(CE) and q(CE)
3- find `p1` and `p2` and `p2p` in a way that 
    `p1*r(CE)*p2 : rdep` 
    `p1*r(CE)*p2p : rindep` 

4- Decompose `rDep` with QR into `qr(rDep)`.
5- Dim `right : new int[rinDep.RowCount]`
6- Dim `sol : new int[rinDep.RowCount]`
7- for j=0 to `rinDep.Cols`:
    Dim `sol = qr(rDep).solve(rinDep.Cols[i])`
    `for (var i = 0; i < rdep.RowCount; i++) bufCrd.At(i, j, sol[i]);`
8- to be done



9- find displacement permutation matrix (`Dp`) size: `n by o` and Displacement permute right side vector (`Dpr`) size `n by 1`



10- form the `Cr` `Xr` and `Br` matrices in order to 