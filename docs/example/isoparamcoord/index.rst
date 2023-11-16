.. _BarElement-Isoparam-example:

Iso Parametric Coordination System Of Elements Example
######################################################

Apart from local and global coordination systems for elements, there is another system based on isoparametric formulation/representation, which is used extensively in finite element method. In BFE also in many places instead of local coordinate system, the iso parametric coordination is used.

For one dimentional objects, usually the greek alphabet ξ is used to represent iso coord of element. it pronounces as `ksi` or simply will call it `xi` next. note that `xi` is different with `x` where we usually `xi` for isoparametric coord, but will use `x` for local coord of bar element.

TODO: for 2 and 3 D models.


Iso Parametric Coordination system for BarElement with two nodes
*****************************************************************

.. figure:: ../images/bar-iso-coord.png
   :align: center

Based on 
   
At the beginning point of the element, where `x=0` the iso parametric coordinate is `ξ=-1`

At the central point of the element, where `x=L/2`, and L is length of elements, the iso parametric coordinate is `ξ=0`

At the end point of the element, where `x=L`, and L is length of elements, the iso parametric coordinate is `ξ=1`

In bar element with two nodes the relation between isoparamtric `ξ` coordinate and local `x` coordinate is:

``x = (ξ + 1)*L/2``
and subsequently

``ξ = (2*x-L)/L``

From code you can convert iso to local coords like this:

``
var x = 1.0 + 0.0000001;//need to find internal force at a little after x = 1.0 m
var xi = e0.LocalCoordsToIsoCoords(x);
``