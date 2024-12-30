#Intro
RREF or Reduced Row Echelon Form is defined in ref[0]. popuse of this interface is to define a form for finders of RREF.
Maybe not exactly RREF form is returned by method. 

Main method:

```
CSparse.Double.CompressedColumnStorage CalculateRref(CSparse.Double.CompressedColumnStorage a);
```
if matrix a is nxm, then result should have at least 

Input:
Matrix ```a``` with dimensions ```m*n``` (possibly nonsquare).

Output:
Matrix ```b``` with dimensions ```r*n``` where ```r``` is rank of matrix ```b```. 
and matrix ```b``` have ```r``` columns with only and exactly only one nonzero element except the last column.
Last column zeros and nonzeros is not taken into account.



#refs
ref[0]: https://en.wikipedia.org/wiki/Row_echelon_form#Reduced_row_echelon_form
ref[1]: https://github.com/wo80/CSparse.NET/issues/7
ref[2]: https://math.stackexchange.com/questions/2340450


TODO:
Create a simple IRrefFinder based on ref[2] for temporarily usage