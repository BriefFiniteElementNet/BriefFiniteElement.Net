#integrating in 2D

for computing 


        / A₂   / F₂(γ)                                                                              
       |      |         
  I =  |      |         G( ξ,η,γ ) . dξ . dη . dγ                                                             
       |      |                                                                               
      / A₁   / F₁(γ)   


simply set:
	G₂(η,γ) = (nu,gamma) => 1
	G₁(η,γ) = (nu,gamma) => 0
	GammaPointCount = 1

#Sampling count and polynomial order
In fact we can integrate an ```2N+1``` degree polynomial exactly with only ```N+1``` integration points (ref[1])

If ```φ=φ(ξ)``` is a polynomial, n-point (nth order) Gauss quadratureyields the exact integral if φ is of degree 2n-1 or less. (A cubicpolynomial is exactly integrated by a two-point rule, three-pointrule, etc.) 

ref[1]: https://www3.nd.edu/~coast/jjwteach/www/www/30125/pdfnotes/lecture16_19v17.pdf
ref[2]: 