.. _BarElement-Material:

Material
--------
``BarElement.Material`` property defines a material for this element.
the type ``BaseMaterial`` is base class that is used for defining a material for a finite element. This class is a general class which can gives every information of section's materials at specific location of length of element.

UniformIsotropicMaterial
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
This class is inherited from ``BaseMaterial`` and defines a uniform material for the finite elements. Uniform material means that material does not change along the length of bar.
Parametric means that properties are parametrically defined.
for example if we have a steel material, with Elastic module = 210 GPa = 210e9 Pa, and Poisson's ratio = 0.3 then:

.. code-block:: cs
	
	using BriefFiniteElement.Elements
	using BriefFiniteElement.Materials
	
	var material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);
	var bar = new BarElement();

    bar.Material = material;