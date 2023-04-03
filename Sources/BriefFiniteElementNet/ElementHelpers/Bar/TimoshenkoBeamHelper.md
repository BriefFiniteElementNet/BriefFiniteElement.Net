

$y(\xi)=N_1(\xi)*\Delta_1+M_1(\xi)*\theta_1+N_2(\xi)*\Delta_2+M_2(\xi)*\theta_2$
$\theta(\xi)= \frac {\partial {y(\xi)}}{\partial \xi}=N'_1(\xi)*\Delta_1+M'_1(\xi)*\theta_1+N'_2(\xi)*\Delta_2+M'_2(\xi)*\theta_2-\gamma_0$

refs:
[1] https://www.brown.edu/Departments/Engineering/Courses/En2340/Projects/Projects_2015/Wenqiang_Fan.pdf
https://www.researchgate.net/publication/236659875_Shape_functions_of_three-dimensional_Timoshenko_beam_element

https://abru.ac.ir/files/teachers/doc-1569398886.pdf
MATLAB codes for finite element Analysis by A.J.M Ferreira (Springer) P124


https://www.researchgate.net/publication/283665997_Shear_and_torsion_correction_factors_of_Timoshenko_beam_model_for_generic_cross_sections

https://eprints.soton.ac.uk/22210/1/bazo_03.pdf

https://www.tandfonline.com/doi/figure/10.1080/15502280600826381?scroll=top&needAccess=true&role=tab

shape functions [1]

$v(\xi) = N_{v1}* v1 + N_{v2}* θz1 + N_{v3}* v2 + N_{v4}* θ_{z2}$

$N_{v1} = \bar\Phi_z (1 − 3ξ^2 + 2ξ^3 + Φz (1 − ξ))$
$N_{v2} = l.\barΦz (ξ − 2ξ^2 + ξ^3 + Φz (ξ − ξ^2)/2)$
$N_{v3} = \barΦz (3ξ^2 − 2ξ^3 + Φz ξ)$
$N_{v4} = l.\barΦz (−ξ^2 + ξ^3 + Φz (−ξ + ξ^2)/2)$

AND

$θ_z (ξ) = N_{θz1} v_1 + N_{θz2} θ_{z1} + N_{θz3} v_2 + N_{θz4} θ_{z2}$

$N_{θz1} = 6 \barΦ_z l (−ξ + ξ^2)$
$N_{θz2} = \barΦ_z (1 − 4ξ + 3ξ^2 + Φ_z (1 − ξ))$
$N_{θz3} = −6 \barΦ_z l (−ξ + ξ^2)$
$N_{θz4} = \barΦz (−2ξ + 3ξ^2 + Φ_z ξ)$


$$\barΦ_z = 1 / (1 + Φ_z)$$

$$Φ_z = 12*Λ_z /l^2 = \frac{ 12EI_{zz}}{\kappa G A l^2}$$

```
syms l alpha
A = [sym(1) 0 0 0;0 sym(1) 0 6*alpha;sym(1) l l^2 l^3;0 sym(1) 2*l (3*l^2+6*alpha)]
factor(inv(A)*det(A))
```

**Note:** the $\xi$ equals to $x/l$ in above formulation. but we assume something else in BFE!


```
syms phi_h phi xi l

n1 = phi_h* (1 - 3*xi^2 + 2*xi^3 + phi* (1 - xi))
m1 = l*phi_h* (xi - 2*xi^2 + xi^3 + phi* (xi - xi^2)/2)
n2 = phi_h* (3*xi^2 - 2*xi^3 + phi *xi)
m2 = l*phi_h* (-xi^2 + xi^3 + phi* (-xi + xi^2)/2)


n1p = +6 *phi_h /l * (-xi + xi^2)
m1p = phi_h* (1 - 4*xi + 3*xi^2 + phi* (1 - xi))
n2p = -6 *phi_h/ l *(-xi + xi^2)
m2p = phi_h* (-2*xi + 3*xi^2 + phi* xi)

% xi = x/l in above equtions
% want to convert to 2*(x/l)-1

sympref display flat

syms x

n1_=subs(subs(n1,xi,x/l),x,(xi+1)/2*l)
n2_=subs(subs(n2,xi,x/l),x,(xi+1)/2*l)
m1_=subs(subs(m1,xi,x/l),x,(xi+1)/2*l)
m2_=subs(subs(m2,xi,x/l),x,(xi+1)/2*l)

n1p_=subs(subs(n1p,xi,x/l),x,(xi+1)/2*l)
n2p_=subs(subs(n2p,xi,x/l),x,(xi+1)/2*l)
m1p_=subs(subs(m1p,xi,x/l),x,(xi+1)/2*l)
m2p_=subs(subs(m2p,xi,x/l),x,(xi+1)/2*l)

disp('N1');
[c,t] = coeffs(n1_,xi,'all');
disp(c(1,1));
disp('N2');
[c,t] = coeffs(n2_,xi,'all');
disp('M1');
[c,t] = coeffs(m1_,xi,'all');
disp('M2');
[c,t] = coeffs(m2_,xi,'all');

disp('N1`');
[c,t] = coeffs(n1p_,xi,'all');
disp('N2`');
[c,t] = coeffs(n2p_,xi,'all');
disp('M1`');
[c,t] = coeffs(m1p_,xi,'all');
disp('M2`');
[c,t] = coeffs(m2p_,xi,'all');
```






For beam we have 4 shape functions, namely $N_1$, $N_2$, $M_1$, $M_2$
Also we have 4 conditions for each of these functions:

|#|condition| $N_1$ |$N_2$  |$M_1$ |$M_2$|
|-|--|--|--|--|--|
|1|$F(\xi=-1)$|1  |0  |0 |0
|2|$\frac {dF}{d\xi}(\xi=-1)$|0  |0  |1|0
|3|$F(\xi=+1)$| 0 | 1 |0 | 0
|4|$\frac {dF}{d\xi}(\xi=+1)$| 0 | 0 | 0|1

For example for $N_1$ we have four conditions

- $N_1(\xi=-1)=1$
- $N_1'(\xi=-1)=0$
- $N_1(\xi=1)=0$
- $N_1'(\xi=1)=0$

Right side is column $N_1$.

Now if we have partial release, then appropriated row will be eliminated.
For example if $\Delta_{left}$ of left node is released, then the row #1 will be eliminated.
For $\theta_{left}$, $\Delta_{right}$ and $\theta_{right}$ the rows 2 to 4 will be eliminated.
If we assume order 3 for each shape function