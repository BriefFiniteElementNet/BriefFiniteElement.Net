HingLink
============
``HingLink`` is an ``MpcElement`` which connects only translational DoFs of nodes together. There is a restriction where node's location must be same (exactly same) otherwise it throws exception. example usage are connecting a slab into beam with simple connection. where slab for example have 4 nodes, and 4 column in corners each one have 2 nodes, total nodes are 12 nodes and top nodes of columns are connected to slab, each with a separate hing link.

hing link is some sort of link that connects two nodes to each other, but only connect their Dx, Dy, Dz together not rotations of them.
Limitation is that both nodes should have same location.
Using this sort of link, it is possible to model end release in Shells, etc.

