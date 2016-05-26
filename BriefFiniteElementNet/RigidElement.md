# public bool HingedConnections{get;set;}
Usually rigid element connect all nodes together with full restrains, which means all DoFs of nodes (displacement + rotational) is connected to each other.
if ```HingedConnections = true```, then only displacement DoFs of nodes will connect to each other