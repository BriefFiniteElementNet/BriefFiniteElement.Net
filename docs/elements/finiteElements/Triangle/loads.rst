.. _TriangleElement-ApplicableLoads:

Applicable Loads
----------------
There are several loads currently applicable to ```TriangleElement```.

Uniform Load
^^^^^^^^^^^^
Uniform load is a uniform, per length load in [N/m^2] dimension, which is applied on the bar element.

[image]

The uniform load have three components, Ux, Uy, Uz which is per length force component in X, Y and Z directions.
Please note that if coordination system of load is set to global, Ux and Uy and Uz will be in global directions, else will be in element's local coordination system.
TODO: uniform load changed

Example:

Concentrated Load
^^^^^^^^^^^^^^^^^
Concentrated load is a single concentrated load which is applying in a point which exists on the ``BarElement``'s length.

Example:

Trapezoidal Load
^^^^^^^^^^^^^^^^
Trapezoidal load is a linearly varying load, with specific start and end, which is applied on the bar element.
This is more general than UniformLoad