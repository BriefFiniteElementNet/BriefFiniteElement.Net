#Formulation

Beam with Shear deformation
formulation taken from ref[3], it gave same result as ref[2].
Probably P.81, part 2.9 EXACT TWO-NODED TIMOSHENKO BEAM ELEMENT of ref[1] also can be used
Also p.116 (11/40) of ref[4] can be used probably

Dga = T . D (6.23 - ref[4])
K = T' Kga T (6.24 - ref[4])

Kga = Bga' E Bga
K = B' E B

=> B = Bga * T

Bga= 1/l^2 [N"1 N"2 N"3 N"4 N"5]   (6.18 - ref[4])
N5 = N2 + N4 (6.12 - ref[4])
N"1 (xb) = −6 + 12 xb (B.27) , xb=X/L
xi = 2 xb - 1 => xb = (xi + 1) / 2

N"1 = 6 * xi			(help of (B.27)
N"2 = L * (3 xi - 1)	(help of (B.28)
N"3 = -6 * xi			(help of (B.29)
N"4 = L * (3 xi + 1)	(help of (B.30)
N"5 = L * 6 xi


#Example converting distributed loads to end moments with shape etc

https://www.ethz.ch/content/dam/ethz/special-interest/baug/ibk/structural-mechanics-dam/education/femI/Numerical_Integration.pdf

octave
--
syms xi l c
A = eye(4)
B = [-2*c -c*l 2*c c*l]
T = [A;B]

n1 = 6 * xi
n2 = L * (3*xi - 1)
n3 = -6 * xi
n4 = L * (3*xi + 1)
n5 = L * 6 * xi

bga = [n1 n2 n3 n4 n5]
b = bga * T

ans:
-12⋅c⋅l⋅ξ + 6⋅ξ  , - 6⋅c⋅l⋅ξ + l⋅(3⋅ξ - 1) , 12⋅c⋅l⋅ξ - 6⋅ξ  , 6⋅c⋅l⋅ξ + l⋅(3⋅ξ + 1)

-12*c*l*xi + 6*xi  , - 6*c*l*xi + l*(3*xi - 1) , 12*c*l*xi - 6*xi  , 6*c*l*xi + l*(3*xi + 1)




#Refs
ref[1]: Structural Analysis with the Finite Element Method Linear Statics - Volume 2. Beams, Plates and Shells
ref[2]: Matrix Structural Analysis (MSA) by HOSSEIN RAHAMI
ref[3]: http://people.duke.edu/~hpgavin/cee541/StructuralElements.pdf
ref[4]: https://ora.ox.ac.uk/objects/uuid:570ebfd5-ec3e-4a0a-a559-d1efddde9e20/datastreams/THESIS03
ref[5]: https://ora.ox.ac.uk/objects/uuid:570ebfd5-ec3e-4a0a-a559-d1efddde9e20/datastreams/THESIS09
ref[6]: Stochastic Structural Dynamics: Application of Finite Element Methods, Cho W. S. To, ISBN: 978-1-118-34235-0
ref[7]: http://marketcode.ir/Code/ShowCode/140/ (needs registration - pdf)

Damp Matrix
===
p.31 of ref[3]
p.52 - eq.84_1 ref[7]

Mass Matrix
===
p.9 of ref[6]
p.52 - eq.84_1 ref[7]

Coordination stranformation
===
Lambda matrix:

[xyz] are vector in element's local coordination system
[XYZ] are vector in global coordination system