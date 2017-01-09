public static void ApplyTransformMatrix(Matrix matrix, Matrix transform)
---
Does apply the trasnformation to square input matrix. transform is 3x3 matrix, and input 
matrix is 3n x 3n, where n is an integer higher than zero.

Lambda * Local = Global


syms l11 l12 l13 l21 l22 l23 l31 l32 l33
syms k11 k12 k13 k21 k22 k23 k31 k32 k33

l = [l11 l12 l13; l21 l22 l23; l31 l32 l33]
k = [k11 k12 k13; k21 k22 k23; k31 k32 k33]

kt = transpose(l) * k * l;