[![Nuget Package](https://img.shields.io/nuget/v/2)](https://www.nuget.org/packages/BriefFiniteElement.NET)   

# BriefFiniteElement.NET

A Finite Element library for Static and Linear analysis of solids and structures 100% in C#.

## Migration from GitHub
Due to U.S. trade controls law restrictions, GitHub did ban all of it's users which where citizen of Iran back in 2019 ([link](https://financialtribune.com/articles/sci-tech/99111/github-bans-iran-based-users) to proof). the main maintainer of this project is currently [epsi1on](https://github.com/epsi1on) which is citizen of Iran. So just in case which he is banned and lose access to his github account again, the development will continue in `git.bfe-framework.net`. Will try to keep BFE and it's successor opensource.
Thank you GitHob for providing the grow environment for this project for ~10 years.

## Nuget Package

https://www.nuget.org/packages/BriefFiniteElement.Net/

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

## Examples
There are examples:
- located in `Samples\Examples.CSharp` in the git repo
- located in wiki 'https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/wiki/Examples'
- located in documentation `https://bfenet.readthedocs.io/en/latest/example/index.html`

## Matrix Must Be Positive and Definite!

Have a look at here:
[Wiki:How to fix NotPosDef error](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/wiki/How-to-fix-NotPosDef-error)


## Introduction
Brief Finite Element Dot NET (BFE.NET) is a .NET based software framework for static and linear Finite Element Analysis (FEA) of solids and structures. BFE.NET help you to simply take advantage of Object Oriented approach to analyze FE models. Advantage of such a framework is that user does have a very powerful control on what he is working with, and control is not limited to an UI with predefined controls.

## Documentation

Some documentation available from [bfenet.readthedocs.io](https://bfenet.readthedocs.io/en/latest/) and Wiki section.

## Support and bug report

You can use [issues section](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) for reporting bugs and requesting new features and asking for help on something, there is also a paid support for commercial developers available via email:

Paid support (Commercial) via email : [bfePaid@gmx.com](mailto:BFE%20Paid%20Support%20<BFEPaid@gmx.com>?Subject=Support%20Request&Body=Please%20fill%20fair%20amount%20of%20description%20here)

For a small payment you'll get premium support about using this library for your specific purpose by its developers. 
Many payment types are accepted, including PayPal and CryptoCurrencies and maybe other types, just message and we'll talk about it!

Please note that free support still exists and is maintained by the community available from [Issues](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues) and [Discussion](https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/discussions) sections.

## Features

- Various Elements (2 node beam, column, truss, shaft, 3 node plate bending and membrane, 4 node tetrahedral)

- Different loads types (Concentrated force or moment in `BarElement` body, uniform load on `BarElement`)

- Calculate internal force of `BarElement` at any location of Element.

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
We have fair amount of experience in linear FEM coding, so You can have developers for consult and/or coding, on hourly basis or project based. Just contact email : [bfePaid@gmx.com](mailto:BFE%20Paid%20Support%20<BFEPaid@gmx.com>)


## A word with programmers with no structural/mechanical background
Well, if you are a programmer like me (most likely a freelancer) with no background in structural design / mechanical engineering wich is trying to build a moderate or big software for a company with this library, then i afraid to tell you with high chance, it is not going to work! not because the library is not completed or evillness of the developers, (honestly library have several incomplete and under development parts but the main body is almost done). But because engaging in developing a software which coder do not know much about underlying procedure, is last item in the list of works which an experienced programmer likes to do. In this case you can get help with co-worker price. you can contact with email above.

Of course if it is small software, then there will be no problem and there is no much need for knowledge about the structural engineering. It is simple possible to do small softwares...
