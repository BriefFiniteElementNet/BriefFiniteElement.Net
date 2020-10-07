# Introduction
Flat shell is combination of DKT (discrete Kirchoff triangular) and a CST (constant triangle ) plus a drilling DoF.
This element is almost based on ref[1]

# Flat Shell Behavior and using TriangleFlatShell as CST element or as DKT element
Flat shell can have three possible behaviors (```FlatShellBehaviour``` enum)
- FlatShellBehaviour.ThinShell : combination of plate bending (Discrete Kirchoff) and membrane
- FlatShellBehaviour.ThinPlate : only plate bending (Discrete Kirchoff)
- FlatShellBehaviour.Membrane : only membrane

## use as CST Element
set ```TriangleFlatShell.Behavior = FlatShellBehaviour.Membrane ``` also ```TriangleFlatShell.AddDrillingDof = false```

## use as DKT Element
set ```TriangleFlatShell.Behavior = FlatShellBehaviour.ThinPlate ``` also ```TriangleFlatShell.AddDrillingDof = false```

# DKT Element Details
Triangular Plate Bending Element Based on Discrete Kirchoff Theory. for more info see section 4.4, page [40-166] (49 of pdf) from ref[1]

# CST element Details
Constant Strain (and stress) triangular Element. for more info see section 3.5, page [15-166] (24 of pdf) from ref[1]
Default is Plane Stress, but can be changed using ```TriangleFlatShell.MembraneFormulationType```.

# Drilling DoF for flat shell element
Neither DKT and CST are giving stiffness in local RZ (rotation about Z) direction. so we manually add drilling DoF to it. 
For more info on formulation see eq. 5.2, page [71 of 166] (page 80 of pdf) from ref[1]
Use ```TriangleFlatShell.AddDrillingDof``` turn this feature on or off. By default it is true.

# Local Coordination System and Transformation
There is a local coordination system, which all nodes are in X-Y plane of it. Note that this is not isoparametric system, it is linearly transformed from global system.
For more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of ref[1].
This transformation is used for both membrane and plate bending.

# Internal Forces

note that result is in local coordination system. result of this is represented as instance of BriefFiniteElementNet.FlatShellStressTensor() which consists of a tensor for membrane element and one for plate bending element.

## DKT Element internal force
Internal force can be found from eq. 17 (and fig 3) from ref[2] 
note that result is in local coordination system. result of this is represented as instance of BriefFiniteElementNet.PlateBendingStressTensor() struct with same naming

## CST Element internal force
Usual D*B*U formula is used for this Element.
note that result is in local coordination system. result of this is represented as instance of BriefFiniteElementNet.MembraneStressTensor() struct with same naming

# Equivalent Nodal Loads
for both CST and DKT element, lumped approach is used where element is under uniform surface load.
Other type of loads (like loads on one edge of triangle) is not implemented yet.

# Mass and Damping matrices
```throw new NotImplementedException();```

# References
ref[1] "Development of Membrane, Plate and Flat Shell Elements in Java" by Kaushalkumar Kansara available from https://theses.lib.vt.edu/theses/available/etd-05142004-234133/unrestricted/Thesis.pdf

ref[2] "AN EXPLICIT FORMULATION FOR AN EFFICIENT TRIANGULAR PLATE-BENDING ELEMENT" by JEAN-LOUIS BATOZ

# Further reading:
'A STUDY OF THREE-NODE TRIANGULAR PLATE BENDING ELEMENTS' by JEAN-LOUIS BATOZ, KLAUS-JORGEN BATHE AND LEE-WING HO