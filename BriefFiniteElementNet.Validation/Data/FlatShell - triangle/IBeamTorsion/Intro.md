This example is about solving same model as "Test Example 13" in Thesis pdf file from Kaushalkumar Kansara with abaqus (with STRI3 element).

![mesh3](https://raw.githubusercontent.com/BriefFiniteElementNet/BriefFiniteElement.Net/master/BriefFiniteElementNet.Validation/Data/FlatShell%20-%20triangle/IBeamTorsion/example%2013.png)

A  cantilever  I  –  beam  is  analyzed. The length of the I – beam is 40 in., the height is 5 in. and the flange widths are 10 in.  A  load  of  1.6  kips  is  applied  at  the  top  and  bottom  flanges  of  the  I  -  beam  in  two  opposite  directions  as  shown  in  Fig.  7.16.  This  example  is  one  of  the  verification examples  presented  in  Alladin  v.  1.0.  A  finite  element  model  of  the  cantilever  beam  consists of 96 three node triangular shell elements. 

Geometric Data:

- Length L = 40.0 in.
- Width = 10.0 in.
- Height h = 5.0 in.
- Thickness t = 0.25 in.

Material Properties:

- Modulus of elasticity E = 10000 ksi.
- Poisson’s ratio ν = 0.3

Boundary Conditions:

- One end of the cantilever is fixed.

Loading:
- A  concentrated  load  of  1.6  kips  is  applied  at  the  top  and  bottom  of  the  flange  in  opposite directions as shown in Fig. 7.16.

Abaqus model (node numbers are differently named)

![mesh1](https://raw.githubusercontent.com/BriefFiniteElementNet/BriefFiniteElement.Net/master/BriefFiniteElementNet.Validation/Data/FlatShell%20-%20triangle/IBeamTorsion/mesh1.jpg)
![mesh2](https://raw.githubusercontent.com/BriefFiniteElementNet/BriefFiniteElement.Net/master/BriefFiniteElementNet.Validation/Data/FlatShell%20-%20triangle/IBeamTorsion/mesh2.jpg)
![mesh3](https://raw.githubusercontent.com/BriefFiniteElementNet/BriefFiniteElement.Net/master/BriefFiniteElementNet.Validation/Data/FlatShell%20-%20triangle/IBeamTorsion/mesh3.jpg)