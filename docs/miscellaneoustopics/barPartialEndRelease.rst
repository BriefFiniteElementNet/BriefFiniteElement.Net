Partial End release of Bar Element
##################################

## Beam Behaviour

For beam we have 4 shape functions, namely $N_1$, $N_2$, $M_1$, $M_2$
Also we have 4 conditions for each of these functions:

.. code-block::

    |#|condition| $N_1$ |$N_2$  |$M_1$ |$M_2$|
    |-|--|--|--|--|--|
    |1|$F(\xi=-1)$|1  |0  |0 |0
    |2|$\frac {dF}{d\xi}(\xi=-1)$|0  |0  |1|0
    |3|$F(\xi=+1)$| 0 | 1 |0 | 0
    |4|$\frac {dF}{d\xi}(\xi=+1)$| 0 | 0 | 0|1
    
    For example for $N_1$ we have four conditions
    
    - $N_1(\xi=-1)=1$
    - $N_1'(\xi=-1)=0$
    - $N_1(\xi=1)=0$
    - $N_1'(\xi=1)=0$
    
    Right side is column $N_1$.
    
    Now if we have partial release, then appropriated row will be eliminated.
    For example if $\Delta_{left}$ of left node is released, then the row #1 will be eliminated.
    For $\theta_{left}$, $\Delta_{right}$ and $\theta_{right}$ the rows 2 to 4 will be eliminated.
    If we assume order 3 for each shape function, then
    