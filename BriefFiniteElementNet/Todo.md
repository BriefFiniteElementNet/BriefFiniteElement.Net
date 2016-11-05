# Use ElementHelper instead of element itself for calculations
Use another class named ElementHelper for internal calculations of say B or D matrix for each type separately (like truss, beam, shaft, ...)

# ComputeB() & GetD()
Implement Element.ComputeB() and Element.GetD() for frame, shell and other elements to help to implement mass and damp matrices.
Create visualizer for showing deformed shape and coloring based on internal stress/force.
XML or any other plane text serialization for ISerializable s
Import Geometry from DXF file
Simple property editor for elements (only property editor, not geometry editor)
Separated class for geometry and material of elements. For example 
	For BarElement, BaseBarElemenetCrossSection class and BaseBarElementMaterial.
	For TriangleElement, BaseTriangleElementCrossSection and BaseTriangleElementMaterial.
	For TetrahedronElement, BaseTetrahedronElementMaterial.

```
BaseBarElemenetCrossSection
	GetCrossSectionPropertiesAt(double x,out A, out Iy, ....)//x in [0,1] range
	GetTurningPoints():double[]

UniformBarElementCrossSection
TaperedBarElementCrossSection


BaseBarElementMaterial
	GetMaterialPropertiesAt(double x, out E, out G)//x in [0,1] range
	GetTurningPoints():double[]
```
	
	
```
BaseTriangleElementCrossSection
	GetThicknessAt(double x, double y, out t)//x & y in [0,1] range

UniformBarElementCrossSection
TaperedBarElementCrossSection
	
BaseTriangleElementMaterial
	GetMaterialPropertiesAt(double x, double y, out E, out nu)//x & y in [0,1] range
```


```
BaseTetrahedronElementMaterial
	GetMaterialPropertiesAt (double x, double y, double z, out double E, out double nu)//x, y, z in [0,1] range
```

# Rename classes
Rename TriangleFlatShell to TriangleElement.
Rename FrameElement2Node to BarElement, add Behaviour property for either Truss, Shaft(Torsion), BeamY, BeamZ, Frame.
Remove TrussElement2Node

# Elements
Only these elements need to be:
BarElement, TriangleElement, TetrahedronElement, Spring.

# Change the FrameElement2Node architecture


# Element.GetEquivalentNodalLoads be moved to Load.GetEquivalentNodalLoads