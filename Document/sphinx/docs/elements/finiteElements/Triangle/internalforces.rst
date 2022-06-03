.. _TriangleElement-InternalForce:

Internal Force
--------------
After solving the ``Model``, ``TriangleElements`` will have some internal forces. Internal force at each location of element can be different and it can be catched with method ``TriangleElement.GetInternalForce``.
with get the location that you need the internal force as input. Internal force means membrane and bending tensors which are shown in picture:
TODO show with returned axis directions

Note that value returned from this method is in element's local coordination system.
to convert to global system:
TODO