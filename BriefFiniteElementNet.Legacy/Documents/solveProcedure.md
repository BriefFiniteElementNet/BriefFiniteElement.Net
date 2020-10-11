Prodecure of solving model in BFE
===

Step1 : Create P<sub>Δ</sub>, R<sub>Δ</sub> and P<sub>F</sub> matrices to apply boundary conditions.

Step2 : Assemble K<sub>T</sub>, create and fill F<sub>T</sub> for known forces.

Step3 : Form K<sub>R</sub>, Δ<sub>r</sub> and F<sub>T</sub>:

K<sub>R</sub> = P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub>
F<sub>r</sub> = P<sub>F</sub> . F<sub>T</sub>

Step4: Find Δ<sub>T</sub> from:

Δ<sub>T</sub> = P<sub>Δ</sub> * K<sub>r</sub> ^ {-1} P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>) - R<sub>Δ</sub>

Thats all yet!