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