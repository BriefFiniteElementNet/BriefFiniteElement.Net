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

B = 1 / l^2 * [6*xi, -(1-3*xi)*l, -6*xi, (1+3*xi)*l] (eq. 14.8 ref[2] p. 245 (258 of PDF))


#refs
ref[1]: "Numerical Modelling of Building Response to Tunnelling" by John Anthony Pickhaver, A Thesis submitted for the degree of Doctor of Philosophy at the University of Oxford.
ref[2]: "Finite Element Analysis" by S.S.BHAVIKATTI, A NEW AGE INTERNATIONAL PUBLISHERS