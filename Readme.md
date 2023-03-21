[![DOI](https://zenodo.org/badge/67097947.svg)](https://zenodo.org/badge/latestdoi/67097947)

[![Nuget Package](https://img.shields.io/nuget/v/2)](https://www.nuget.org/packages/BriefFiniteElement.NET)   

# BriefFiniteElement.NET

A Finite Element library for Static and Linear analysis of solids and structures 100% in C#.

## Nuget Package

V2.0.5 released: https://www.nuget.org/packages/BriefFiniteElement.Net/

## How to build source

For building the source, Visual Studio 2022 is needed. Since the target framework is set to `Net6.0`, and VS2019 and prior do not support NET6.
If you want to build the source with say `Visual Studio 2019`, then simply edit this line on the `*.csproj` files with a text editor (like Notepad):

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net6.0;netstandard2.0;net45;</TargetFrameworks>
	<AssemblyVersion>2.0.5</AssemblyVersion>
	<FileVersion>2.0.5</FileVersion>
```

remove `net6.0` on the 4th line:

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>netstandard2.0;net45;</TargetFrameworks>
	<AssemblyVersion>2.0.5</AssemblyVersion>
	<FileVersion>2.0.5</FileVersion>
```
and you can build it with VS2019.

## Introduction
Brief Finite Element Dot NET (BFE.NET) is a .NET based software framework for static and linear Finite Element Analysis (FEA) of solids and structures. BFE.NET help you to simply take advantage of Object Oriented approach to analyze FE models. Advantage of such a framework is that user does have a very powerful control on what he is working with, and control is not limited to an UI with predefined controls.

## Documentation

Some documentation available from [bfenet.readthedocs.io](https://bfenet.readthedocs.io/en/latest/) and Wiki section.

## Support and bug report

You can use [issues section](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) for reporting bugs and requesting new features and asking for help on something, there is also a paid support for commercial developers available via email:

Paid support (Commercial) via email : [bfePaid@gmx.com](mailto:BFE%20Paid%20Support%20<BFEPaid@gmx.com>?Subject=Support%20Request&Body=Please%20fill%20fair%20amount%20of%20description%20here)

For a small payment (like a USD or two) you'll get premium support about using this library for your specific purpose by its developers. You can ask any question about FEM and its applications, we'll answer as much as we can...

Many payment types are accepted, including PayPal and CryptoCurrencies and maybe other types, just message and we'll talk about it!

Please note that free support still exists and is maintained by the community available from [Issues](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) and [Discussion](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/discussions) sections.

## Features

- Various Elements (2 node beam, column, truss, shaft, 3 node plate bending and membrane, 4 node tetrahedral)

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

## Hire Developers & Consultation service
We have fair amount of experience in linear FEM coding, so
You can have developers for consult and/or coding, on hourly basis or project based. Just contact email : [bfePaid@gmx.com](mailto:BFE%20Paid%20Support%20<BFEPaid@gmx.com>?Subject=Ask%20About%20Hiring&Body=Please%20fill%20fair%20amount%20of%20description%20and%20details%20here)

## Academic users
If you are using this library for research or academic porpuses, note that there are no academic/journal articles pubished yet about this project, so you can cite this project just like this:

```
@online{BFE,
  author = {epsi1on},
  title = {BriefFiniteElement.Net library: linear FEA for solids in .NET},
  year = 2014,
  url = {(https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net)},
  urldate = {2022-08-09}
}
```


