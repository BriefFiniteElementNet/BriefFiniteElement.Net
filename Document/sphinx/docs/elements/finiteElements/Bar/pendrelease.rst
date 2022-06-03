.. _BarElement-PartialEndRelease:

Partial End Release Condition
-----------------------------
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


This is possible some times to 