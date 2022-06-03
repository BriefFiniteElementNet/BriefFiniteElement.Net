MatrixPool
==========
In calculating local stiffness matrix for elements that need integration, matrixes are created and destroyed many times.
Using a MatrixPool concept increased the performance in calculations maybe about 100%