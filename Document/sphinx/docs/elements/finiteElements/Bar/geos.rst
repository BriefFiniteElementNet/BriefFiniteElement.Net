.. _BarElement-CrossSection:

Cross Section
-------------
``BarElement`` is modelled as a 1D element, and it needs to have geometrical values of it's cross section (like A, Iy, Iz, etc.). ``BarElement.CrossSection`` does define a cross section for ``BarElement``.
The type ``Base1DSection`` is base class that is used for defining a cross section for bar element. This class is a general class which can gives every information of section's geometric properties at specific location of length of element.
All other cross sections of bar element are inherited from ``Base1DSection`` class.

UniformParametric1DSection
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Inherited from ``Base1DSection``, defines a uniform section for the ``BarElement``. Uniform section means that section does not change along the length of bar.
Parametric means that properties are parametrically defined one by one. 
for example if we have a circular section, with Iy = Iz = J/2 = 1e-6 m^4, A = 2e-6 m^4 then:

.. code-block:: cs

   var section = new UniformParametric1DSection();
   section.A = 1e-4;
   section.Iy = section.Iz = 1e-6;
   section.J = 2e-6;
   
   var bar = new BarElement();
   bar.CrossSection = section;

.. hint:: Note that two properties ``Ay`` and ``Az`` of ``UniformParametric1DSection``, are about shear areas of section and their value will not be used unless BarElement have one of  ```TimoshenkoBeam``` behaviours.

UniformGeometric1DSection
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Inherited from ``Base1DSection``, defines a uniform section for the ``BarElement``. Uniform section means that section does not change along the length of bar.
Geometrix means that section properties are defined as polygon.

Important Note: In `UniformGeometric1DSection` it is only possible to define one polygon. if polygon contains nested holes etc., then it should convert to one polygon. See :ref:`BarElement-CrossSection-example`

Important Note: The way that geoemtric properties are calculated for section is defined here: (https://en.wikipedia.org/wiki/Second_moment_of_area#Any_polygon). Maybe this method be not applicable to thin walled section.

Important Note: there is no analystical solution for findind torsional constant J for noncircular sections, in those cases user must set `UniformGeometric1DSection.JOverrided` property to correct value, otherwise polar area moment will be used (https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues/38)

Important Note: The geometric section is defined in Y-Z plane of local coordination system of element. the X axis in local coordination system is the beam length direction.