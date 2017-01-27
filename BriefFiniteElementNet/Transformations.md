This standard is used in naming matrices about transformation of local to global coords in BFE:

There are two matrices named Transformation (T) and Lambda (λ) as this:

[xyz] = [λ]' * [XYZ]
[XYZ] = [λ] * [xyz]

where
	[xyz] is vector in LOCAL coordinates
	[XYZ] is vector in GLOBAL coordinates

Transformation matrix is takes as transopose of lambda:
[T] = [λ]'

X     λ_Xx  λ_Xy  λ_Xz    x
Y  =  λ_Yx  λ_Yy  λ_Yz    y
Z     λ_Zx  λ_Zy  λ_Zz    z

where λ_Ab is cosine of angle between A and b axis. 
Actually λ_x = {λ_Xx λ_Yx λ_Zx} 


Note that:
λ_Ab = λ_bA
λ_Ab ≠ λ_aB



Ref:
http://www.wind.civil.aau.dk/lecture/7sem_finite_element/lecture_notes/Lecture_6_7.pdf P53 for transformation of 3d frame element

http://www.oofem.org/resources/doc/elementlibmanual/html/elementlibmanualsu2.html section 2.2.2 Beam3d element, detailed info on transformation matrix of 3d beam