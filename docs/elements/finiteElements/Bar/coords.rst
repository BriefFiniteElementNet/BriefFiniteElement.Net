.. _BarElement-CoordinationSystems:

Coordination Systems
--------------------

Local Coordination System
^^^^^^^^^^^^^^^^^^^^^^^^^

Local coordination system for ``BarElement`` has tree axis that we name ``x'``, ``y'`` and ``z'``. 

TODO with images

Relation of global and local system
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

"The global axes are brought to coinside with the local member axes by seauence of rotation about y, z and x axes respectively. This is refered to an y-z-x transformation." ref[0].

Imagine a bar element with start node ``N1`` located at ``(x1, y1, z1)`` and end node ``N2`` located at ``(x2,y2,z2)``. Four steps are needed to find the directions of the local axis x'-y'-z':

- Step 1:

Move the element in a way that ``N1`` be placed at origins of global system.
TODO: Image

- Step 2:

Rotate global system about global Y axis rotated X axis goes under element length (shown as β in image below). Note that if element is vertical (e.g. x1 = x2 and y1 = y2 and z1 ≠ z2) no need to do this step.
TODO: Image

- Step 3:

Rotate the system from previous step about it's Z axis in a way that X axis go exactly through same direction of element's length (shown as γ in image below).
TODO: Image

- Step 4:

If element have any custom web rotation α, do rotate system about it's X axis by α:
TODO: Image

the result system is local system of bar element.

code for transforming local to global is in https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/blob/e46bf72ca4c49f592b2762487ff1169704658164/Sources/BriefFiniteElementNet/Utils/CalcUtil.cs#L899

code is ported from MATLAB code. from book 'MATLAB Codes for Finite Element Analysis' by 'A. J. M. Ferreira' , section 8.3 (First 3D frame example) page 107 (111 of 236)

.. code-block::matlab
    
    if x1 == x2 & y1 == y2
        if z2 > z1
            Lambda = [0 0 1 ; 0 1 0 ; -1 0 0];
        else
            Lambda = [0 0 -1 ; 0 1 0 ; 1 0 0];
        end
    else
        CXx = (x2-x1)/L;
        CYx = (y2-y1)/L;
        CZx = (z2-z1)/L;
        D = sqrt(CXx*CXx + CYx*CYx);
        CXy = -CYx/D;
        CYy = CXx/D;
        CZy = 0;
        CXz = -CXx*CZx/D;
        CYz = -CYx*CZx/D;
        CZz = D;
        Lambda = [CXx CYx CZx ;CXy CYy CZy ;CXz CYz CZz];


Iso Parametric Coordination System
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Apart from local and global coordination systems for elements, there is another system based on isoparametric formulation/representation, which is used extensively in finite element method. In BFE also in many places instead of local coordinate system, the iso parametric coordination is used.

Iso Parametric Coordination system for BarElement with two nodes
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. figure:: ../images/bar-iso-coord.png
   :align: center
   
   
Based on 
   
- At the beginning point of the element, where `x=0` the iso parametric coordinate is `ξ=-1`

- At the central point of the element, where `x=L/2`, and L is length of elements, the iso parametric coordinate is `ξ=0`

- At the end point of the element, where `x=L`, and L is length of elements, the iso parametric coordinate is `ξ=1`

In bar element with two nodes the relation between isoparamtric `ξ` coordinate and local `x` coordinate is:

``x = (ξ + 1)*L/2``
and subsequently

``ξ = (2*x-L)/L``

Iso Parametric Coordination system for BarElement with more than two nodes
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

There is no simple formula to show relation of ξ and x in elements with more than two nodes, but there is a method for conversion between local coordinate system and isoparametric coordination system ``BarElement.LocalCoordsToIsoCoords`` and ``BarElement.IsoCoordsToLocalCoords`` which works right with any number of node. As this method is defined in base ``Element`` class, input and output of these is double array, but as BarElement is one dimensional element, then only first member of array have value and that is X or ξ.

ref[1]: Finite Element Analysis: Theory and Programming by by C Krishnamoorthy p.243