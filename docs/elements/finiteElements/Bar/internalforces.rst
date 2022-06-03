.. _BarElement-InternalForce:

Internal Force And Displacement
-------------------------------
After solving the ``Model``, ``BarElement`` will have some internal forces. Internal force at each location of element can be different and it can be retrieved with method ``BarElement.GetInternalForceAt`` and ``BarElement.GetExactInternalForceAt``. both methods gives the internal force at specified iso parametric coordinate. The difference between ``BarElement.GetInternalForce`` and ``BarElement.GetExactInternalForce`` is that 
``BarElement.GetInternalForceAt`` only consider nodal displacement for internal force but Exact one (``GetExactInternalForceAt``) also considers effect of elemental loads (like distributed loads) in element in addition to nodal displacements. Internal force means 3 forces and 3 moments: axial load (Fx), two shear loads (Fy,Fz), torque moment (Mx) and two biaxial moments (My,Mz) which are shown in picture:

.. figure:: ../images/barinternalforce.png
   :align: center

Note that value returned from this method is in element's local coordination system.


For example the beam below with both ends fixed, after solve does not have any nodal displacement, so the standard FEM formula D*B*u will return 0, so ``BarElement.GetInternalForceAt`` for this example returns 0, but ``BarElement.GetExactInternalForceAt`` at middle will not return zero...

.. code-block:: cs
   
    var model = new Model();

	Node n1, n2;

	model.Nodes.Add(n1 = new Node(0, 0, 0) { Constraints = Constraints.Fixed });
	model.Nodes.Add(n2 = new Node(1, 0, 0) { Constraints = Constraints.Fixed });

	var elm = new BarElement(n1, n2);

	elm.Section = new BriefFiniteElementNet.Sections.UniformParametric1DSection(a: 0.01, iy: 0.01, iz: 0.01, j: 0.01);
	elm.Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

	var load = new Loads.UniformLoad();

	load.Case = LoadCase.DefaultLoadCase;
	load.CoordinationSystem = CoordinationSystem.Global;
	load.Direction = Vector.K;
	load.Magnitude = 10;

	elm.Loads.Add(load);
	model.Elements.Add(elm);

	model.Solve_MPC();

	var f1 = elm.GetInternalForceAt(0);
	var f2 = elm.GetExactInternalForceAt(0);
	
	
TODO: internal displacement