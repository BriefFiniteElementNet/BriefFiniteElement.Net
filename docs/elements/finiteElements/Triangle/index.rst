TriangleElement
==================

Overview
--------
A triangle element is referred to a 2D element, which only have dimension in two direction. It's features in an quick overview:

1. It can act as thin shell, thick shell, plate bending or membrane - see :ref:`TriangleElement-Behaviour` section.

.. |pic1| figure:: ../images/tri-full.png
   :align: center
   :width: 45%
   
   DoFs of ``TriangleElement`` acting as a Shell

.. |pic2| figure:: ../images/tri-membrane.png
   :align: center
   :width: 45%
   
   DoFs of ``TriangleElement`` acting as a Membrane
   
   
2. It can have a cross section - see :ref:`TriangleElement-CrossSection` section.
3. It can modeled as `PlaneStress <https://en.wikipedia.org/wiki/Plane_stress>`_ or `PlainStrain <https://en.wikipedia.org/wiki/Plane_stress>`_ - see :ref:`TriangleElement-MembraneFormulation` section.
4. It can have a material - see :ref:`TriangleElement-Material` section.
5. Several types of loads are possible to be apply on them - see :ref:`TriangleElement-ApplicableLoads` section.
6. It Does have a local coordination system, apart from global coordination system - see :ref:`TriangleElement-CoordinationSystems` section.
7. It is possible to find internal force of it - see :ref:`TriangleElement-InternalForce` section.


ref[1] "Development of Membrane, Plate and Flat Shell Elements in Java" by Kaushalkumar Kansara available from https://theses.lib.vt.edu/theses/available/etd-05142004-234133/unrestricted/Thesis.pdf

ref[2] "AN EXPLICIT FORMULATION FOR AN EFFICIENT TRIANGULAR PLATE-BENDING ELEMENT" by JEAN-LOUIS BATOZ


.. toctree::
    :titlesonly:
    :hidden:
    :maxdepth: 2

    behavs
    geos
    mats
    loads
    coords
    internalforces
    membraneFormula