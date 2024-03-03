resources:

Lecture 10: Equivalent Load on nodes in beam element
https://www.youtube.com/watch?v=bFGLWWhWPhU

  

  

Theory stuff:

  

  

  

- shear: $V(x)=\int{W(x).dx}+F_0$

  

  

- moment: $M(x)=\int{V(x).dx}+M_0=\int {\int{W(x).dx}+F_0.x+M_0}$

  

  

- slope's diff: $\theta(x)=\int{\frac{M(x)}{E(x).I(x)}.dx}+\theta_0=\int {\frac{\int {\int{W(x).dx}+F_0.x+M_0}}{EI(x)}}+\theta_0$

  

  

- slope: $\Delta(x)=\int{\theta(x).dx}+\Delta_0=\int \int {\frac{\int {\int{W(x).dx}+F_0.x+M_0}}{EI(x)}}+\theta_0$

  

  

# Internal Load Force and Displacement

  

  

$W(x)$: arbitrary distributed load over the beam body, which we do know

  

  

$F_0$: support reaction at start point, equal to inverse of equivalent nodal load applied to first node (force)

  

  

$M_0$: support reaction at start point, equal to inverse of equivalent nodal load applied to first node (moment)

  

  

$\theta_0$: support settlement at start point, equal to zero usually but depends on load and end release of beam

  

  

$\Delta_0$: support settlement at start point, equal to zero usually but depends on load and end release of beam

  

  

$E(x), I(x)$: young modulus and section inertia

  

  

# GetLoadInternalForce_UniformLoad(Load, Element)

  

  

  

this method is used when Load is uniform. both cases load can be assumed as a polynomial:

  

  

  

  

Step 1: find $M_0$ and $F_0$ (the inverse of equivalent nodal loads on starting node) and $W_0$ (the severity of distributed uniform load)

  

  

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

  

  

# Equivalent Nodal Loads

  

  

Consider a beam with ends fixed or released

  

- $F_0$ and $F_1$ are vertical forces applied to both ends from support

  

- $M_0$ and $M_1$ are moment applied to both ends from support

  

- $\theta_0$ and $\theta_1$ are rotation at both ends

  

- $\Delta_0$ and $\Delta_1$ are displacement at both ends

  

  

for a fixed or released end we have a constraint:

  

one or several of $F$, $M$, $\theta$ or $\Delta$ is zero for each end based on end release condition of beam.

  

  

- Shear: $V(x)=\int{W(x).dx}+F_0$

  

- Moment: $M(x)=\int{V(x).dx}+M_0=\int {\int{W(x).dx}+F_0.x+M_0}$

  

- Rotation: $\theta(x)=\int{\frac{M(x)}{E(x).I(x)}.dx}+\theta_0=\int {\frac{\int {\int{W(x).dx}+F_0.x+M_0}}{EI(x)}}+\theta_0$

  

- Displacement: $\Delta(x)=\int{\theta(x).dx}+\Delta_0=\int \int {\frac{\int {\int{W(x).dx}+F_0.x+M_0}}{EI(x)}}+\theta_0$

  

  

## For partial Load

$W(x) = w.(U_1-U_2)$

$\int W(x) = \int w . U_1-\int w . U_2$
$=w^{(1)}.U_1-w_1^{(1)}.U_1-w^{(1)}.U_2+w_2^{(1)}.U_2$

$\int \int W(x) =$
$=\int w^{(1)}.U_1-\int w_1^{(1)}.U_1-\int w^{(1)}.U_2+\int w_2^{(1)}.U_2$

- $\int w^{(1)}.U_1 = w^{(2)}.U_1-w_1^{(2)}.U_1$
- $\int w_1^{(1)}.U_1=w^{(1)}.(x-x_1).U_1=w^{(1)}.x.U_1-w^{(1)}.x_1.U_1$
- $\int w^{(1)}.U_2 = w^{(2)}.U_2-w_2^{(2)}.U_2$
- $\int w_1^{(1)}.U_2=w^{(1)}.(x-x_2).U_2=w^{(1)}.x.U_2-w^{(1)}.x_2.U_2$

$=w^{(2)}.U_1-w_1^{(2)}.U_1-w^{(1)}.x.U_1+w^{(1)}.x_1.U_1+w^{(2)}.U_2-w_2^{(2)}.U_2-w^{(1)}.x.U_2+w^{(1)}.x_2.U_2$

Note that $W(x)$ have 2 components, $\int W(x)$ have 4 components, $\int \int W(x)$ have 8 components,

If we approximate $\frac {1}{EI}$ with a polynomial which we call $Q=Q(x)$, then:

$\frac {\int \int W(x)}{EI}=Q.\int \int W(x)$
$=Q.w^{(2)}.U_1-Q.w_1^{(2)}.U_1-Q.w^{(1)}.x.U_1+Q.w^{(1)}.x_1.U_1+Q.w^{(2)}.U_2-Q.w_2^{(2)}.U_2-Q.w^{(1)}.x.U_2+Q.w^{(1)}.x_2.U_2$

lets call $Q.w^{(i)}$ as $z^{(i)}$ 

$=z^{(2)}.U_1-Q.w_1^{(2)}.U_1-z^{(1)}.x.U_1+Q.w^{(1)}.x_1.U_1+Q.w^{(2)}.U_2-Q.w_2^{(2)}.U_2-Q.w^{(1)}.x.U_2+Q.w^{(1)}.x_2.U_2$

$W(x) = w.(U_1-U_2)$
 
$V(x)=F_0 *U_0+ \int W(x).dx +F_L.U_L$

- note:  ${\int w . U_i .dx}=w^{(1)}.U_i-w_i^{(1)}.U_i$ where $w^{(i)}$ is $i$'th integral of $w(x)$

$V(x)=w^{(1)}.U_1-w_1^{(1)}.U_1-w^{(1)}.U_2+w_2^{(1)}.U_2 +F_0.U_0+F_1.U_1+F_2.U_2$
 
$M(x)=\int w^{(1)}.U_1-\int w_1^{(1)}.U_1-\int w^{(1)}.U_2+\int w_2^{(1)}.U_2 +\int F_0.U_0+\int F_1.U_1+\int F_2.U_2+M_0.U_0+M_1.U_1+M_2.U_2$


thus

$M(x) = w^{(2)}.U_1-w_1^{(2)}.U_1-w^{(1)}.x.U_1+w^{(1)}.x_1.U_1+w^{(2)}.U_2-w_2^{(2)}.U_2-w^{(1)}.x.U_2+w^{(1)}.x_2.U_2$

$\frac {M(x)}{EI} = \frac {w^{(2)}.U_1}{EI}-\frac {w_1^{(2)}.U_1}{EI}-\frac {w^{(1)}.x.U_1}{EI}+\frac {w^{(1)}.x_1.U_1}{EI}+\frac {w^{(2)}.U_2}{EI}-\frac {w_2^{(2)}.U_2}{EI}-\frac {w^{(1)}.x.U_2}{EI}+\frac {w^{(1)}.x_2.U_2}{EI}$



$\frac {M(x)}{EI} =  {w^{(2)}.U_1}{.Q}-{w_1^{(2)}.U_1}{.Q}- {w^{(1)}.x.U_1}{.Q}+ {w^{(1)}.x_1.U_1}{.Q}+ {w^{(2)}.U_2}{.Q}- {w_2^{(2)}.U_2}{.Q}- {w^{(1)}.x.U_2}{.Q}+ {w^{(1)}.x_2.U_2}{.Q}$

$\theta(x)=\theta_0+\int \frac{M(x)}{EI}= \int {w^{(2)}.U_1}{.Q}-\int {w_1^{(2)}.U_1}{.Q}-\int  {w^{(1)}.x.U_1}{.Q}+ \int {w^{(1)}.x_1.U_1}{.Q}+ \int {w^{(2)}.U_2}{.Q}- \int {w_2^{(2)}.U_2}{.Q}- \int {w^{(1)}.x.U_2}{.Q}+ \int {w^{(1)}.x_2.U_2}{.Q}$

$\theta(x) = \theta_0+\int w^{(2)}.U_1/EI-\int w_1^{(2)}.U_1-\int w^{(1)}.x.U_1+\int w^{(1)}.x_1.U_1+\int w^{(2)}.U_2-\int w_2^{(2)}.U_2-\int w^{(1)}.x.U_2+\int w^{(1)}.x_2.U_2$

$\int\int W(x).dx = \int(\int W(x).dx) = \int(\int w(x) * (U1-U2) .dx).dx$
${\int w(x) * (U1-U2) *dx}=\int w(x).U1 -\int w(x).U2$
$\int w*U_1=(w^{(1)}(x)-w^{(1)}(x_1))*U_1=w^{(1)}(x).U_1-w^{(1)}(x_1).U_1$

in next, will show  $w^{(i)}(x)$ with $w^{(i)}$ and $w^{(i)}(x_j)$ with $w_j^{(i)}$ for simplicity.
note that $w^{(i)}$ is a function of $x$ but $w_j^{(i)}$ is not a function of $x$ and is constant and get out of any integrals as coefficient

$\int \int w*U_1=\int w^{(1)}.U_1-\int w_1^{(1)}.U_1$
- $\int w^{(1)}.U_1 =(w^{(2)}-w_1^{(2)})*U_1$
- $\int w_1^{(1)}.U_1=w_1^{(1)}.(x-x_1)*U_1=(w_1^{(1)}.x-w_1^{(1)}.x_1)*U_1$

$\int \int w*U_1=w^{(2)}-w_1^{(2)}-(w_1^{(1)}.x-w_1^{(1)}.x_1)$

$=w^{(2)}-w_1^{(2)}-w_1^{(1)}.x+w_1^{(1)}.x_1$


  

$\theta(x)=\theta_0+M_0\int{\frac{ 1}{EI}} + F_0 \int{\frac{ x}{EI}} + \frac{ w_0}{2} \int \frac{ x^2}{EI}$

  

$\Delta(x)=\Delta_0+\theta_0*x+M_0\int\int{\frac{ 1}{EI}} + F_0 \int\int{\frac{ x}{EI}} + \frac{ w_0}{2} \int \int \frac{ x^2}{EI}$

  

  

$V(0)=V_0$

  

$M(0)=M_0$

  

$\theta(0)=\theta_0$

  

$\Delta(0)=\Delta_0$

  

  

$V(L)=F_0 + w_0*L$

  

$M(L)=M_0 + F_0*L + w_0*L^2/2$

  

$\theta(L)=\theta_0+M_0\int{\frac{1}{EI}} + F_0 \int{\frac{x}{EI}} + \frac{w_0}{2} \int \frac{x^2}{EI}$

  

$\Delta(L)=\Delta_0+\theta_0.x+M_0\int\int{\frac{ 1}{EI}} + F_0 \int\int{\frac{ x}{EI}} + \frac{ w_0}{2} \int\int \frac{ x^2}{EI}$

  

  

So we need to calculate:

  

- $\int _{0}^{L}{\frac{ 1}{EI}}dx$

  

- $\int _{0}^{L}{\frac{ x}{EI}}dx$

  

- $\int _{0}^{L}{\frac{ x^2}{EI}}dx$

  

- $\int _{0}^{L}(\int{\frac{ 1}{EI}}dx)dx$

  

- $\int _{0}^{L}(\int{\frac{ x}{EI}}dx)dx$

  

- $\int _{0}^{L}(\int{\frac{ x^2}{EI}}dx)dx$

  

  

Can estimate $\frac{ 1}{EI}$,$\frac{ x}{EI}$,$\frac{ x^2}{EI}$ with polynomials. it will be approximation then integrations are easy.

for uniform load, number of sample points to form polynomial:

  

$c=3*n+3*m$

- $n$: $E$'s degree

- $m$: $I$'s degree

-
