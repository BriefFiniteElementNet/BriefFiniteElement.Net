.. _DefineFullModel:

Defining a full model
######################

As (defined in wikipedia.org)[https://en.wikipedia.org/wiki/Finite_element_method]:

The finite element method (FEM), is a numerical method for solving problems of engineering and mathematical physics. Typical problem areas of interest include structural analysis, heat transfer, fluid flow, mass transport, and electromagnetic potential.

This library is meant to give user a tool to do FEM for structural analysis. Usually FEM for structural analysis falls into 2 major areas, Linear FEM and Nonlinear FEM. Linear is simple and straight forward method, for both program developer and users. But in other side it does not give accurate results against Nonlinear. Results of Nonlinear FEM is usually more near to reality. This library is currently for linear analysis of structures.


In area of FEA of solids and structures, there are several fundamental concepts or things:
- Elements

- Nodes

- Loads

We will use these things as ``Class`` es in our object oriented architecture. So there will be 3 classes exactly named as above. For example take a look at this Model:

[TODO image]

As you see in above image, there phisical model is considered ans a series of Elements, at corner of each element there is a Node, and there are some loads applying to either nodes or elements. FEM does calculate quantities such as displacements in Nodes. Then using interpolation or shape functions some other quantities like stress, strain or displacement will be calculated for each Element. 

{TODO image: a: model, b:displacement at nodes, c: displacement at elements}

