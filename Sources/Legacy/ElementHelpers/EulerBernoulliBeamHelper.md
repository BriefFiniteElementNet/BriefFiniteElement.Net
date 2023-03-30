this is euler bernouly with 2 node and NO PARTIAL CONNECTION to nodes

https://www.iitg.ac.in/stud/sumit_kumar/blog/finite-element-analysis-of-a-beam-in-matlab/

``` octave
syms xi J

n1 = 1/4*(1-xi)*(1-xi)*(2+xi)
n2 = 1/4*(1-xi)*(1-xi)*(1+xi)
n3 = 1/4*(1+xi)*(1+xi)*(2-xi)
n4 = 1/4*(1+xi)*(1+xi)*(xi-1)

sympref display flat

N = [n1,n2*J,n3,n4*J]

expand(diff(N,xi,0))
expand(diff(N,xi,1))
expand(diff(N,xi,2))
expand(diff(N,xi,3))

```