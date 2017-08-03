#Intro
RREF or Reduced Row Echelon Form is defined in ref[0]. popuse of this interface is to define a form for finders of RREF.

Main method:

```
CSparse.Double.CompressedColumnStorage CalculateRref(CSparse.Double.CompressedColumnStorage a);
```

Input:
Matrix ```a``` with dimensions ```m*n``` (possibly nonsquare).

Output:
Matrix ```b``` with dimensions ```r*n``` where ```r``` is rank of matrix ```b```. and matrix ```b``` is in RREF form

#refs
ref[0]: https://en.wikipedia.org/wiki/Row_echelon_form#Reduced_row_echelon_form
ref[1]: https://github.com/wo80/CSparse.NET/issues/7
ref[2]: https://math.stackexchange.com/questions/2340450


TODO:
Create a simple IRrefFinder based on ref[2] for temporarily usage