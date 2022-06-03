Introduction
#############

Introduction (What is BFE.NET)
==============================

`Brief Finite Element DOT NET  <https://github.com/BriefFiniteElementNet/BFE.Net>`_
or BFE.NET is an open source, software library/framework which is written in C# programming language. This library makes .NET developers to simply be able to add some linear FEM capabilities (in solids and structures) into their software. 

This library do not contains any all purpose graphical user interface yet so only way to use this library is to interact with it using C# language or other .NET based programming languages.


BFE.NET Basics
================
Usually an FE (Finite Element) model is consists of a set of nodes, elements and loads. These three things are fundamentals in this Finite Element Method library. Nodes are points in the space and each elements is connected to a set of nodes, and loads are applying to either nodes or elements.
This is an example of a pure FE model of a solid object:

	TODO

BFE have a Object Oriented background. In BFE.NET, Whole model is an instance of ``Model`` class. The Model class is consisted of a list of ``Node`` Class, and a list of ``Element`` Class allowing user to analyze the model, get displacements of Nodes displacements and internal forces of Elements.
``Node`` class have a position in 3d space(X-Y-Z), Element class does have a type and have a list of nodes, and each Node or Element does have a list of loads which is applying to it. This combination makes vast modelling capabilities and organization.

Acknowledgement
================

This project is using/used these projects/files:

- `CSParse.NET <https://github.com/wo80/CSparse.NET>`_  and some other codes from Christian Woltering for solving sparse linear system

- `Helix 3D <http://www.helix-toolkit.org/>`_  Toolkit for some 3D visualizations

- Formulation of considering shear deformation and partial end release in FrameElement2Node are ported from `Matrix Structural Analysis (MSA) <https://nl.mathworks.com/matlabcentral/fileexchange/27012-matrix-structural-analysis?focused=5148840&tab=function>`_


Key Features
============

Key Features of BFE.NET:

- Only analysis type is **static** and **linear** Finite Element Analysis of solids and structures

- Supports 'load case' and 'load combination' concepts

- 6 DoF per each Node

- Supports Full or partial restrains per node (inclined or skewed supports not supported)

- Ability to consider initial displacements (settlements)

- Serialize friendly classes! (almost all classes can be binary searialized/deserialized seamlessly )

- Available for .NET 3.5 Client Profile and higher

- Fully written in C# without any external dependencies

- Does have a very nice documentation (took plenty of time to make it!)

- Classes inside code are properly documented (in XML format)

- Highly optimized compressed column storage (CCS) sparse storage for large matrix operations (thanks to CSparse.NET library)

- Direct (Cholesky) and Iterative (Condugate Gradient - CG) solvers for solving equation system

- Ability to make some pre checks to find fundamental errors in Model