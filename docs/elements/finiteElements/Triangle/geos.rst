.. _TriangleElement-CrossSection:

Cross Section
-------------
``TriangleElement`` is modelled as a 2D element, and it needs to have thickness values of it's cross section. ``TriangleElement.Section`` does define a cross section for ``TriangleElement``.
The type ``Base2DSection`` is base class that is used for defining a cross section for triangle element. This class is a general class which can gives every information of section's geometric properties at specific location of surface of element. In this case ``Base2DSection`` gets the isometric location of any arbitrary point and returns the thickness of section at that point.
All other cross sections of triangle element are inherited from ``Base2DSection`` class.

UniformParametric2DSection
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Inherited from ``Base2DSection``, defines a uniform section for the ``TriangleElement``. Uniform section means that section thickness or probably other geometric properties does not change along the surface of trignale.
Parametric means that properties are parametrically defined one by one. for example if we have a section, with thickness = 1 cm then:

.. code-block:: cs

   var section = new Base2DSection();
   section.Thickness = 0.01;
   
   var tri = new TriangleElement();
   tri.CrossSection = section;