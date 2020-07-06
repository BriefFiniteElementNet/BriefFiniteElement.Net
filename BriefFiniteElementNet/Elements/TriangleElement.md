# GetLocalInternalForce()

public CauchyStressTensor GetLocalInternalForce(IsoPoint location)
Gets the specified internal force at specified location. element is 2D but `location.Lambda` is used in third dimensions.
Triangle element can have bending behaviour, it means there is internal moment and stress is not constant through the thickness of the shell. so at the buttom of plate where Lambda=-1, at the center where Lambda=0 and at the top where Lambda=1, there are different cauchy stresses.
