

# BriefFiniteElement.NET

A Finite Element library for Static and Linear analysis of solids and structures 100% in C#.

[![Build status](https://ci.appveyor.com/api/projects/status/var3sx7nxa309tmo?svg=true)](https://ci.appveyor.com/project/epsi1on/brieffiniteelement-net)     [![Join the chat at https://gitter.im/BFE-Net/Lobby](https://badges.gitter.im/BFE-Net/Lobby.svg)](https://gitter.im/BFE-NET/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Notice
We are fixing bugs before release Nuget Package, please let us know any bugs you found in this library in order to let us fix it. To report bugs please use [issues section](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues).

## Introduction
Brief Finite Element Dot NET (BFE.NET) is a .NET based software framework for static and linear Finite Element Analysis (FEA) of solids and structures. BFE.NET help you to simply take advantage of Object Oriented approach to analyze FE models. Advantage of such a framework is that user does have a very powerful control on what he is working with, and control is not limited to an UI with predefined controls.

## Support and bug report

You can use [issues section](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) for reporting bugs and requesting new features and asking help on something, there is also a paid support available via email:

[BFEPaid@gmx.com](mailto:BFEPaid@gmx.com?subject=Paid%20Support&body=Please%20fill%20fair%20amount%20of%20description%20here)

For a small payment (like a USD or two) you'll get premium support about using this library for your specific purpose, if it is bug with library itself, then will fix it for free and return your payment.

Please note that free support still exists and maintained by community in [Issues](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) and [Discussion](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/discussions) sections.

We do accept many payment types, including PayPal and Crypto-Currencies and maybe other types, just message and we'll talk about it!

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
Since it is not possible to validate all features with a single software, 

* Unit Test (in project ``BriefFiniteElementNet.Tests``)
* Validating the result with other well known and open source applications:

	*  Validating the result with OpenSees (the Open System for Earthquake Engineering Simulation) available at [opensees.berkeley.edu](http://opensees.berkeley.edu/)

	*  Validating the result with Frame3dd application available at [frame3dd.sourceforge.net](http://frame3dd.sourceforge.net)

for more information on validation please have a look at [Validation.md](Validation.md) file.


## Known Issues

for more information on known issues please have a look at [know-issues.md](know-issues.md) file and [issues](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) section.

## Donation


