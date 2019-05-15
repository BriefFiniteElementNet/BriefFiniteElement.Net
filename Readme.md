

# BriefFiniteElement.NET

A Finite Element library for static linear analysis of solids and structures fully in C#.

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

- Static Linear analysis (dynamic analysis not fully implemented yet)

- Supports ``LoadCase`` and ``LoadCombination`` approach for analysis and post process

- Considering initial displacements (settlements)
 
- Full or partial nodal restrains

- Compatible with .NET 4.0 and higher

- All data classes are Serializable

- Direct and Iterative solvers ([More](https://github.com/BriefFiniteElementNet/BFE.Net/wiki/Solvers-Available-in-package))

- Reasonable performance ([More](https://github.com/BriefFiniteElementNet/BFE.Net/wiki/Performance-and-Speed), [More 2](https://github.com/BriefFiniteElementNet/BFE.Net/wiki/Performance))

- Good Documentation available at [bfenet.readthedocs.io](https://bfenet.readthedocs.io/en/latest/)
## Donation


## Pros

### Cons
