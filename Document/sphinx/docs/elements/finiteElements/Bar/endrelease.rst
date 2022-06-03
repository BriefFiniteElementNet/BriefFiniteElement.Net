.. _BarElement-PartialEndRelease:

Partial End Release
--------------
By default connection of BarElement into end nodes are rigid, e.g. all DoFs of BarElement are connected to end node, but there are some situation that there is need for partial connections. 

``BarElement.NodalReleaseConditions`` defines the partial end release of ``BarElement`` on each of it's nodes. Also ``BarElement.StartReleaseCondition`` and ``BarElement.EndReleaseCondition`` uses this property to get/set release conditions for start and end nodes:

.. code-block:: cs

     /// <summary>
     /// Gets or sets the connection constraints od element to the start node
     /// </summary>
     public Constraint StartReleaseCondition
     {
         get { return _nodalReleaseConditions[0]; }
         set { _nodalReleaseConditions[0] = value; }
     }

     /// <summary>
     /// Gets or sets the connection constraints od element to the end node
     /// </summary>
     public Constraint EndReleaseCondition
     {
         get { return _nodalReleaseConditions[_nodalReleaseConditions.Length - 1]; }
         set { _nodalReleaseConditions[_nodalReleaseConditions.Length - 1] = value; }
     }
	 
There are 3 properties for BarElement for taking end releases into consideration:

``
public Constraint StartReleaseCondition{get;set;}
public Constraint EndReleaseCondition{get;set;}
public Constraint[] NodalReleaseConditions{get;set;}
``

StartReleaseCondition Gets or sets the release condition to first node, EndReleaseConditionGets or sets the release condition to last node and NodalReleaseConditions Gets or sets the release condition to all nodes (it is an array).
