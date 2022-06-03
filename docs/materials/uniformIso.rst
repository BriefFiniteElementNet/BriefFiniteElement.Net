UniformIsotropicMaterial
========================

Overview
--------

inherited from ``BaseMaterial``, this represents a **unifrom** and **isotropic** material:

- **uniform** means material properties are not varying throught the element's body, or in every location of element material properties are identical.
- **isotropic** means having identical values of a property in all directions

YoungModulus
------------
``UniformIsotropicMaterial.YoungModulus`` represents a value defining the `Young's Modulus <https://en.wikipedia.org/wiki/Young%27s_modulus>`_ (aka. elastic modulus). The dimension is standard SI unit [Pas].

PoissonRatio
------------
``UniformIsotropicMaterial.PoissonRatio`` represents a value defining the `Poisson's ratio <https://en.wikipedia.org/wiki/Poisson%27s_ratio>`_. Poisson’s ratio is Dimensionless and has no SI unit.

Mass Density
------------
``UniformIsotropicMaterial.Rho`` represents a value defining the `Mass density <https://en.wikipedia.org/wiki/Mass_Density>`_. The dimension is standard SI unit [kg/m^3].

Damp Density
------------
``UniformIsotropicMaterial.Mu`` represents a value defining the `Damp density <https://en.wikipedia.org/wiki/Damp_Density>`_. The dimension is standard SI unit [TODO].


static CreateFromYoungPoisson()
-------------------------------
Creates a new instance of ``UniformIsotropicMaterial`` using Young's Modulus and Poisson's Ratio.

Example 
^^^^^^^
Create steel material with:

- Young's Modulus = 210 [GPa]
- Poisson's Ratio = 0.3

.. code-block:: cs

  var e = 210e9;//210 gpa
  var nu = 0.3;

  var steelMat = UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);
  
static CreateFromYoungShear()
-----------------------------
Creates a new instance of ``UniformIsotropicMaterial`` using Young's Modulus and Shear Modulus.
Poisson's ratio is calculated based on this formula: `G = E / (2*(1-nu))` then: `nu = e/(2*G) - 1` 

Example
^^^^^^^
Create steel material with:

- Young's Modulus = 210 [GPa]
- Shear Modulus = 79 [GPa]

.. code-block:: cs

  var e = 210e9;//210 gpa
  var g = 79e9;//79 gpa

  var steelMat = UniformIsotropicMaterial.CreateFromYoungShear(e, g);
  
static CreateFromShearPoisson()
-------------------------------
Creates a new instance of ``UniformIsotropicMaterial`` using Shear Modulus and Poisson's Ratio.
Elastic modulus is calculated based on this formula: `G = E / (2*(1-nu))` then: `E = G * (2*(1-nu))` 

Example
^^^^^^^
Create steel material with:

- Shear Modulus = 79 [GPa]
- Poisson's Ratio = 0.3

.. code-block:: cs

  var g = 79e9;//79 gpa
  var nu = 0.3;

  var steelMat = UniformIsotropicMaterial.CreateFromShearPoisson(g, nu);