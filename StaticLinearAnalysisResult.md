AddAnalysisResult_v2 procedure
========

Assuming:

Δ<sub>T</sub> = P<sub>Δ</sub> . Δ<sub>r</sub> - R<sub>Δ</sub>
F<sub>r</sub> = P<sub>F</sub> . F<sub>T</sub>

P<sub>F</sub> = P<sub>Δ</sub> <sup>T</sup> = 

And:

F<sub>T</sub> = K<sub>T</sub> . Δ<sub>T</sub>

P<sub>F</sub> F<sub>T</sub> = P<sub>F</sub> K<sub>T</sub> (P<sub>Δ</sub> * Δ<sub>r</sub> - R<sub>Δ</sub>)
P<sub>F</sub> F<sub>T</sub> = P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub> Δ<sub>r</sub> - P<sub>F</sub> K<sub>r</sub> R<sub>Δ</sub>
P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub> Δ<sub>r</sub> - P<sub>F</sub> K<sub>r</sub> R<sub>Δ</sub> - P<sub>F</sub> F<sub>T</sub> = 0

K<sub>r</sub> = P<sub>F</sub> K<sub>T</sub> P<sub>Δ</sub>

K<sub>r</sub> Δ<sub>r</sub> = P<sub>F</sub> K<sub>T</sub> R<sub>Δ</sub> + P<sub>F</sub> F<sub>T</sub>

Δ<sub>r</sub> = K<sub>r</sub> ^ {-1} P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>)
Δ<sub>T</sub> = P<sub>Δ</sub> Δ<sub>r</sub> - R<sub>Δ</sub>
= P<sub>Δ</sub> * K<sub>r</sub> ^ {-1} P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>) - R<sub>Δ</sub>

Final Result:
Δ<sub>T</sub> = P<sub>Δ</sub> * K<sub>r</sub> ^ {-1} P<sub>F</sub> (K<sub>T</sub> R<sub>Δ</sub> + F<sub>T</sub>) - R<sub>Δ</sub>
