# Internal Load Force and Displacement

Theory stuff:

shaer: $V(x)=\int{W(x).dx}+F_0$
moment: $M(x)=\int{V(x).dx}+M_0=\int {\int{W(x).dx}+F_0.x+M_0}$
slope's diff: $\theta(x)=\int{\frac{M(x)}{E(x).I(x)}.dx}+\theta_0=\int {\frac{\int {\int{W(x).dx}+F_0.x+M_0}}{EI(x)}}+\theta_0$
slope: $\Delta(x)=\int{\theta(x).dx}+\Delta_0=\int \int {\frac{\int {\int{W(x).dx}+F_0.x+M_0}}{EI(x)}}+\theta_0$

$W(x)$: arbitrary distributed load over the beam body, which we do know
$F_0$: support reaction at start point, equal to inverse of equivalent nodal load applied to first node (force)
$M_0$: support reaction at start point, equal to inverse of equivalent nodal load applied to first node (moment)
$\theta_0$: support settlement at start point, equal to zero usually but depends on load and end release of beam
$\Delta_0$: support settlement at start point, equal to zero usually but depends on load and end release of beam
$E(x), I(x)$: young modulus and section inertia
# GetLoadInternalForce_UniformLoad(Load, Element)

this method is used when Load is uniform. both cases load can be assumed as a polynomial:


Step 1: find $M_0$ and $F_0$  (the inverse of equivalent nodal loads on starting node) and $W_0$ (the severity of distributed uniform load)
Step 2: define $x_s$ as array of double, sampling points to create the $\frac {M(x)} {E(x).I(x)}$ as a polynomial
Step 3: $M(x) = W_0*\frac{x^2}{2}+F_0*x+M_0$, $E(x)$ and $I(x)$ are accessible
Step 4: form an array `MoverEis = x_s.Select(x=>M(x)/(E(x)*I(x)))`
note that `x = (ξ + 1)*L/2`
Step 5: we have the $\frac {M(x)} {E(x).I(x)}$ approximated as a polynomial, lets call it $G(x)$
Step 6: find $\theta_0$ and $\Delta_0$. if beam end release are all fixed, those are zero. otherwise they should be calculated with another method which is not implemented yet. so w'll throw exception if end release are not fixed.
Step 7: $\Delta(x)=\int \int {G(x).dx}+{(\theta_0.x+\Delta_0=0)}$

Note that if $E(x)$ and $I(x)$ are constant regard to $x$, i.e. section and material do not change along the beam, then $G(x)$ is a polynomial of degree `2`. so having 3 sample points or more will give us exact solution if $EI(x)$ is constant. but if $EI(x)$ is not constant, then $G(x)$ will not be polynomial anymore so to increase accuracy of our approximation should have more sampling points, so:
- will use `3` points if $EI(x)$ is constant
- will use `3+2*n+2*m` points if $EI(x)$ is not constant (`n` is degree of $E(x)$ and `m` is degree of $I(x)$)


