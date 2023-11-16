import clr
import System
clr.AddReference("CSparse.dll")
clr.AddReference("BriefFiniteElementNet.dll")
clr.AddReference("BriefFiniteElementNet.Common.dll")
import BriefFiniteElementNet
from BriefFiniteElementNet import *

model = Model()
l = 5
n1 = Node(0, 0, 0)
n1.Label = "n1"
n2 = Node(0, 0, l)
n2.Label = "n2"

e1 = Elements.BarElement(n1, n2)
e1.Label = "e1"

h = 0.1; w = 0.05; a = h*w
iy = h*h*h*w / 12.0; iz = w*w*w*h / 12.0
j = iy+iz
e = 210e9; nu = 0.3; g = e/(2*1+nu)

sec1 = Sections.UniformParametric1DSection(a, iy, iz, j)
e1.Section = sec1
mat1 = Materials.UniformIsotropicMaterial.CreateFromYoungShear(e, g)
e1.Material = mat1
model.Nodes.Add(n1, n2)
model.Elements.Add(e1)

n1.Constraints = Constraint.Fixed

axialLoad = 1000
horizontalLoad = 1000
f = Force(horizontalLoad, 0, axialLoad, 0, 0, 0)
n2.Loads.Add(NodalLoad(f))

model.Solve()

d = model.Nodes[1].GetNodalDisplacement()
print d

expectedDx = (horizontalLoad*l*l*l)/(3*e*iy)
print "expectedDx = %.2e" %expectedDx
expectedRy = (horizontalLoad*l*l) / (2*e*iy)
print "expectedRy = %.2e" %expectedRy
expectedDz = axialLoad*l / (e*a)
print "expectedDz = %.2e" %expectedDz
