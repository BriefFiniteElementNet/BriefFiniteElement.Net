Elemental Loads
***************

.. toctree::
    :titlesonly:
    :hidden:
    :maxdepth: 2

    concentratedload
    uniformload
    partialnonuniformload
	
ElementLoad is a base class that can only apply on the Element. There are several ``ElementLoad``s:

- ``UniformLoad``: A uniform load that can apply on a ``Element`` or one of its faces or edges.
- ``PartialNonuniformLoad``: A Partial varying load.
- ``ConcentratedLoad``: A concentratel load that applies on a single point in `Element`'s body