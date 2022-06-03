.. _TriangleElement-Material:

Material
--------
``TriangleElement.Material`` property defines a material for this element.
the type ``BaseTriangleMaterial`` is base class that is used for defining a material for bar element. This class is a general class which can gives every information of section's materials at specific location of surface of element.
 All other materials of triangle section are inherited from ``BaseTriangleMaterial`` class.

UniformParametricTriangleMaterial
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
This class is inherited from ``BaseTriangleMaterial`` and defines a uniform material for the triangle element. Uniform material means that material does not change along the surface of triangle.
Parametric means that properties are parametrically defined (like ```UniformParametricTriangleMaterial.E``` and ```UniformParametricTriangleMaterial.G```).
for example if we have a steel material, with E = 210 GPa, G = 80 GPa then:

.. code-block:: cs

   var steelMaterial = new UniformParametricTriangleMaterial();
   steelMaterial.E = 210e9;//210 * 10^9 Pas
   steelMaterial.G = 80e9;//80 * 10^9 Pas
   
   var tri = new TriangleElement();
   tri.Material = steelMaterial;