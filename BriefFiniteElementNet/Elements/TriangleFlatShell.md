#Introduction
Flat shell is combination of DKT (discrete Kirchoff triangular) and a CST (constant triangle ) plus a drilling DoF.
This element is almost based on ref[1]

#DKT Element
Triangular Plate Bending Element Based on Discrete Kirchoff Theory. for more info see section 4.4, page [40-166] (49 of pdf) from ref[1]

#CST element
Constant Strain (and/or stress) triangular Element. for more info see section 3.5, page [15-166] (24 of pdf) from ref[1]

#Drilling DoF
Neither DKT and CST are giving stiffness in local RZ (rotation about Z) direction. so we manually add drilling DoF to it. 
For more info on formulation see eq. 5.2, page [71 of 166] (page 80 of pdf) from ref[1]

# Local Coordination System of Element and Coordination Transformation
There is a local coordination system, which all nodes are in X-Y plane of it. Note that this is not isoparametric system, it is linearly transformed from global system.
For more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of ref[1].
This transformation is used for both membrane and plate bending.

#Internal Forces

note that result is in local coordination system. result of this is represented as instance of BriefFiniteElementNet.FlatShellStressTensor() which consists of a tensor for membrane element and one for plate bending element.

##DKT Element
	Internal force can be found from eq. 17 (and fig 3) from ref[2] 
	note that result is in local coordination system. result of this is represented as instance of BriefFiniteElementNet.PlateBendingStressTensor() struct with same naming

##CST Element
	Usual D*B*U formula is used for this Element.
	note that result is in local coordination system. result of this is represented as instance of BriefFiniteElementNet.MembraneStressTensor() struct with same naming


#Equivalent Nodal Loads
for both CST and DKT element, lumped approach is used where element is under uniform surface load.
Other type of loads (like loads on one edge of triangle) is not implemented yet.

#Mass and Damping matrices
```throw new NotImplementedException();```

#Refs
[1] "Development of Membrane, Plate and Flat Shell Elements in Java" by Kaushalkumar Kansara
	available (at least at the time) from https://theses.lib.vt.edu/theses/available/etd-05142004-234133/unrestricted/Thesis.pdf

[2] "AN EXPLICIT FORMULATION FOR AN EFFICIENT TRIANGULAR PLATE-BENDING ELEMENT" by JEAN-LOUIS BATOZ

#Further reading:
'A STUDY OF THREE-NODE TRIANGULAR PLATE BENDING ELEMENTS' by JEAN-LOUIS BATOZ, KLAUS-JORGEN BATHE AND LEE-WING HO