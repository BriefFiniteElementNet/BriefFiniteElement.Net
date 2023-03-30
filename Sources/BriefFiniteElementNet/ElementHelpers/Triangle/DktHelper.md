represents a discrete Kirchoff triangular element with constant thickness.

implementations:
https://github.com/eudoxos/woodem/blob/9e232d3a737cd3095a7c1eaa82e9e28829af97ab/pkg/fem/Membrane.cpp
https://github.com/ahojukka5/FEMTools/blob/dfecb12d445feeac94ea2e8bcb2b6cc785fbaa72/TeeFEM/teefem/models/dkt.py (cool one, also have geometric B and stiffness matrix)
https://github.com/Micket/oofem/blob/b5fc923f27ccfea47095fd62c8f3209f025224bd/src/sm/Elements/Plates/dkt.C
https://github.com/IvanAssing/FEM-Shell/blob/5700101c97b90230646f82167e7528cae5d875b4/elementsdkt.cpp
https://github.com/jiamingkang/bles/blob/939394f241f8e01a296d38ff52feac246793cbc9/CHak2DPlate3.cpp
https://github.com/cescjf/locistream-fsi-coupling/blob/610a5b50933333bf6f43bc0b0ec19cc849dcb543/src/FSI/nlams_dkt_shape.f90


# GetLocalInternalForce()


Gets the specified internal force at specified location. element is 2D but `location.Lambda` is used in third dimensions.
Triangle element can have bending behaviour, it means there is internal moment and stress is not constant through the thickness of the shell. so at the buttom of plate where Lambda=-1, at the center where Lambda=0 and at the top where Lambda=1, there are different cauchy stresses.


refs:
    [1] "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web
    [2] "A STUDY OF THREE-NODE TRIANGULAR PLATE BENDING ELEMENTS" by JEAN-LOUIS BATOZ,KLAUS-JORGEN BATHE and LEE-WING HO
    [3] "Membrane element" https://woodem.org/theory/membrane-element.html



Hermite functions


``` Octave

syms a b c d e f g h i
syms xi eta

A = [a, b ,c ,d ,e ,f ,g ,h ,i]
XI = transpose( [1 ,xi ,eta, xi^2, xi*eta, eta^2, xi^2*eta^2 ,xi*eta^2 ,xi^2*eta])
f0= A * XI
f0xi=diff(f0,xi)
f0eta=diff(f0,eta)

e1=1-subs(f0,[xi,eta],[0,0]);
e2=subs(f0,[xi,eta],[0,0]);
e3=subs(f0,[xi,eta],[0,0]);

e4=subs(f0xi,[xi,eta],[0,0]);
e5=subs(f0xi,[xi,eta],[1,0]);
e6=subs(f0xi,[xi,eta],[0,1]);

e7=subs(f0eta,[xi,eta],[0,0]);
e8=subs(f0eta,[xi,eta],[1,0]);
e9=subs(f0eta,[xi,eta],[0,1]);

ee=[e1 e2 e3 e4 e5 e6 e7 e8 e9]

Q = expand(transpose(ee)*(1./A))

FIN=(zeros(9,9).+a .* 0) # define FIN as symbolic matrix!

for cnt=1:9
	FIN(:,cnt)=subs(expand(Q(:,cnt)),A(cnt),inf)
endfor