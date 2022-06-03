MPC Elements
****************

.. toctree::
    :titlesonly:
    :hidden:
    :maxdepth: 2

    rigid/index
    vconstraint/index
    hingLink/index
    telepaty/index	
    
MPC elements or Multi-Point Constraint elements, are kind of virtual elements that binds several DoFs of a model together and reduces the overall number of independent DoFs using technique MPC (Multi-Point Constraints) and Master/Slave model. All MPC elements are inherited from ``BriefFiniteElement.Elements.MpcElement'.

Overview of special elements available:
	- ``TelepathyLink``: Partially binds DoFs of several nodes together
	- ``RigidElement``: An non-deformable element with virtually infinite (∞) stiffness
	- ``VirtualConstraint``: An element that virtually binds its nodes into ground and make them support nodes.
	
more info: https://mashayekhi.iut.ac.ir/sites/mashayekhi.iut.ac.ir/files//files_course/lesson_16.pdf


MpcElement have a feature than can be taken into account in particular loads. For example when analyzing a Model against Eq. loads, a rigid diaphragm (with infinite stiffness) can be considered. This rigid diaphragm should not be applied when model is solving against other loads like Dead or Live loads. There are three properties for ``MpcElement`` class regarding this feature:

- MpcElement.UseForAllLoads:
It is false by default, if set to true then ``MpcElement`` will be applied in every situation and all loads.
Set this to true, when ``MpcElement`` should be considered against all loads.

- MpcElement.AppliedLoadCases
By defaults is empty, ``MpcElement`` will be applied when structure is analysing with LoadCases inside this.

- MpcElement.AppliedLoadTypes
By defaults is empty, ``MpcElement`` will be applied when structure is analysing with a LoadCase which have a LoadCase.LoadType which is present inside this.


Example 1:
An MpcElement (rigid element) which connect several nodes and only taken into account with loads with type of Eq.

.. code-block:: cs
  var model = new Model();
  //TODO: Initaite nodes and elements
  //Pseudu code:
  // foreach roof:
  //   make a rigid element, bind nodes in the roof's surface
  //   set each rigid element's:
  //     MpcElement.UseForAllLoads = false;
  //     MpcElement.AppliedLoadTypes.Add(LoadType.Quake);

In this model there are ``TODO`` number of roofs, that are only considered when Eq. loads are applied.

