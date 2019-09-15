

# BriefFiniteElement.NET

A Finite Element library for Static and Linear analysis of solids and structures 100% in C#.

[![Build status](https://ci.appveyor.com/api/projects/status/q5an94f88kofefm9?svg=true)](https://ci.appveyor.com/project/epsi1on/bfe-net)     [![Join the chat at https://gitter.im/BFE-Net/Lobby](https://badges.gitter.im/BFE-Net/Lobby.svg)](https://gitter.im/BFE-NET/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Introduction
Brief Finite Element Dot NET (BFE.NET) is a .NET based software framework for static and linear Finite Element Analysis (FEA) of solids and structures. BFE.NET help you to simply take advantage of Object Oriented approach to analyze FE models. Advantage of such a framework is that user does have a very powerful control on what he is working with, and control is not limited to an UI with predefined controls.

## Acknowledgement
This project is using/used these projects/files:

- [CSParse.NET](https://github.com/wo80/CSparse.NET) and some other codes from [Christian Woltering](https://github.com/wo80) for solving sparse linear system
- [Helix 3D Toolkit](http://helixtoolkit.codeplex.com/) for 3D visualizations
- Formulation of considering shear deformation or partial end release in frame elements are ported from [Matrix Structural Analysis (MSA)](http://www.mathworks.com/matlabcentral/fileexchange/27012-matrix-structural-analysis/content/MSA/MSA.m) by Dr. Hossein Rahami


## Features

- Various Elements (variable node beam, column, truss, shaft, 3 node plate bending and membrane, 4 node tetrahedral)

_ Different loads types (Concentrated force or moment in `BarElement` body, uniform load on `BarElement`)

_ Calculate internal force of `BarElement` at any location of Element.

- Static Linear analysis (dynamic analysis not fully implemented yet)

- Supports ``LoadCase`` and ``LoadCombination`` approach for analysis and post process

- Considering initial displacements (settlements)
 
- Full or partial nodal restrains

- Compatible with .NET 4.0 and higher

- All data classes are Serializable

- Direct and Iterative solvers ([More](https://github.com/BriefFiniteElementNet/BFE.Net/wiki/Solvers-Available-in-package))

- Reasonable performance ([More](https://github.com/BriefFiniteElementNet/BFE.Net/wiki/Performance-and-Speed), [More 2](https://github.com/BriefFiniteElementNet/BFE.Net/wiki/Performance))

- Good Documentation available at [bfenet.readthedocs.io](https://bfenet.readthedocs.io/en/latest/)

## Validation

This library is developed regarding "Code Reuse" so the code will be somehow complicated. There are several types of validation for FE models in this library:

* Unit Test (in project ``BriefFiniteElementNet.Tests``)
* Validating the result with other well known and open source applications:

	*  Validating the result with OpenSees (the Open System for Earthquake Engineering Simulation) available at [opensees.berkeley.edu](http://opensees.berkeley.edu/)

	*  Validating the result with Frame3dd application available at [frame3dd.sourceforge.net](http://frame3dd.sourceforge.net)

for more information on validation please have a look at [Validation.md](Validation.md) file.

## Known Issues

for more information on known issues please have a look at [know-issues.md](know-issues.md) file.

## Donation


## Pros

### Cons
