#IPolynomial interface

Represents an polynomial with one or more inputs, and single output

 x2 − 4x + 7. An example in three variables is x3 + 2xyz2 − yz + 1

 `int[] Degree { get; }`
 Gets the max degree of polynomial
 Example for 'y*x^2 + 2 y − 4x + 7' is y degree is 1, and x degree is 2

 `double[] EvaluateNthDerivative(int n, params double[] v)`
 gets the derivative matrix with derivative of order n
 size: 1 row, n cols
 buf[0,i] = Rond(F)/Rond(v[i])

 `double[] EvaluateDerivative(params double[] v)`
 same as EvaluateNthDerivative(1, params double[] v)

