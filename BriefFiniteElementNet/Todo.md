# Material class
Create a ```Material``` class with these properties:
- E (Young's modulus - in [Pa])
- Nu (Poisson's ratio)
- Rho (Specific Mass - in [kg/m3])
With a way that G can be setted or changed for the Material (either in constructor, methods or extension methods).


# ComputeB() & GetD()
Implement Element.ComputeB() and Element.GetD() for frame, shell and other elements to help to implement mass and damp matrices.
Create visualizer for showing deformed shape and coloring based on internal stress/force.
XML or any other plane text serialization for ISerializable s
Import Geometry from DXF file
Simple property editor for elements (only property editor not geometry editor)