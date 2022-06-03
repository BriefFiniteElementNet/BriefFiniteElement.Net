.. _BarElement-CrossSection-example:

Sections for BarElement
########################

Example 1
=========

Consider example below, a cantilever beam with fixes start node and free end node, under defined loads.

image todo

The section is I section with :

+ Width of `w = 10 cm` and 
+ Height of `h = 15 cm`
+ Flange thickness of `tf = 5 mm`
+ Web thickness of `tw = 5 mm`
+ Material is steel with elastic module of `E = 210 GPa` and Poisson's ratio of `nu = 0.3`.

For defining the element itself we should do:

TODO

Till here the section for element is not defined. We can define the section in two ways:

- UniformParametric1DSection
- UniformGeometric1DSection

** UniformParametric1DSection: **
If we want to define section with `UniformParametric1DSection`, as defined in section :ref:`BarElement-CrossSection`, the parametric means that properties are parametrically defined one by one. for this example assuming we know that:

+ Area of section is `A = 0 mm^2`
+ Second area moment around `y` axis is `Iy = 0 mm^4`
+ Second area moment around `z` axis is `Iz = 0 mm^4`
+ Torsion constant is `J = 0 mm^4`

.. code-block:: cs

   var section = new UniformParametric1DSection();
   section.A = 1e-4;
   section.Iy = section.Iz = 1e-6;
   section.J = 2e-6;
   
   var bar = new BarElement();
   bar.CrossSection = section;

** UniformGeometric1DSection: **
If we want to define section with `UniformGeometric1DSection`, as defined in section :ref:`BarElement-CrossSection`, the geometric means that properties are parametrically defined one by one. For this example there is no need to know the properties of section, BFE will calculate them. We just need to define the section geometry:

.. code-block:: cs

   var section = SectionHelper.GenerateISection(0,0,0,0,0,0,0,00,0,0,0,0,);
   bar.CrossSection = section;

