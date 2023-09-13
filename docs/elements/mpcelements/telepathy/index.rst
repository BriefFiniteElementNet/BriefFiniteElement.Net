TelepathyLink
=============
``TelepathyLink`` is an ``MpcElement``, That partially connect Dofs of several nodes together. Final result is equal displacement in connected nodes. like its name, the nodes will have telepathy with each other in terms of displacement. 

Best example can be a hing that binds displacement DoFs of two nodes in same location together but doest not bind their rotational DoFs so they can each rotate individually and connection will be a hing connection.


```TelepathyLink``` is kind of connection that connects two DoF s of two different nodes in a way that both of those DoF s will have same displacement. 
Note that it is different than RigidElement, one difference is that it can partially connect nodes to each other, but RigidElement binds all DoF s of nodes together.
Another difference is if a Node have only rotation, then all other nodes will have displacement also (can you imagine?), but with this link, all connected nodes will have only rotation. 
When you connect two rotational DoF of two different nodes, it is like you connected those DoFs with gears.

.. hint:: all types of partiall connection of elements to nodes are able to be modeled with ``TelepathyLink``.

Example 1 : partial end release of a 5 meter beam
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

This example creates a ``TelepathyLink`` that connect three nodes in a way that ``Dx`` of all nodes will be equal after analysis, also ``Dy`` and ``Dz`` but not ``Rx``, ``Ry`` and ``Rz``.

.. code-block:: cs

  var n1 = new Node(0,0,0);
  var n2 = new Node(0,0,0);
  var n3 = new Node(5,0,0);

  var bar = new BarElement(n2,n3);
  var link = new TelepathyLink();
  link.UseForAllLoads = true;
  link.Nodes.Add(n1, n2);
  link.BindDx = link.BindDy = link.BindDz = true;
  link.BindRx = link.BindRy = link.BindRz = false;


Example 2: Cog (TODO add image and code)
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Imagine two nodes each one is connected to center of a cog, and two cogs are connected with another cog in between, then the two nodes will have same rotational displacemenet, and this connection can be modeled with `TelepathyLink`.