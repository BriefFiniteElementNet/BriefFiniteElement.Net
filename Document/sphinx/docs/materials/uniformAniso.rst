UniformAnisotropicMaterial
==========================

Overview
--------
inherited from ``BaseMaterial``, this represents a **unifrom** and **anisotropic** material:

- **uniform** means material properties are not varying throught the element's body, or in every location of element material properties are identical.
- **anisotropic** means having different mechanical properties in different directions

Properties
------------
There are 9 properties with this class:

  - **Ex**: Young's Modulus in element's local X direction
  - **Ey**: Young's Modulus in element's local Y direction
  - **Ez**: Young's Modulus in element's local Z direction
  - **NuXy**, **NuYx**: Poisson's Ratio
  - **NuYz**, **NuZy**: Poisson's Ratio
  - **NuZx**, **NuXz**: Poisson's Ratio