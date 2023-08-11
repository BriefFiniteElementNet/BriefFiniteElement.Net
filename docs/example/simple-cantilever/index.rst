.. _BarElement-SimpleCantilever-example:

Cantilever Beam (Console Beam) Example
######################################

Consider the beam shown in fig below.

TO BE DONE

We need to find out the support reaction on node `n0` and internal force of beam at any length `x`.

Step 1: making model

.. code-block:: cs

    // Initiating Model, Nodes and Members
    var model = new Model();
	
	var n1 = new Node(0, 0, 0);
	n1.Label = "n1";//Set a unique label for node
	var n2 = new Node(2, 0, 0) {Label = "n2"};//using object initializer for assigning Label
	
	var e0 = new BarElement(n1, n2) { Label = "e1", Behavior = BarElementBehaviours.FullFrame };
	
	model.Nodes.Add(n1, n2);
	model.Elements.Add(e1);
	
	e1.Section = new Sections.UniformParametric1DSection() { A = 9e-4 };

	e1.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);
	
	n1.Constraints = Constraints.Fixed;
	
	var load = new UniformLoad()!
	
	e1.Loads.Add(load);
	
	model.Solve_Mpc();
	
	var r1 = n1.GetSupportReaction();
	
	var fnc = new Func<double,double>(x=>e1.GetExactInternalForceAt(e1.LocalToIso(x)).Fz;
	
	FuncVisualizer.VisualizeInNewWindow(fnc);
	
	//note: code not complete
	
