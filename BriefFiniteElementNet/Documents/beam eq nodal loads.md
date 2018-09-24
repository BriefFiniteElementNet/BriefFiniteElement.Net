

# equivalent nodal loads for concentrated load on beam

## concentrated force (no moments).
based on ``https://www.slideshare.net/AnasCivil/table-of-fixed-end-moments-formulas`` octave code:

	syms l a b x;
	m0 = a*b^2/l^2;
	m1 = -a^2*b/l^2;
	v0 = b^2*(3*a+b)/l^3;
	v1 = a^2*(3*b+a)/l^3;
	
	nn1 = 1-3*(x/l)^2+2*(x/l)^3;
	mm1 = x-2*(x^2/l)+(x^3/l^2);
	
	nn2 = 3*(x/l)^2-2*(x/l)^3;
	mm2 = -x^2/l+x^3/l^2;
	
	
	nm0 = subs(expand(subs(m0,b,l-a)),a,x)-mm1;disp(nm0);
	nm1 = subs(expand(subs(m1,b,l-a)),a,x)-mm2;disp(nm1);
	
	nv0 = subs(expand(subs(v0,b,l-a)),a,x)-nn1;disp(nv0);
	nv1 = subs(expand(subs(v1,b,l-a)),a,x)-nn2;disp(nv1);

result: 

m0 = p * mm1

m1 = p * mm2

v0 = p * nn1

v1 = p * nn2

	
where nn1,nn2,mm1,mm2 are hermitian shape functions of beam 

## concentrated moment (no force).
based on ``https://www.slideshare.net/AnasCivil/table-of-fixed-end-moments-formulas`` octave code:

	syms l a b x;
	m0 = b*(2*a-b)/l^2;
	m1 = a*(2*b-a)/l^2;
	v0 = 6*a*b/l^3;
	v1 = -6*a*b/l^3;
	
	nn1 = 1-3*(x/l)^2+2*(x/l)^3;
	mm1 = x-2*(x^2/l)+(x^3/l^2);
	
	nn2 = 3*(x/l)^2-2*(x/l)^3;
	mm2 = -x^2/l+x^3/l^2;
	
	
	nm0 = subs(expand(subs(m0,b,l-a)),a,x)+diff(mm1,x,1);disp(nm0);
	nm1 = subs(expand(subs(m1,b,l-a)),a,x)+diff(mm2,x,1);disp(nm1);
	
	nv0 = subs(expand(subs(v0,b,l-a)),a,x)+diff(nn1,x,1);disp(nv0);
	nv1 = subs(expand(subs(v1,b,l-a)),a,x)+diff(nn2,x,1);disp(nv1);
	
	
result: (unrendered version looks right!)

m0 = -m1' * M

m1 = -m2' * M

v0 = -n1' * M

v1 = -n2' * M
	
where n1,n2,m1,m2 are shape functions of beam, M is concentrated moment maGNITUDE
