from sympy import *
from sympy.abc import x
from sympy import Function, Symbol
import sympy

from pathlib import Path

def print_tree(self, level=0):
    print('\t' + str(level) + repr(self.value))
    for child in self.children:
        child.other_name(level + 1)


init_printing() # sympy expression printing

Qx2 =   Function("Qx2")(x)
Qx =   Function("Qx")(x)
Q =   Function("Q")(x)

F0 =   Function("F0")
F1 =   Function("F1")
F2 =   Function("F2")
F3 =   Function("F3")
F4 =   Function("F4")
F5 =   Function("F5")





u0 =    Symbol("u0")
u1 =    Symbol("u1")
u2 =    Symbol("u2")
u3 =    Symbol("u3")


x =   Symbol("x")

x0 =   Symbol("x0")
x1 =   Symbol("x1")
x2 =   Symbol("x2")
x3 =   Symbol("x3")


alpha =   Symbol("alpha")
beta =   Symbol("beta")

f0 =   Symbol("f0")
m0 =   Symbol("m0")
f1 =   Symbol("f1")
m1 =   Symbol("m1")
fl =   Symbol("fl")
ml =   Symbol("ml")



def Integrate(exp):
  exp = expand(exp)
  
  if(type(exp) is sympy.core.mul.Mul):
    print("is MULL")
    pprint(exp)
    return INTR(exp)
  
  
  buf = sympy.core.add.Add();
  
  if(type(exp) is sympy.core.add.Add):
    print("is ADD")
    pprint(exp.args)
    
    for ar in exp.args:
      buf = Add(buf,Integrate(ar))
      #return buf
    return buf

  print(exp.func)
  
  
def INTR(exp):
  
  exp = expand(exp)
  
  if(type(exp) is not sympy.core.mul.Mul):
    print("E2")      
    print(exp)      
    print(exp.func)      
    
    
  a = Wild("a",exclude=[x])
  n = Wild("n",properties=[lambda k: k.is_Integer])    

#  m0 = Wild("m0",properties=[lambda k: k.is_Integer])
#  m1 = Wild("m1",properties=[lambda k: k.is_Integer])    
#  m2 = Wild("m2",properties=[lambda k: k.is_Integer])    
#  m3 = Wild("m3",properties=[lambda k: k.is_Integer])    
#  m4 = Wild("m4",properties=[lambda k: k.is_Integer])    
#  m5 = Wild("m5",properties=[lambda k: k.is_Integer])    
  
#  cu0 = Wild("cu0",properties=[lambda k: k.is_Integer])    
#  cu1 = Wild("cu1",properties=[lambda k: k.is_Integer])    
#  cu2 = Wild("cu2",properties=[lambda k: k.is_Integer])    
#  cu3 = Wild("cu3",properties=[lambda k: k.is_Integer])    

#  ca = Wild("ca",properties=[lambda k: k.is_Integer])    
#  cb = Wild("cb",properties=[lambda k: k.is_Integer])      
#  */
  
  ca = int(alpha in exp.free_symbols)
  cb = int(beta in exp.free_symbols)  

  cf0x = int(F0(x) in exp.free_symbols)  
  cf1x = int(F1(x) in exp.free_symbols)  
  cf2x = int(F2(x) in exp.free_symbols)  
  cf3x = int(F3(x) in exp.free_symbols)  
  cf4x = int(F4(x) in exp.free_symbols)  
  cf5x = int(F5(x) in exp.free_symbols)  

  cu0 = int(u0 in exp.free_symbols)  
  cu1 = int(u1 in exp.free_symbols)  
  cu2 = int(u2 in exp.free_symbols)  
  cu3 = int(u3 in exp.free_symbols)  

  cq = int(Q in exp.free_symbols)
  cx = int(x in exp.free_symbols)  
  
  r = exp.match(a * x**n)
  
  if(r != None):
    n = r.get(n)
  else:
    n = 0
    
#  t = [ca,cb,m0,m1,m2,m3,m4,m5,cu0,cu1,cu2,cu3,cq,cx]
#  r1 = (F0(x) if m0 else 1)*(F1(x) if m1 else 1)*(F2(x) if m2 else 1)*(F3(x) if m3 else 1)*(F4(x) if m4 else 1)
#  r2 = (u0 if cu0 else 1)*(u1 if cu1 else 1)*(u2 if cu2 else 1)*(u3 if cu3 else 1)
#  r3 = (alpha if ca else 1)*(beta if cb else 1)
#  r4 = (Q if cq else 1)
#  r5 = (x**n if n>0 else 1)
  
  
    
#  ui = r2
  
#  if(m0!=0):
#    mm=0
  
#  if(m1!=0):
#    mm=1
    
#  if(m2!=0):
#    mm=2
    
#  if(m3!=0):
#    mm=3
    
#  if(m4!=0):
#    mm=4
        
  cms = Matrix([cf0x,cf1x,cf2x,cf3x,cf4x])
  vms = Matrix([F0(x),F1(x),F2(x),F3(x),F4(x)])
  cus = Matrix([cu0,cu1,cu2,cu3])
  vus = Matrix([u0,u1,u2,u3])
  cabs = Matrix([ca,cb])
  vabs = Matrix([alpha,beta])
  cqx = Matrix([cq,cx])
  vqx = Matrix([Q,x**n])
    
    
  pprint(cms)
  pprint(vms)  
  pprint(exp)
  exit(0)
  recons = cms*vms
  zero = expand(exp-recons)
  #pprint(zero)
  
  if zero != 0:
    print("Error identify parts")
    pprint(exp)
    pprint(zero)
    exit(0)
    
  def replaceBoolWithInt(arr):#matrixfy boolean array
    arr2=arr.copy()
    
    for idx, val_ in enumerate(arr2):
      arr2[idx]=1 if arr2[idx] else 0
    
    return Matrix(arr2)
    
  cms = matrixify(cms)
  cus = matrixify(cus)
  cabs = matrixify(cabs)
  cqx = matrixify(cqx)
  
  pprint (cms)
  pprint (cus)
  pprint (cabs)  
  pprint (cqx)
  exit(0)   

  if(sum(cms) != 0 and n!=0):
    print("ERROR");

  print("input");
  pprint(exp)
  
  m = sum(cms)
  u = sum(cus)
  
  if(u > 1):
    print("ERROR");
    print("u: "+str(u))
    exit(0)

  if(m > 1):
    print("ERROR");
    print("m: "+str(m))
    exit(0)    
    
  if(m==0 and n==0 ):
      ret =  a*ui*(x-x0)
      
  elif(n!=0 and m==0):
      ret =  a*ui*(x**(n+1)-x0**(n+1))/(n+1)

  elif(m!=0 and n==0):
    F_ = Function("F"+str(mm+1))
    ret =  a*ui*(F_(Symbol('x'))-F_(x0))
    
  else:
      print("alg errror: n:"+str(n)+", m:"+str(m))
      
  print("output:")
  pprint(ret)
  
  return ret
  

cnt = 2


ir = expand(parse_expr("F0(x)*u"))


fileObj = open(r"tmp.txt", "w")

fileObj.write("Input:")
fileObj.write("\r\n")
fileObj.write(str(ir))
fileObj.write("\r\n")
fileObj.write("\r\n")

  
for i in range(cnt):
  ir = expand(Integrate(ir))
  print("Level: "+str(i+1))
  pprint((ir))
  fileObj.write("Level " + str(i+1) + ":")
  fileObj.write("\r\n")
  fileObj.write(str(ir))
  fileObj.write("\r\n")
  
print("Final: ")
pprint((ir))
fileObj.close()
#for x in range(1, 1):
#t=diff(f(x0),x0)

    
#print(expand(t))
