clc
clear
pkg load symbolic
syms v0 v1 v2 v3
syms m0 m1 m2 m3
syms x0 x1 x2 x3 ei
syms w0
syms x L
syms Ft Mt

ei=1;
# uniform load

v1 = 0;
v2 = 0;

m1 = 0;
m2 = 0;

x1 = 0;
x0 = 0;
x2 = L;
x3 = L;


f0 = @(XX) w0;
f1 = @(XX) w0*XX;
f2 = @(XX) w0*XX^2/2;

q0 = @(XX) ei;
#r0 = @(XX) XX * ei*0;

q1 = @(XX) XX*ei;
r1 = @(XX) XX^2*ei/2;#????

q2 = @(XX) XX^2/2*ei;
r2 = @(XX) XX^3/2*ei/3;#????

t0 = @(XX) ei * w0 / 2 * XX^2;
t1 = @(XX) ei * w0 / 2 * XX^3 / 3;
t2 = @(XX) ei * w0 / 2 * XX^4 / 12;


Vs = +v0
Ve = +f1(L)-f1(x1)-f1(L)+f1(x2)+v0+v1+v2

Ms = +v0*0-v0*x0+m0
Me = +f2(L)-f2(x1)-f1(x1)*L+f1(x1)*x1-f2(L)+f2(x2)+f1(x2)*L-f1(x2)*x2+v0*L-v0*x0+v1*L-v1*x1+v2*L-v2*x2+m0+m1+m2

Ts = +v0*r1(0)-v0*r1(x0)-v0*x0*q1(0)+v0*x0*q1(x0)+m0*q1(0)-m0*q1(x0)
Te = +t1(L)-t1(x1)-f2(x1)*q1(L)+f2(x1)*q1(x1)-f1(x1)*r1(L)+f1(x1)*r1(x1)+f1(x1)*x1*q1(L)-f1(x1)*x1*q1(x1)-t1(L)+t1(x2)+f2(x2)*q1(L)-f2(x2)*q1(x2)+f1(x2)*r1(L)-f1(x2)*r1(x2)-f1(x2)*x2*q1(L)+f1(x2)*x2*q1(x2)+v0*r1(L)-v0*r1(x0)-v0*x0*q1(L)+v0*x0*q1(x0)+v1*r1(L)-v1*r1(x1)-v1*x1*q1(L)+v1*x1*q1(x1)+v2*r1(L)-v2*r1(x2)-v2*x2*q1(L)+v2*x2*q1(x2)+m0*q1(L)-m0*q1(x0)+m1*q1(L)-m1*q1(x1)+m2*q1(L)-m2*q1(x2)

Ds = +v0*r2(0)-v0*r2(x0)-v0*r1(x0)*0+v0*r1(x0)*x0-v0*x0*q2(0)+v0*x0*q2(x0)+v0*x0*q1(x0)*0-v0*x0*q1(x0)*x0+m0*q2(0)-m0*q2(x0)-m0*q1(x0)*0+m0*q1(x0)*x0
De = +t2(L)-t2(x1)-t1(x1)*L+t1(x1)*x1-f2(x1)*q2(L)+f2(x1)*q2(x1)+f2(x1)*q1(x1)*L-f2(x1)*q1(x1)*x1-f1(x1)*r2(L)+f1(x1)*r2(x1)+f1(x1)*r1(x1)*L-f1(x1)*r1(x1)*x1+f1(x1)*x1*q2(L)-f1(x1)*x1*q2(x1)-f1(x1)*x1*q1(x1)*L+f1(x1)*x1*q1(x1)*x1-t2(L)+t2(x2)+t1(x2)*L-t1(x2)*x2+f2(x2)*q2(L)-f2(x2)*q2(x2)-f2(x2)*q1(x2)*L+f2(x2)*q1(x2)*x2+f1(x2)*r2(L)-f1(x2)*r2(x2)-f1(x2)*r1(x2)*L+f1(x2)*r1(x2)*x2-f1(x2)*x2*q2(L)+f1(x2)*x2*q2(x2)+f1(x2)*x2*q1(x2)*L-f1(x2)*x2*q1(x2)*x2+v0*r2(L)-v0*r2(x0)-v0*r1(x0)*L+v0*r1(x0)*x0-v0*x0*q2(L)+v0*x0*q2(x0)+v0*x0*q1(x0)*L-v0*x0*q1(x0)*x0+v1*r2(L)-v1*r2(x1)-v1*r1(x1)*L+v1*r1(x1)*x1-v1*x1*q2(L)+v1*x1*q2(x1)+v1*x1*q1(x1)*L-v1*x1*q1(x1)*x1+v2*r2(L)-v2*r2(x2)-v2*r1(x2)*L+v2*r1(x2)*x2-v2*x2*q2(L)+v2*x2*q2(x2)+v2*x2*q1(x2)*L-v2*x2*q1(x2)*x2+m0*q2(L)-m0*q2(x0)-m0*q1(x0)*L+m0*q1(x0)*x0+m1*q2(L)-m1*q2(x1)-m1*q1(x1)*L+m1*q1(x1)*x1+m2*q2(L)-m2*q2(x2)-m2*q1(x2)*L+m2*q1(x2)*x2

tmp = solve(Ds == 0, De == 0, Ts == 0, Te == 0 ,v0, m0)