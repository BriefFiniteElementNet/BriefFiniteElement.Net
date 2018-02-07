AddAnalysisResult_v2 procedure
========
Solve procedure:

- n : Total number of nodes in the model
- Δ<sub>T</sub> : Total Displacement Vector (length = ``6*n``)
- F<sub>T</sub> : Total External Force Vector (length = ``6*n``)
- K<sub>T</sub> : Total Stiffness matrix (size = ``6*n`` by ``6*n``)


This procedure uses master-slave method for handling constraints, so two matrices named P<sub>Δ</sub> and R<sub>Δ</sub> can be generated regarding all constraints of model (both SPC and MPC constrains), so that:

Δ<sub>T</sub> = P<sub>Δ</sub> . Δ<sub>r</sub> - R<sub>Δ</sub>

in this equation:

- P<sub>Δ</sub> : ``6*n`` by ``m`` matrix, we call it Displacement Permutation matrix

- R<sub>Δ</sub> : ``6*n`` by ``1`` matrix (it is a vector)

- Δ<sub>r</sub> : ``m`` by ``1`` matrix (it is a vector), we call it Reduced Displacement Vector

In above matrices, ``m`` is number of master DoFs which is less or equal to ``6*n`` (total DoFs in model).

For relation between reduced and total vectors we have:

Δ<sub>T</sub> = P<sub>Δ</sub> . Δ<sub>r</sub> - R<sub>Δ</sub>

F<sub>r</sub> = P<sub>F</sub> . F<sub>T</sub>

P<sub>F</sub> = P<sub>Δ</sub> <sup>T</sup> 

From fundamental equations we have:

F<sub>T</sub> = K<sub>T</sub> . Δ<sub>T</sub>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;multiply both side with P<sub>F</sub>:

P<sub>F</sub> F<sub>T</sub> = P<sub>F</sub> K<sub>T</sub> (P<sub>Δ</sub> * Δ<sub>r</sub> - R<sub>Δ</sub>)

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Expand it:

P<sub>F</sub> F<sub>T</sub> = P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub> Δ<sub>r</sub> - P<sub>F</sub> K<sub>r</sub> R<sub>Δ</sub>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;substitute with prior equations:

P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub> Δ<sub>r</sub> - P<sub>F</sub> K<sub>r</sub> R<sub>Δ</sub> - P<sub>F</sub> F<sub>T</sub> = 0

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;we introduce K<sub>r</sub> (reduced stiffness matrix) as:

K<sub>r</sub> = P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;simplify:

K<sub>r</sub> Δ<sub>r</sub> = P<sub>F</sub> K<sub>T</sub> R<sub>Δ</sub> + P<sub>F</sub> F<sub>T</sub>

Δ<sub>r</sub> = K<sub>r</sub> <sup>-1</sup> P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>)

Δ<sub>T</sub> = P<sub>Δ</sub> Δ<sub>r</sub> - R<sub>Δ</sub>

= P<sub>Δ</sub> * K<sub>r</sub> <sup>-1</sup> P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>) - R<sub>Δ</sub>

And Final Result:

Δ<sub>T</sub> = P<sub>Δ</sub> * K<sub>r</sub> <sup>-1</sup> P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>) - R<sub>Δ</sub>

where K<sub>r</sub> <sup>-1</sup> is inverse of K<sub>r</sub>

Finding F<sub>T</sub> (force vector)
-----
for solving the above eq., we must have external forces vector that is applied into the master DoFs. It have two components: Equivalent Nodal Loads from loads on the element bodies, and external nodal loads applied on nodes itself:

F<sub>T</sub> = F<sub>e</sub> + F<sub>n</sub>

where:
- F<sub>T</sub>: Total external force
- F<sub>e</sub>: Total equivalent nodal loads from elements
- F<sub>n</sub>: Total nodal loads
- 
Finding Δ<sub>T</sub> (displacement vector) and support reactions
------

in the final equation all matrices are known so with arithmetic operations, Δ<sub>T</sub> vector can be calculated and system is solved.

Only exception here is when there is no master DoFs in the model, i.e. all DoFs are fixed into the ground as support. In this situation we will simply have:

Δ<sub>T</sub> = - R<sub>Δ</sub>

After finding Δ<sub>T</sub>, we will recover F<sub>T</sub> by:

F<sub>T</sub> = K<sub>T</sub> * Δ<sub>T</sub>

F<sub>T</sub> is total external loads vector that is applied into the model and it includes support reaction, equivalent nodal of elements and nodal loads:

F<sub>T</sub> = F<sub>c</sub> + F<sub>e</sub> + F<sub>n</sub>

where:
- F<sub>T</sub>: Total external force
- F<sub>c</sub>: Total support reactions vector
- F<sub>e</sub>: Total equivalent nodal loads from elements
- F<sub>n</sub>: Total nodal loads

so we can find support reaction vector:

F<sub>c</sub> = F<sub>T</sub> - F<sub>e</sub> - F<sub>n</sub> 
