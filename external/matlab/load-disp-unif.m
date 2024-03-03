# uniform EI for euler bernouly beam
# concentraled load
clear

syms x ei L xt vt mt
syms v0 m0

# concentrated force with (ft and mt) applied in xt location


v_1 = v0
v_2 = v0+vt

m_1 = m0+int(v_1,x)
m_2 = subs(m_1,x,xt)+int(v_2,x,xt,x)+mt

t_1 = int(m_1,x)
t_2 = subs(t_1,x,xt)+int(m_2,x,xt,x)

d_1 = int(t_1,x)
d_2 = subs(d_1,x,xt)+int(t_2,x,xt,x)

tl = subs(t_2,x,L)
dl = subs(d_2,x,L)

tmp = solve(tl==0,dl==0,v0,m0)

tmp.v0

a = xt

b = L-a

tmp2 = -vt*(L+2*a)*b^2/L^3  # from https://www.youtube.com/watch?app=desktop&v=3Y6JXG2K12Y for test
tmp2M = vt*a*b*b/L^2       # from https://www.youtube.com/watch?app=desktop&v=3Y6JXG2K12Y for test 

disp 'sssssssssssssssssssss'
expand(tmp.v0 - tmp2)
expand(tmp.m0 - tmp2M)

sympref display flat

expand(tmp.v0)
expand(tmp.m0)

subs(m_2,[x,m0],[L,tmp.m0])

subs(d_1,[m0,v0],[tmp.m0,tmp.v0])

d_1
# m0*x**2/2 + v0*x**3/6
d_2
# m0*xt**2/2 + v0*xt**3/6 + x**3*(v0/6 + vt/6) + x**2*(m0/2 + mt/2 - vt*xt/2) + x*(-mt*xt + vt*xt**2/2) - xt**3*(v0/6 + vt/6) - xt**2*(m0/2 + mt/2 - vt*xt/2) - xt*(-mt*xt + vt*xt**2/2)