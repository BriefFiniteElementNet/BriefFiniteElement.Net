# Intro
The FrameElement2Node object is a frame element in 3D system with arbitrary section.
Theorically a frame 3d element is combination of a truss element (carries axial load) and two beams (for moments in two directions).

# Section Geometry
There are two ways to specify the geometrical properties for section of frame element:
- Specifiy ```A, Ay, Az, Iy, Iz, J``` properties one by one on a FrameElement2Node instance
- Specifiy ```Geometry``` property on a FrameElement2Node instance and set ```UseOverridedProperties``` to false to let the element know that it should not use ```A, Ay, Az, Iy, Iz, J``` and should calculate these from Geometry which is a polygon

# Partial End Release
TODO

# Combine shape matrix and B matrix of truss + beam + torsion

kt = Sum(ki)

ki = int(Biᵀ*Di*Bi,x,0,l)

Bi = Bi,l * Pi

B = [B1 B2 B3 ... Bn]


D = [D1 0 ..... 0]
	[0  D2 .... 0]
	[...      . 0]
	[0  ....   Dn]

kt = int(Bᵀ*D*B,x,0,l)

# Shape Functions


## Beam
in iso coord system, with xi from -1 to 1:

N1(xi) = 1/4 (1-xi)^2 (2+xi) [slope(-1) = slope(1) = val(1) = 0, val(-1) = 1]
M1(xi) = L/8 (1-xi)^2 (xi+1) [slope(-1) = 1, slope(1) = val(1) = val(-1) = 0]

N2(xi) = 1/4 (1+xi)^2 (2-xi) [val(1) = 1, slope(-1) = slope(1) = val(-1) = 0]
M2(xi) = L/8 (1+xi)^2 (xi-1) [slope(1) = 1, slope(-1) = val(1) = val(-1) = 0]

N = [N1 M1 N2 M2]

xi = (2*x)/L - 1 -> ∂xi/∂x = 2/L -> J = [L/2]

B = ∂²N/∂x² = ∂²N/∂xi² * ∂xi²/∂x² = ∂²N/∂xi² * (∂xi/∂x)² =  ∂²N/∂xi² * (2/L)²

J = ∂x/∂xi 

K = int(Bᵀ * D * B * |J|);


## Truss

N = [1+xi 1-xi]/2

xi = (2*x)/L - 1 -> ∂xi/∂x = 2/L -> J = [L/2]

B = ∂N/∂x = ∂N/∂xi * ∂xi/∂x =  ∂N/∂xi * (2/L)

J = ∂x/∂xi 

K = int(Bᵀ * D * B * |J|);

Calculations are inside BehindTheScene folder in root folder

# Local and Global coordinate system
there is a local and a global system which are linearly transformable.
transformation formulas are taken from ref [3], p.107 (p.107 of PDF).

lambda = [CXx CYx CZx ;CXy CYy CZy ;CXz CYz CZz]

T = [Lambda 0 0; 0 Lambda 0;0 0 Lambda]

Kg= T' * Kl * T

#refs
ref[1]: "Numerical Modelling of Building Response to Tunnelling" by John Anthony Pickhaver, A Thesis submitted for the degree of Doctor of Philosophy at the University of Oxford.
ref[2]: "Finite Element Analysis" by S.S.BHAVIKATTI, A NEW AGE INTERNATIONAL PUBLISHERS
ref[3]: "MATLAB Codes for Finite Element Analysis" A.J.M. Ferreira, Springer