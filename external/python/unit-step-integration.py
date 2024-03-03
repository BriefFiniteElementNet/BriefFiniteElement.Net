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

F0 =   Function("F0")(x)
F1 =   Function("F1")(x)
F2 =   Function("F2")(x)
F3 =   Function("F3")(x)
F4 =   Function("F4")(x)
F5 =   Function("F5")(x)

u =    Symbol("u")
x =   Symbol("x")
x0 =   Symbol("x0")

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

  m0 = Wild("m0",properties=[lambda k: k.is_Integer])
  m1 = Wild("m1",properties=[lambda k: k.is_Integer])    
  m2 = Wild("m2",properties=[lambda k: k.is_Integer])    
  m3 = Wild("m3",properties=[lambda k: k.is_Integer])    
  m4 = Wild("m4",properties=[lambda k: k.is_Integer])    
  m5 = Wild("m5",properties=[lambda k: k.is_Integer])    
  
  r = exp.match(F0**m0 * F1**m1 * F2**m2 * F3**m3 * F4**m4 * F5**m5 * x**n * u * a)
  
  pprint("MTCH")
  pprint(r) 
  pprint(exp)  
  
  if(r == None):
    print("ERROR")
    print(exp)
    return 0
    
  
  m0 = r.get(m0)
  m1 = r.get(m1)
  m2 = r.get(m2)
  m3 = r.get(m3)
  m4 = r.get(m4) 
  a = r.get(a)  
  n = r.get(n)  
  
  if(m0!=0):
    mm=0
  
  if(m1!=0):
    mm=1
    
  if(m2!=0):
    mm=2
    
  if(m3!=0):
    mm=3
    
  if(m4!=0):
    mm=4
        
  m=m0+m1+m2+m3+m4
  
  if(m != 0 and n!=0):
    print("ERROR");

  print("input");
  pprint(exp)
  

  
  if(n == None):
    n=0

  if(n==0 and m==0):
      ret =  a*u*(x-x0)
      
  elif(n!=0 and m==0):
      ret =  a*u*(x**(n+1)-x0**(n+1))/(n+1)

  elif(m!=0 and n==0):
    F_ = Function("F"+str(mm+1))
    ret =  a*u*(F_(Symbol('x'))-F_(x0))
    
  else:
      print("alg errror: n:"+str(n)+", m:"+str(m))
      
  print("output:")
  pprint(ret)
  
  return ret
  

cnt = 6


ir = F0*u

fileObj = open(r"exact-internal-force-displacement-output.txt", "w")

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
