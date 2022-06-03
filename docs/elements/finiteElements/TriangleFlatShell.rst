Intro
=====
Triangle element is only area or surface element in BFE. It is consist of a CST (Constant Stress (and Strain) Triangle) element for in plane loads, and a DKT (Discrete Kirchhoff Triangle) element for carying out off plane loads and moments and also an additional stiffness for drilling DoF.

# DoFs
Triangle element in BFE does have three nodes, each one for a corner of triangle in 3D space. Each node have 6 DoFs in global coordination system.

# Properties
The properties of Triangle element:

## Mechanical Properties:
	ElasticModulus: Elastic Modulus measures in [Pascal] or [Pas]
	PoissonRatio: poisson ratio, in [0,0.5] range, dimension less
	
## Geometrical Properties:
	Thickness: Thickness of element, measures in [m] unit
	
## Modelling properties:
	Behavior: Determines the behaviour of element, three possible values: (by default is ThinShell)
		FlatShellBehaviour.ThinPlate: based on discrete Kirchhoff theory, only plate bending behaviour
		FlatShellBehaviour.Membrane: The membrane, only in-plane forces, no moments. Only membrane behavior.
		FlatShellBehaviour.ThinShell: The thin shell, based on discrete Kirchhoff theory, combination of Plate and Membrane
		
	MembraneFormulationType: Determines the formulation type of membrane. only if Behaviour is either Membrane or ThinShell, then this property does taken into account. Two possible values:
		MembraneFormulation.PlaneStress: plane stress situation
		MembraneFormulation.PlaneStrain: plane strain situation
	
	AddDrillingDof: neither DKT and CST elements does have stiffness in rotation DOF about element's local Z axis. 
	If this is set to true, based on solution provided in ref[1], an additional stiffness in rotation DoF about local Z axis is added to element.
	
## Notes:

	
	
# References
ref[1] "Development of Membrane, Plate and Flat Shell Elements in Java" by Kaushalkumar Kansara available from https://theses.lib.vt.edu/theses/available/etd-05142004-234133/unrestricted/Thesis.pdf
ref[2] "AN EXPLICIT FORMULATION FOR AN EFFICIENT TRIANGULAR PLATE-BENDING ELEMENT" by JEAN-LOUIS BATOZ
