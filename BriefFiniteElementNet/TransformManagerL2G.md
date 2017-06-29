

This standard is used in naming matrices about transformation of local to global coords in BFE:

There are two matrices named Transformation (T) and Lambda (λ) as this:

[xyz] = [λ]' * [XYZ]
[XYZ] = [λ] * [xyz]

[xyz] = [T] * [XYZ]
[XYZ] = [T]' * [xyz]

where
	[xyz] is vector in LOCAL coordinates
	[XYZ] is vector in GLOBAL coordinates

Transformation matrix is taken as transopose of lambda:
[T] = [λ]'

X     λ_Xx  λ_Xy  λ_Xz    x
Y  =  λ_Yx  λ_Yy  λ_Yz    y
Z     λ_Zx  λ_Zy  λ_Zz    z


      λ_Xx  λ_Xy  λ_Xz 
λ  =  λ_Yx  λ_Yy  λ_Yz 
      λ_Zx  λ_Zy  λ_Zz 


      λ_Xx  λ_Yx  λ_Zx
T  =  λ_Xy  λ_Yy  λ_Zy  =  λ'
      λ_Xz  λ_Yz  λ_Zz


Where λ_Ab is Cosine of angle between A and b axis.


 λ_x = {λ_Xx λ_Yx λ_Zx} = Direction Cosine of i vector with respect to global system (i is unit vector in x direction).
 λ_y = {λ_Xy λ_Yy λ_Zy} = Direction Cosine of j vector with respect to global system (j is unit vector in y direction).
 λ_z = {λ_Xz λ_Yz λ_Zz} = Direction Cosine of k vector with respect to global system (k is unit vector in z direction).

Note that:
λ_Ab = λ_bA
λ_Ab ≠ λ_aB


purpose of making this class is to achieve simpler managmenet of transformation of vectors, points ans matrixes such as stiffness, mass and damp matrix.

Transforming vector and points
===
as shown above

Transforming matrices
===
Local to Global using λ:
λ K λ'

Local to Global using T:
T' K T

Global to Local using λ:

Global to Local using T:


Improving Performance of Transforming matrices
====
Transforming a (for example) 12x12 matrix named ```A```, is like this:

```
A = [a11,a12,a13,a14;a21,a22,a23,a24;a31,a32,a33,a34;a41,a42,a43,a44]
```

Where ```aij``` is a 3x3 matrix itself. Applygin transformation to this matrix is:

T_a' * A * T_a

where T_a is:
```
T_a = [T,0,0,0;0,T,0,0;0,0,T,0;0,0,0,T;] 
```
where T is a 3x3 matrix defined according to previous section in this document.

this does costs a multiplication of 12x12 matrix and then multiplication of 12x12 to another 12x12, in total takes two 12x12 matrix multiplications. 
For multiplication of two ```n x n``` matrixes every member should be calculated with 12 x multiply and 11 summation (member at [i,j] at result is dot product of i'th row of first matrix with j'th row of second matrix).
Calculating the multiply of two 12x12 matrixes, from mathematic operation preview needs 12 x multiply and 11 summation for each member, and count of all members is 12x12. 
for multiplying two 12x12 matrixes, in total it will be ```12 x 12 x 12 = 1728``` times multiply operation and ```12 x 12 x 11 = 1584``` summation operation.
Computer does this very fast for say 12x12, but if dimention increased this will takes time progressively. 


We did formed the T_a matrix, and then do the ```T_a' * A * T_a```, other way that is more complex but higher performance is to inline multiplication:

T_a' * A * T_a = A = [T'*a11*T,T'*a12*T,T'*a13*T,T'*a14*T;T'*a21*T,T'*a22*T,T'*a23*T,T'*a24*T;T'*a31*T,T'*a32*T,T'*a33*T,T'*a34*T;T'*a41*T,T'*a42*T,T'*a43*T,T'*a44*T]

for our example that is 12x12, it will cost 16 times of multiplying three 3x3 matrixes, each multiplication need ```3x3x3``` times of multiply operation, and ```3x3x2`` of summation. 
in total it will be ```16x3x3x3 = 432``` times multiply operation and ```16x3x3x2 =288``` times plus operator which is somewhere about 18% to 25% of original method on the paper. 
In reality for large matrices (say 30x30) this type


Ref:
http://www.wind.civil.aau.dk/lecture/7sem_finite_element/lecture_notes/Lecture_6_7.pdf P53 for transformation of 3d frame element

http://www.oofem.org/resources/doc/elementlibmanual/html/elementlibmanualsu2.html section 2.2.2 Beam3d element, detailed info on transformation matrix of 3d beam

ref[3]: http://www.starlino.com/dcm_tutorial.html