Intro
=====
Usually a Frame Element in referred to an element which is consisted of two beam elements in two bending directions, a truss element for axial load carying and a rod to carry the torsion load.


# DoFs
Frame element in BFE does have two nodes, one for starting node and one for end node. Each node have 6 DoFs in global coordination system.

# Properties
The properties of frame element splits into three categories:

## Mechanical Properties:
- FrameElement2Node.E: Elastic modulus of frame element, in [Pas] unit.
- FrameElement2Node.G: Shear modulus of frame element, in [Pas] unit. measured with a formula regarding elastic modulus as poison's ration of material.
## Geometrical Properties:
There are two different ways to define geometrical properties of a FrameElement2Node, note that if you define geometrical properties one by one, set ```FrameElement2Node.UseOverridedProperties = true```:

	- Define each geometrical property one by one:
		- FrameElement2Node.A: area of element's cross section, in [m]^2 dimension
		- FrameElement2Node.Ay: shear area of element's cross section, in local y direction, in [m]^2 dimension. only used if ```ConsiderShearDeformation == true```
		- FrameElement2Node.Az: shear area of element's cross section, in local z direction, in [m]^2 dimension. only used if ```ConsiderShearDeformation == true```
		- FrameElement2Node.Iy: The Second Moment of Area of section regard to Z axis. in [m]^4 dimension.
		- FrameElement2Node.Iz: The Second Moment of Area of section regard to Y axis. in [m]^4 dimension.
		- FrameElement2Node.J: The polar moment of inertial. in [m]^4 dimension.
		
	- Define a polygon for FrameElement2Node.Geometry
		- FrameElement2Node.Geometry: A polygon, in Y-Z 2d space, which describes the geometry of cross section in Y-Z coordinate system. Note: Ay and Az are note calculated from Geometry
		

## Modelling Properties:
- FrameElement2Node.WebRotation: Determines the additional rotation amount of element around its X direction in clockwise (TODO: or ccw?) direction.
- FrameElement2Node.UseOverridedProperties: determines whether calculations should calculates geometrical properties from FrameElement2Node.Geometry, or from other properties (like A, Iy, Iz, etc).
- FrameElement2Node.HingedAtStart: determines whether connection of beam at start point is hinged, or it is fixed. This is different than fixing DoFs in start node.
- FrameElement2Node.HingedAtEnd: determines whether connection of beam at end point is hinged, or it is fixed. This is different than fixing DoFs in end node.
