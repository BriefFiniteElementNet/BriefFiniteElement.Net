A `LoadHandler` handles calculation of a load on an specific `ElementHelper`. becuase each element helper have it's own calculations for each load.
For example euler bernauly beam under concentrated load.

A `LoadHandler` typically do:

- Calculate EquivalentNodalLoads
- Calculate LoadDisplacementAt (used for GetExactDisplacementAt)
- Calculate LoadInternalForceAt (used for GetExactForceAt)
- Tells wether it can handle a specific combination of ElementHelper-Load-Element or not

These are public methods for interface `ILoadHandler`:

``` c#
public interface ILoadHandler
{
    //xml documentation removed
    Force[] GetLocalEquivalentNodalLoads(Element elm, ElementalLoad load);
    StrainTensor GetLocalLoadDisplacementAt(Element elm, ElementalLoad load, IsoPoint loc);
    CauchyStressTensor GetLocalLoadInternalForceAt(Element targetElement, ElementalLoad load, IsoPoint loc);
    bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load);
}
```


Element Helpers (7)

- EulerBernulyBeamHelper
- TrussHelper
- ShaftHelper

- CstHelper
- DktHelper
- TriangleDrillingDofHelper

- TetrahedronHelper

Material and Geometry condition (2)

- Uniform
- NonUniform

Loads (4)

- UniformLoad
- ConcentratedLoad
- PartialNonuniformLoad
- ~~PartialLinearLoad~~ (deprecated - use partial nonuniform load instead)
 
- ImposedStrainLoad



Total 56 different combination.
Naming convention: `{HLP}_{UF}{NUF}_{LOAD}` where:
- HLP: helper abbreviation
- UF: uniform material and section
- NUF: nonuniform material / section
- LOAD: name of the load

# Example

EulerBernuly_UF_ImposedStrain.cs

Those which are zero, no need to create. like `EulerBernuly_UF_ImposedStrain.cs` which it's output is always zero...
overlap, for example `NUF` formulation will work for `UF` situation, but higher computation cost.

generaly there will be 3 types of handler:
- zero handler (output always zero)
- nonzero handler (output nonzero)
- not implemented handler (which is not implemented yet!)

hanlers for each helper goes into a different directory.

# Search mechanism

The problem is, how one know if the combination of ElementHelper and Load is handled and searching mechanism?
each helper knows it's own handlers. 

this pseudo code should work as process:

``` C#
Force[] ElementHelper.GetLocalEquivalentNodalLoads(Element elm, ElementalLoad load)
{
  var handlers = this.GetLoadHandlers();

  foreach(var hnd in handlers)
  {
    if(hnd.CanHandle(elm,this,load))
      return hnd.GetLocalEquivalentNodalLoads(elm,this,load);
  }

  throw new NotImplementedException("no hander for combination " + elm.GetType() + load.GetType() + "and uf-f ity!" );
}
```