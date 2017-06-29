#Intro
```TelepathyLink``` is kind of connection that connects two DoF s of two different nodes in a way that both of those DoF s will have same displacement. 
Note that it is different than RigidElement, one difference is that it can partially connect nodes to each other, but RigidElement binds all DoF s of nodes together.
Another difference is if a Node have only rotation, then all other nodes will have displacement also (can you imagine?), but with this link, all connected nodes will have only rotation. 
When you connect two rotational DoF of two different nodes, it is like you connected those DoFs with gears.