.. _ConcentratedLoad:

ConcentratedLoad
================

``ConcentratedLoad`` in namespace ``BriefFiniteElementNet.Loads``, is a concentrated load which can apply on 1D (like ``BarElement``), 2D (like ``TriangleElement``) or 3D (like ``TetrahedronElement``) elements.

Force
-----
``ConcentratedLoad.Force`` which is a ``Force`` property, defines the amount of force that is applied on the element body.

IsoPoint
--------

The iso-parametric location of force inside element's body


CoordinationSystem
------------------
``ConcentratedLoad.CoordinationSystem`` which is a enum typed property, defines the coordination system of load. It can only have two different values of ``CoordinationSystem.Global`` or ``CoordinationSystem.Local``:

	- ``CoordinationSystem.Global``: The load is assumed in global coordination system
	- ``CoordinationSystem.Local``: The load is assumed in local coordination system of element that load is applied to (each element type have different local coordination system which is stated in appropriated section).

Look at :ref:`element-load-coordination-system` for more information on how to use.
