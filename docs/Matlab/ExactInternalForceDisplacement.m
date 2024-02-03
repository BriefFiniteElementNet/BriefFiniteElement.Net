pkg load symbolic;
pkg load control;

clear

syms w0(x) xs xe L v0 m0 teta0 delta0 x zeta;

#syms a b c d

#w0 = c*x+d

w =  ( heaviside(x-xs)- heaviside(x-xe))*w0 # Uxs is step function of Xs and Uxe is step function of Xe

v = int(w,x) + v0

m = int(v,x) + m0

teta = int(m,x) + teta0

delta0 = int(teta,x) + delta0

#heaviside(x-xs)
expand(delta0)