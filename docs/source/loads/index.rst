Loads Available
###############

TODO image

.. toctree::
    :titlesonly:
    :hidden:
    :maxdepth: 2

    elementLoads/index
    nodalLoads/index
	
There are two types of load in general: NodalLoad and ElementLoad. NodalLoad does apply on nodes and only have concentrated load, but ElementLoad is abstract base class and does
apply on elements instead of nodes. Image can shows the difference better:

All loads (including nodal and elemental) have ``LoadCase``. See LoadCase and combinations for more info.
