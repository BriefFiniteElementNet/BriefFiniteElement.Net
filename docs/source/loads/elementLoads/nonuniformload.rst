.. _PartialNonUniformLoad:

PartialNonUniformLoad
======================
`BriefFiniteElementNet.Loads.PartialNonUniformLoad` class represents a nonuniform load on an element's body. Partial means not covering whole element's body, but only one region and nonuniform means load severity changes along any location. The partial part of element is defined in iso parametric coordination system.

.. figure:: images/pnfload-body-bar.png
   :align: center
   
   ``PartialNonUniformLoad`` applying on a BarElement
   

StartIsoLocations
---------
Isoparametric coordination of start location of load on the element in isoparametric coordination system.

EndIsoLocations
---------
Isoparametric coordination of end location of load on the element in isoparametric coordination system.

Coordination System
-------------------
Same as `UniformLoad.CoordinationSystem`, ``PartialNonUniformLoad.CoordinationSystem`` defines the coordination system of load. It can only have two different values of ``CoordinationSystem.Global`` or ``CoordinationSystem.Local``:

	- ``CoordinationSystem.Global``: The load is assumed in global coordination system
	- ``CoordinationSystem.Local``: The load is assumed in local coordination system of element that load is applied to (each element type have different local coordination system which is stated in appropriated section).

Look at examples section for more information on how to use.

Direction
-------------
``PartialNonUniformLoad.Direction`` which is a property with type of ``Vector``, defines the direction of trapezoidal load. An instance of ``Vector`` class defines a vector in 3d space with three components of ``X``, ``Y`` and ``Z``. Note that length of vector is not taken into account, only its direction is used.

Look at examples section and definition of local CoordinationSystem in BarElement, TriangleElement, etc. for more information on how to use.




Example 1 (BarElement)
^^^^^^^^^^^^^^^^^^^^^^
Adding a ``PartialNonUniformLoad`` with :

+ Magnitude of **100 [N/m]**
+ Direction of **Z**
+ Coordination System of **global**

to a ``BarElement``:

.. figure:: images/tload-ex1.png
   :align: center
   
.. code-block:: c++
   
   var bar = new BarElement();			    		//creating new instance of element
   var load = new PartialTrapezoidalLoad();	     	//creating new instance of load
   load.StartMagnitudes = new double[]{2000};		//set Magnitude at start
   load.EndMagnitudes = new double[]{1000};	   		//set Magnitude at end
   
   load.StartLocations = new double[]{-1+2/6};		//set locations of trapezoidal load
   load.EndLocations = new double[]{1-1/6};			//set locations of trapezoidal load
   
   load.Direction = Vector.K;						//set direction
   load.CoordinationSystem = 
       CoordinationSystem.Global;					//set coordination system
   
   bar.Loads.Add(load);								//apply load to element