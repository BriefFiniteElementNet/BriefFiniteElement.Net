syms xi L Iz Iy J A E G

syms dx1 dy1 dz1 rx1 ry1 rz1 dx2 dy2 dz2 rx2 ry2 rz2
l=L;
N1 = 1/4 *(1-xi)^2 *(2+xi);
M1 = L/8 *(1-xi)^2 *(xi+1);
N2 = 1/4 *(1+xi)^2 *(2-xi);
M2 = L/8 *(1+xi)^2 *(xi-1);

bzN = [N1 M1 N2 M2]%beam Z shape function
byN = [N1 -M1 N2 -M2]%beam Y shape function

ttN = [(1+xi)/2 (1-xi)/2];%truss and torsion shape function
ttB = expand(1/ttJ*diff(ttN,xi));%torsion and truss consistency matrix, eq 4.18, p.245 (258 of pdf) ref[2]

bJ = (l/2);%jacobian ?x/?xi for beam 
ttJ = (l/2);%jacobian ?x/?xi for rod

bzNp = expand(1/bJ*diff(bzN,xi,1));%?x/?xi for beam Z
byNp = expand(1/bJ*diff(byN,xi,1));%?x/?xi for beam Y

byB = expand(1/bJ^2*diff(byN,xi,2));%eq 4.18, p.245 (258 of pdf) ref[2], B is RN/Rx not Rn/Rxi
bzB = expand(1/bJ^2*diff(bzN,xi,2));%eq 4.18, p.245 (258 of pdf) ref[2], B is RN/Rx not Rn/Rxi

%b3=[-1 1]*l;
%b4=[-1 1]*l;

% 1: Beam with Iy inertia
p1 = zeros(12, 4);
p1(3,1)=1;
p1(5,2)=1;
p1(9,3)=1;
p1(11,4)=1;
s1 = [dz1,ry1,dz2,ry2];
a1 = p1*transpose(s1);
b1 = byB * transpose(p1);
n1 = byN * transpose(p1);
n1p = byNp * transpose(p1);
d1 = E*Iy;

% 2: Beam with Iz inertia
p2 = zeros(12, 4);
p2(2,1)=1;
p2(6,2)=1;
p2(8,3)=1;
p2(12,4)=1;
s2 = [dy1,rz1,dy2,rz2];
a2 = p2*transpose(s2);
b2 = bzB * transpose(p2);
n2 = bzN * transpose(p2);
n2p = bzNp * transpose(p2);
d2 = E*Iz;

% 3: truss
p3 = zeros(12, 2);
p3(1,1)=1;
p3(7,2)=1;
s3 = [dx1,dx2];
a3 = p3*transpose(s3);
b3 = ttB * transpose(p3);
n3 = ttN * transpose(p3);

d3 = E*A;

% 4: torsion 
p4 = zeros(12, 2);
p4(4,1)=1;
p4(10,2)=1;
s4 = [rx1,rx2];
a4 = p4*transpose(s4);
b4 = ttB * transpose(p4);
n4 = ttN * transpose(p4);
d4 = G*J;


a=a1+a2+a3+a4;
b=[b1; b2; b3; b4];

n = n1+n2+n3+n4;

d=[d1 0 0 0; 0 d2 0 0; 0 0 d3 0; 0 0 0 d4];

ktruss = int(ttJ*transpose(ttB)*d3*(ttB),xi,-1,1);

kbeamz = int(bJ*transpose(bzB)*d2*(bzB),xi,-1,1);
kbeamy = int(bJ*transpose(byB)*d1*(byB),xi,-1,1);

kbeamzGeo = int(bJ*transpose(bzNp)*bzNp,xi,-1,1);
kbeamyGeo = int(bJ*transpose(byNp)*byNp,xi,-1,1);



kt = int(bJ*transpose(b)*d*(b),xi,-1,1);

disp(kt);
disp(b);
disp(n);

%refs:
%ref[2]: "Finite Element Analysis" by S.S.BHAVIKATTI, A NEW AGE INTERNATIONAL PUBLISHERS