VirtualSupport
============
``VirtualSupport`` is an ``MpcElement``, that can fix any free dofs of model. 
Technically there is no difference between using `Node.Constraint` and `VirtualSupport` element in these two version of code, both will have exactly same result after solve:

.. code-block:: cs

    for(var i = 0;i < 10;i ++)
    {
	    model.Nodes[i].Constraint = Constraints.FixedDx;
    	model.Nodes[i].Settlements = new Displacement(0.1,0,0,0,0,0);
    }


.. code-block:: cs

    var elm = new VirtualConstraint();
    elm.Constraint = Constraints.FixedDx;
    elm.Settlement = new Displacement(0.1,0,0,0,0,0);

    for(var i = 0;i < 10;i ++)
	    elm.Nodes.Add(model.Nodes[i]);

    model.MpcElements.Add(elm);
	

but the second one will let user to define settlements for specific LoadCases. Or set some constraints for only specific load cases. for example it is possible to have set constraint in loadcase `A` when rigid elements are applied only in analysing with load case `B`.
