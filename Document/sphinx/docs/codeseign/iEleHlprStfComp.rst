IElementHelper.AddStiffnessComponents()
=======================================
Current procedure for assembling full stiffness matrix is to create a stiffness matrix for each element, then translate each DoF and assemble in global matrix.
but can be changed to pass coordinate storage into each element with a DoFTranslator, and element put a coordinate for specified element. 
good thing about is that matrix are not several times when integrating.
Not a good idea, because local stiffness is calculated then transforms to global stiffness, components are not computed separately