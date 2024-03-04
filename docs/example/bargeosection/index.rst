.. _BarElement-CrossSection-example:

Sections for BarElement
########################

To define the section of a `BarElement`, it can be done in two ways:

- UniformParametric1DSection
- UniformGeometric1DSection

UniformParametric1DSection
--------------------------

As in section :ref:`BarElement-CrossSection`, the term `Parametric` in `UniformParametric1DSection` means that properties are parametrically defined one by one. 

For example consider a section with:

+ Area of section is `A = 1 cm^2`
+ Second area moment around `y` axis is `Iy = 100 cm^4`
+ Second area moment around `z` axis is `Iz = 200 cm^4`
+ Torsion constant is `J = 0 mm^4`

.. code-block:: cs

   var section = new UniformParametric1DSection();
   //note: units are metric, which mean Area is in m^2, inertia is in m^4 and so on...
   section.A = 1e-4;//in m^2
   section.Iy = section.Iz = 1e-6;//100cm^4 equals to 0.000001 or 1e-6 in m^4
   section.J = 2e-6;//in m^4

   var bar = new BarElement();
   bar.CrossSection = section;

UniformGeometric1DSection
-------------------------

As in section :ref:`BarElement-CrossSection`, the term `Geometric` in `UniformGeometric1DSection` means that section is defined with it's geometric shape as a 2D polygon (list of 2d points). We just need to define the section geometry, and BFE will calculate the area, inertia and ... of section when it is needed.
There is a helper class `SectionHelper` which have some handy methods to create I shaped sections and rectangular. Coder can make it's own section while section can be defined as polygon in 2D dimention.

.. code-block:: cs

   var section = SectionHelper.GenerateISection(0,0,0,0,0,0,0,0,0,0,0,0,);//todo: set numbers in method, check the method
   bar.CrossSection = section;


Actially after you call `SectionHelper.GenerateISection` method, the section is made as a list of 2d points in the method, and result is returned.
You can check the source code for `SectionHelper.GenerateISection()` method to see what happens when it is called.