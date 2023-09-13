Finite Elements
***************

.. toctree::
    :titlesonly:
    :hidden:
    :maxdepth: 4

    Bar/index
    Triangle/index
    Tetrahedron/index
    frame-element/index
    triangle-flat-shell/index

There are several finite elements available in library. Each finite element does provide stiffness, mass and damp matrices. Their difference with special elements is that normal elements does provide stiffness, mass and damp matrices and usually have material, geometrical properties (like section on thickness). Finite elements are inherited from ``BriefFiniteElement.Elements.Element`` class.

Overview of Finite Elements available:
	- BarElement: A 1D, 2 noded element
	- TriangleElement: A 2D, 3 noded element
	- TetrahedronElement: A 3D, 4 noded element