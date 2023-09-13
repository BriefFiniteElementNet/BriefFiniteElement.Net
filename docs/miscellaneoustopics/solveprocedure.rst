Solving Procedure
=================

By solving writer means converting model and loads and other things into mathematics form, [e.g. stiffness matrice, force and displacement vector], then solve the result linear equation system and finally convert back the results into .NET objects.

In this library solving a linear finite element model in structural topic have these steps:

1. Forming the full load vector and full stiffness matrix and full displacement vector. note that only unconstrained part of force and constrained part of displacement vector is filled with non-zeros in this step.
2. Applying boundary conditions due to support constrains and MPC (Multi Point Constrain) elements.
3. Solving the equation system as `K x = F` where x is unknown displacement
4. Finding the unknown forces (support reactions) with using unknown displacements.
5. Inserting full displacement and force values into full displacement and force vector.
6. Store full displacement and load vectors in `Model` for later usages.

 
Forming Full Stiffness, Load and Displacement Matrices
------------------------------------------------------
a `Model` can have unlimited `Nodes`, where each node have a fixed number of 6 DoF (Degree of Freedom). So the full stiffness matrix will be a square matrix of `6*n` by `6*n`. Also force and displacement vectors will be vectors with length of `6*n` that we use single column matrices to store them. A `double[]` array also can be used but we will use single column matrices next.

To form full stiffness matrix, we should first create a zero matrix with size of `6*n` by `6*n`, then assemble the stiffness matrices, element by element into full stiffness matrix. This happens in `MatrixAssemblerUtil.AssembleFullStiffnessMatrix(Model)` method and returns the full stiffness matrix as a sparse matrix. For example imagine we have a model with 10k free DoFs, then the stiffness matrix would be a matrix by 10000*10000=100'000'000 members. if each member need a 8 byte RAM as double precision value, then we'll need minimum 800 MB free ram to start analysing such a model. maybe 800 MB is found on all computers but about a model with 100k free DoFs it would be around 800 GB of RAM, which is almost always is not present. Usually most of members of stiffness matrix are zeros, and member at row `i` and column `j` is usually zero if there is no element connecting the corresponding Nodes and DoFs. These type of matrices are named Sparse Matrices which have a few non-zero members. A structure with 4 roofs and 25 column in each level, will have a total number of 125 node, thus `125*6=750` DoFs, but only 8250 non-zero members on stiffness matrix which is ~1.5% of full stiffness matrix. For a 10 roof structure with 100 column in each level, total nodes are 1000, total DoF are 6000 and non-zero count is 72k members which non-zero ratio is about 0.2%, and for a 50x50x50 3d grid non-zero ratio is 0.0017%. Several techniques are created for only storing the non-zero members of matrices in memory. After forming the stiffness matrix we should apply boundary conditions and then solve a linear system of equations to find unknown displacements. These procedures should all be done on sparse matrices. This library uses another library named CSparse.Net for handling sparse matrices, and that library uses Compressed Column Storage (CSR) format for keeping non-zero members of sparse matrices.

Also another zero matrix with length of 6*n by 1 is assumed as total displacement matrix and another one with same dimension for total force matrix. If we have any settlement on nodes, we'll fill them into appropriated members of total displacement vector, also we should convert loads applied on elements (like distributed loads) into equivalent nodal loads, and then add with nodal loads and then finally set members of total force vector. Note that in this stage all members corresponding to free DoFs in total displacement matrix are zero (which are unknown nodal displacements), also all members corresponding to fixed DoFs in force matrix are zero too (which are unknown support reactions).


Applying Boundary Conditions and MPC elements
---------------------------------------------

After forming the total stiffness matrix and total force and displacement vectors, then we should apply the boundary conditions. There are at least a usual way of converting the stiffness matrix into four parts, also displacement and force vectors into two parts each like this:

:math:`K = \begin{bmatrix}K_{ff} & K_{fs} \\K_{sf} & K_{ss}\end{bmatrix}`\

:math:`U = \begin{bmatrix}U_{f} \\U_{s} \end{bmatrix}`\

:math:`F = \begin{bmatrix}F_{f} \\F_{s} \end{bmatrix}`\

Where the f and s subscript tails in above names are related to Fixed and Not Fixed (or Released) DoFs. Then the main equation of `K * U = F` will convert to this form:

:math:`\begin{bmatrix}K_{ff} & K_{fs} \\K_{sf} & K_{ss}\end{bmatrix} * \begin{bmatrix}U_{f} \\U_{s} \end{bmatrix} = \begin{bmatrix}F_{f} \\F_{s} \end{bmatrix}`\

Expanding the matrix multiplies:

:math:`K_{ff} * U_f + K_{fs} * U_s = F_f`\

:math:`K_{fs} * U_f + K_{ss} * U_s = F_s`\

We have all terms known, expect :math:`U_f` and :math:`F_s`. We are searching for the :math:`U_f` and :math:`F_s` which are displacement of free DoFs and external force on fixed DoFs (e.g support reactions). So we can rewrite these into:

:math:`U_f  = K_{ff} ^{-1} * (F_f - K_{fs} * U_s )`\

And then we can find :math:`F_s` (support reactions) from this:

:math:`F_s = K_{fs} * U_f + K_{ss} * U_s`\

finally both unknown term will be found. 

Also for applying effect of MPC elements, Since the models are linear, the master slave method is used for considering the rigid elements. You can find a detail about how it is with this paper. In linear analysis displacement vector, force vector and stiffness matrix should be determined and after some calculation, we should solve a linear equation system and displacements will be found.

Because of reducing the count of degree of freedoms (DoF) of structure, rigid element will reduce the stiffness, mass and damp matrix dimensions. For example, consider a problem with no settlement which can be shown as:

:math:`F = K . U`\   then :math:`U = K^-1 * F`\

Let's say we have n DoFs (in previous section we used `n` for total number nodes, but here as total number of DoFs, because this section is coppied from another article). If rigid elements do connect two nodes with constrained supports, we can define a matrix  Pf (m * n) which when multiplied to the F will get a Fr vector which is force vector for Reduced structure (after applying the rigid elements to reduce DoFs). Also can define a Pd (n * m) matrix, which when is multiplied to Dr (Dr is displacement vector for reduced structure) will give the D which is displacement vector for original structure as below:

Fr = Pf * F

D = Pd * Dr

Can combine these two equations with first one as:

F = K . Pd . Dr  (pre multiply both sides with Pf)=> Pf . F = Fr = Pf . K . Pd . Dr

Taking Kr = Pf . K . Pd

Fr = Kr * Dr

This way, we can convert the problem into a reduced problem. Same can be applied for dynamic analysis:

M . Ẍ + C . Ẋ + K . X = F(t)

Taking:

X = Pd * Xr => Ẋ = Pd * Ẋr => Ẍ = Pd * Ẍr

Then:

M . Pd * Ẍr + C . Pd * Ẋr + K . Pd * Xr = F(t)

Premultiply which Pf:

Pf . M . Pd * Ẍr + Pf . C . Pd * Ẋr + Pf . K . Pd * Xr = Pf . F = Fr

Fr = Mr . Ẍr + Cr . Ẋr + Kr . Xr

where:

  Mr = Pf . M . Pd

  Cr = Pf . C . Pd

  Kr = Pf . K . Pd

These two ways of solving system for unknowns and also applying effect of MPC elements, are simple ways but not used in latest version of BFE, but in earlier version, because combining these two procedures will probably result in a complex code and also error prone as i remember when i was dealing permutation thing and that resulted into a class named `DofMappingManager` (probably still presented in the source code) with very hard usage. Instead another method is used to apply both boundary conditions and extra eqaution of MPC element in same time. This method is described in next.

Applying Boundary Conditions and MPC elements - new method
----------------------------------------------------------

 - Step 1: Extract equations related to boundary conditions and constraints
 - Step 2: None
 - Step 3: Create Reduced Row Echelon Form (RREF) of Step 1
 - Step 4: Make pioneer members to -1 by multiplying whole row (for definition of pioneer members please continue reading)
 - Step 5: Insert each row into appropriated row of a `nxn` matrix where n is total number of DoFs
 - Step 6: Remove extra columns
 
Step 1: Extract equations related to boundary conditions and constraints
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

A Finite Element model does have boundary conditions (e.g support DoFs and settlements), also MPC (Multi Point Constraint) elements like rigid diaphragm, and SPC (Multi Point Constraint) elements like virtual constraints. All of these can be represented as equations. For example:


 - U :sub:`11`\ = 0 : U :sub:`11`\  DoF is connected to ground without settlement
 - U :sub:`12`\ = 0.1 : U :sub:`12`\  DoF is connected to ground with settlement amount of 0.1 in that direction
 - U :sub:`12`\ = U :sub:`13`\ : U :sub:`12`\  is equal to U :sub:`13`\
 - U :sub:`16`\ = 2*U :sub:`12`\ + 3*U :sub:`13`\  : U :sub:`16`\  is connected to U :sub:`12`\  and U :sub:`13`\ with a MPC element (like rigid diaphragm or ...).
 
Every boundary condition and and MPC/SPC element will give a set of these extra equations, and every equation can be represented as a row of a matrix with column count equal to total number of DoFs in that model, plus a right hand side vector. Above equations can turn into matrix rows plus a right hand side like below table:

.. list-table:: Title
   :widths: 25 50 30 30 30 30 30 30 30
   :header-rows: 1

   * - Eq. Number
     - Equation
     - U :sub:`11`\'s coeff.
     - U :sub:`12`\'s coeff.
     - U :sub:`13`\'s coeff.
     - U :sub:`14`\'s coeff.
     - U :sub:`15`\'s coeff.
     - U :sub:`16`\'s coeff.
     - Right Side

   * - 1
     - U :sub:`11`\=0
     - 1
     - 0
     - 0
     - 0
     - 0
     - 0
     - 0

   * - 2
     - U :sub:`12`\=0.1
     - 0
     - 1
     - 0
     - 0
     - 0
     - 0
     - 0.1

   * - 3
     - U :sub:`13`\ = U :sub:`12`\
     - 0
     - -1
     - 1
     - 0
     - 0
     - 0
     - 0

   * - 4
     - U :sub:`16`\ = 2*U :sub:`12`\ + 3*U :sub:`13`\
     - 0
     - -2
     - -3
     - 0
     - 1
     - 0
     - 0

Finally there will be a system with `m` rows and `n` columns and a right side vector:

TODO

:math:`P_1*U=R_1`

Step 3: Create Reduced Row Echelon Form (RREF) of Step 1
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Next step is to compute RREF form of :math:`P_1`\ matrix calculated in step 1 with gauss elimination. We should start from column 0, choose a row with non-zero member at column 0, then eliminate members of all other rows that have a non-zero element at column 0. Do same thing for all columns from 1 to n, where n is total number of DoFs. The operation will stop when in every `i`th column of matrix. all members are zero or at most one non-zero element. In other words elimination will stop when in each column there is at most one non-zero member. After elimination done for each `i` th row there is three possible cases:

1. There are one or several non-zeros on row `i`.

2. All members of row `i` are zero, also right side at row `i` is zero. This means the equation corresponding to that row was not useful, but also is not a problem. For example :math:`U_1 = U_2`\ , :math:`U_2 = U_3`\ , :math:`U_3 = U_1`\ can be result of three SPC elements, but only two of them are useful and third one is result of first and second. 

3. All members of row `i` are zero, but right side at row `i` is non-zero (we should also consider floating point operation stuff, so check with small epsilon number instead of zero 0.0). This means an error. Like two inconsistent settlements on two DoFs or nodes that are connected with a SPC or MPC element. :math:`U_1 = 0.1`\ , :math:`U_2 = 0.2`\ , :math:`U_1 = U_2`\ . 

Rows with all members zero and right side zero will be removed from result, and rows with all members zero but right side non zero will cause solving procedure failure, because of invalid user input.

Finally there will be a matrix :math:`P_3`\ with `o` rows and `n` columns, that `o<=m` (`m` is total extra equation count) due to removing useless rows. Also as this matrix is RREF form, then there are `o` columns with only one non-zero element. If a member be the only non-zero member in the column, we call that member "pioneer" or "leading" member. Finally we have :math:`P_3*U=R_3`\ where :math:`P_3`\ is in RREF form.

Step 4: Make pioneer members equal to -1 by multiplying whole row with a coefficient
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

we shaould take output of last step, :math:`P_3`\ and :math:`R_3`\. Then multiply each row and corresponding right side member with a coefficient in a way that pioneer member turn into `-1.0`. result is :math:`P_4`\ and :math:`R_4`\.

Step 5: Insert each row into appropriated row of a `nxn` matrix where n is total number of DoFs
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Create and empty :math:`P_5`\ matrix with size `n` by `n`, also a vector :math:`R_5`\ with size `n` by `1`. Then for each i'th row of :math:`P_4`\ with pioneer member at column `j`, insert it into `i`'th row of :math:`P_5`\ and :math:`R_5`\. Next we should replace the zeros on main diagonal of :math:`P_5`\ with `1.0` and no change in :math:`R_5`\.


Step 6: Remove extra columns
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Remove columns that have pioneer member equal to `-1.0` and no change to right side. Final result is :math:`P_6`\ with size `n` by `o` and :math:`R_6`\ with size `n` by `1`.

:math:`P_6`\ is displacement expander and `o` is total number of master DoF count.

Notes
^^^^^
There is an interface named `IDisplacementPermutationCalculator` in the namespace `BriefFiniteElementNet.Mathh` which should do all 6 steps above or an output equivalent to output of step 6.

What we want to do is to solve :math:`F_t = K_t * U_t`\ where there are some extra equations. Maybe there are other ways to handle this, for example maybe QR decomposition. But this is a way also...

Example
^^^^^^^

Step 1:

:math:`\begin{bmatrix}0 &0 &1 &1&3& 0&	2 \\0 &0 &2 &6 &1 &0 &5 \\ 0&0&3&7&4&0&7\end{bmatrix} * \begin{bmatrix}x0\\x1\\x2\\x3\\x4\\x5\\x6\end{bmatrix} = \begin{bmatrix}3\\1\\4\end{bmatrix}`\

Step 2: None

Step 3:
[+0.00 +0.00 +1.00 +0.00 +4.25 +0.00 +1.75 | +4.25]
[+0.00 +0.00 +0.00 +1.00 -1.25 +0.00 +0.25 | -1.25]

Step 4: 2 by 7
[+0.00 +0.00 -1.00 +0.00 -4.25 +0.00 -1.75 | -4.25]
[+0.00 +0.00 +0.00 -1.00 +1.25 +0.00 -0.25 | +1.25]

Step 5: 7 by 7
[+1.00 +0.00 +0.00 +0.00 +0.00 +0.00 +0.00 | +0.00]
[+0.00 +1.00 +0.00 +0.00 +0.00 +0.00 +0.00 | +0.00]
[+0.00 +0.00 -1.00 +0.00 -4.25 +0.00 -1.75 | -4.25]
[+0.00 +0.00 +0.00 -1.00 +1.25 +0.00 -0.25 | +1.25]
[+0.00 +0.00 +0.00 +0.00 +1.00 +0.00 +0.00 | +0.00]
[+0.00 +0.00 +0.00 +0.00 +0.00 +1.00 +0.00 | +0.00]
[+0.00 +0.00 +0.00 +0.00 +0.00 +0.00 +1.00 | +0.00]
[+0.00 +0.00 +0.00 +0.00 +0.00 +0.00 +0.00 | +1.00]

step 6: result is 7 by 5
[+1.00 +0.00 +0.00 +0.00 +0.00 | +0.00]
[+0.00 +1.00 +0.00 +0.00 +0.00 | +0.00]
[+0.00 +0.00 -4.25 +0.00 -1.75 | -4.25]
[+0.00 +0.00 +1.25 +0.00 -0.25 | +1.25]
[+0.00 +0.00 +1.00 +0.00 +0.00 | +0.00]
[+0.00 +0.00 +0.00 +1.00 +0.00 | +0.00]
[+0.00 +0.00 +0.00 +0.00 +1.00 | +0.00]

:math:`P_U`\ is left side (this 7 by 5 matrix) and `R` is right side vector (1 by 7 matrix)
:math:`U_t = P_U * U_r`\ where t is Total, r is Reduced.