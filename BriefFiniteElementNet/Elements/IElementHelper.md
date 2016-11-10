There is a usual way to formthe stiffness matrix, and that is int(Bt*D*B*|J|). If any helper have other types of K matrix calculation then it should:
- return true from method DoesOverrideKMatrixCalculation
- return 


Every element have a local coordination system which is rotated global coordination system (like local system for frame or triangle element).