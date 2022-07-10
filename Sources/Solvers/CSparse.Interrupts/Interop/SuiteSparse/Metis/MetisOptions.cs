
namespace CSparse.Interop.SuiteSparse.Metis
{
    using System;

    #region Option enums

    /// <summary>
    /// Specifies the partitioning method.
    /// </summary>
    public enum PartitioningType
    {
        /// <summary>
        /// Multilevel recursive bisectioning.
        /// </summary>
        RB,
        /// <summary>
        /// Multilevel k-way partitioning.
        /// </summary>
        KWAY
    }

    /// <summary>
    /// Specifies the type of objective.
    /// </summary>
    public enum ObjectiveType
    {
        /// <summary>
        /// Edge-cut minimization.
        /// </summary>
        CUT,
        /// <summary>
        /// Total communication volume minimization.
        /// </summary>
        VOL,
        /// <summary>
        /// Only used for graph ordering.
        /// </summary>
        NODE
    }

    /// <summary>
    /// Specifies the matching scheme to be used during coarsening.
    /// </summary>
    public enum CoarseningType
    {
        /// <summary>
        /// Random matching.
        /// </summary>
        RM,
        /// <summary>
        /// Sorted heavy-edge matching.
        /// </summary>
        SHEM
    }

    /// <summary>
    /// Determines the algorithm used during initial partitioning.
    /// </summary>
    public enum InitialPartitioningType
    {
        /// <summary>
        /// Grows a bisection using a greedy strategy.
        /// </summary>
        GROW,
        /// <summary>
        /// Computes a bisection at random followed by a refinement.
        /// </summary>
        RANDOM,
        /// <summary>
        /// Derives a separator from an edge cut.
        /// </summary>
        EDGE,
        /// <summary>
        /// Grow a bisection using a greedy node-based strategy.
        /// </summary>
        NODE
    }

    /// <summary>
    /// Determines the algorithm used for refinement.
    /// </summary>
    public enum RefinementType
    {
        /// <summary>
        /// FM-based cut refinement.
        /// </summary>
        FM,
        /// <summary>
        /// Greedy-based cut and volume refinement.
        /// </summary>
        GREEDY,
        /// <summary>
        /// Two-sided node FM refinement.
        /// </summary>
        SEP2SIDED,
        /// <summary>
        /// One-sided node FM refinement.
        /// </summary>
        SEP1SIDED
    }

    [Flags]
    public enum DebugLevel
    {
        NONE = 0,
        /// <summary>
        /// Shows various diagnostic messages
        /// </summary>
        INFO = 1,
        /// <summary>
        /// Perform timing analysis
        /// </summary>
        TIME = 2,
        /// <summary>
        /// Show the coarsening progress
        /// </summary>
        COARSEN = 4,
        /// <summary>
        /// Show the refinement progress
        /// </summary>
        REFINE = 8,
        /// <summary>
        /// Show info on initial partitioning
        /// </summary>
        IPART = 16,
        /// <summary>
        /// Show info on vertex moves during refinement
        /// </summary>
        MOVEINFO = 32,
        /// <summary>
        /// Show info on vertex moves during sep refinement
        /// </summary>
        SEPINFO = 64,
        /// <summary>
        /// Show info on minimization of subdomain connectivity
        /// </summary>
        CONNINFO = 128,
        /// <summary>
        /// Show info on elimination of connected components
        /// </summary>
        CONTIGINFO = 256,
        /// <summary>
        /// Show info related to wspace allocation
        /// </summary>
        MEMORY = 2048
    }

    #endregion

    /// <summary>
    /// Metis options.
    /// </summary>
    public class MetisOptions
    {
        private const int NOPTIONS = 40;

        #region Option definitions
        
        const int PTYPE = 0;
        const int OBJTYPE = 1;
        const int CTYPE = 2;
        const int IPTYPE = 3;
        const int RTYPE = 4;
        const int DBGLVL = 5;
        const int NITER = 6;
        const int NCUTS = 7;
        const int SEED = 8;
        const int NO2HOP = 9;
        const int MINCONN = 10;
        const int CONTIG = 11;
        const int COMPRESS = 12;
        const int CCORDER = 13;
        const int PFACTOR = 14;
        const int NSEPS = 15;
        const int UFACTOR = 16;
        const int NUMBERING = 17;

        // Used for command-line parameter purposes
        const int HELP = 18;
        const int TPWGTS = 19;
        const int NCOMMON = 20;
        const int NOOUTPUT = 21;
        const int BALANCE = 22;
        const int GTYPE = 23;
        const int UBVEC = 24;

        #endregion

        #region Option properties

        /// <summary>
        /// Specifies the partitioning method.
        /// </summary>
        public PartitioningType Partitioning
        {
            get
            {
                return raw[PTYPE] < 0 ? PartitioningType.KWAY : (PartitioningType)raw[PTYPE];
            }
            set
            {
                raw[PTYPE] = (int)value;
            }
        }

        /// <summary>
        /// Specifies the type of objective.
        /// </summary>
        public ObjectiveType Objective
        {
            get
            {
                return raw[OBJTYPE] < 0 ? ObjectiveType.CUT : (ObjectiveType)raw[OBJTYPE];
            }
            set
            {
                raw[OBJTYPE] = (int)value;
            }
        }

        /// <summary>
        /// Specifies the matching scheme to be used during coarsening.
        /// </summary>
        public CoarseningType Coarsening
        {
            get
            {
                return raw[CTYPE] < 0 ? CoarseningType.RM : (CoarseningType)raw[CTYPE];
            }
            set
            {
                raw[CTYPE] = (int)value;
            }
        }

        /// <summary>
        /// Determines the algorithm used during initial partitioning.
        /// </summary>
        public InitialPartitioningType InitialPartitioning
        {
            get
            {
                return raw[IPTYPE] < 0 ? InitialPartitioningType.EDGE : (InitialPartitioningType)raw[IPTYPE];
            }
            set
            {
                raw[IPTYPE] = (int)value;
            }
        }

        /// <summary>
        /// Determines the algorithm used for refinement.
        /// </summary>
        public RefinementType Refinement
        {
            get
            {
                return raw[RTYPE] < 0 ? RefinementType.FM : (RefinementType)raw[RTYPE];
            }
            set
            {
                raw[RTYPE] = (int)value;
            }
        }

        /// <summary>
        /// Specifies the number of different partitionings that it will compute
        /// (default is 1).
        /// </summary>
        public int Cuts
        {
            get
            {
                return raw[NCUTS];
            }
            set
            {
                raw[NCUTS] = value;
            }
        }

        /// <summary>
        /// Specifies the number of different separators that it will compute at 
        /// each level of nested dissection (default is 1).
        /// </summary>
        public int Seperators
        {
            get
            {
                return raw[NSEPS];
            }
            set
            {
                raw[NSEPS] = value;
            }
        }

        /// <summary>
        /// Specifies the number of iterations for the refinement algorithms at 
        /// each stage of the uncoarsening process.
        /// </summary>
        public int Iterations
        {
            get
            {
                return raw[NITER];
            }
            set
            {
                raw[NITER] = value;
            }
        }

        /// <summary>
        /// Specifies the seed for the random number generator.
        /// </summary>
        public int Seed
        {
            get
            {
                return raw[SEED];
            }
            set
            {
                raw[SEED] = value;
            }
        }

        /// <summary>
        /// Specifies that the coarsening will not perform any 2–hop matchings when the standard matching approach
        /// fails to sufficiently coarsen the graph. The 2–hop matching is very effective for graphs with power-law
        /// degree distributions.
        /// </summary>
        public bool No2Hop
        {
            get
            {
                return raw[NO2HOP] == 1;
            }
            set
            {
                raw[NO2HOP] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Specifies that the partitioning routines should try to minimize the maximum
        /// degree of the subdomain graph, i.e., the graph in which each partition is
        /// a node, and edges connect subdomains with a shared interface.
        /// </summary>
        public bool MinConnect
        {
            get
            {
                return raw[MINCONN] == 1;
            }
            set
            {
                raw[MINCONN] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Specifies that the partitioning routines should try to produce partitions
        /// that are contiguous. Note that if the input graph is not connected this option
        /// is ignored.
        /// </summary>
        public bool Contiguous
        {
            get
            {
                return raw[CONTIG] == 1;
            }
            set
            {
                raw[CONTIG] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Specifies that the graph should be compressed by combining together vertices 
        /// that have identical adjacency lists.
        /// </summary>
        public bool Compress
        {
            get
            {
                return raw[COMPRESS] == 1;
            }
            set
            {
                raw[COMPRESS] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Specifies if the connected components of the graph should first be identified 
        /// and ordered separately.
        /// </summary>
        public bool ComponentOrder
        {
            get
            {
                return raw[CCORDER] == 1;
            }
            set
            {
                raw[CCORDER] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Specifies the minimum degree of the vertices that will be ordered last.
        /// </summary>
        public int PFactor
        {
            get
            {
                return raw[PFACTOR];
            }
            set
            {
                raw[PFACTOR] = value;
            }
        }

        /// <summary>
        /// Specifies the maximum allowed load imbalance among the partitions.
        /// </summary>
        public int UFactor
        {
            get
            {
                return raw[UFACTOR];
            }
            set
            {
                raw[UFACTOR] = value;
            }
        }

        /// <summary>
        /// Specifies the partitioning method.
        /// </summary>
        public DebugLevel DebugLevel
        {
            get
            {
                return raw[DBGLVL] <= 0 ? DebugLevel.NONE : (DebugLevel)raw[DBGLVL];
            }
            set
            {
                raw[DBGLVL] = (int)value;
            }
        }

        #endregion

        internal int[] raw;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetisOptions"/> class.
        /// </summary>
        public MetisOptions()
        {
            // METIS_SetDefaultOptions sets all values to -1.
            raw = Helper.CreateArray(NOPTIONS, -1);

            raw[NSEPS] = 1;
            raw[DBGLVL] = (int)DebugLevel.NONE;
        }

        #region Static methods

        /// <summary>
        /// Creates a new instance of the <see cref="MetisOptions"/> class with default graph partitioning options.
        /// </summary>
        public static MetisOptions GraphPartitionDefault()
        {
            var opt = new MetisOptions();

            opt.Partitioning = PartitioningType.KWAY;
            opt.Objective = ObjectiveType.CUT;
            opt.Coarsening = CoarseningType.SHEM;
            opt.InitialPartitioning = InitialPartitioningType.GROW;
            opt.Refinement = RefinementType.GREEDY;

            opt.No2Hop = false;
            opt.MinConnect = false;
            opt.Contiguous = false;

            opt.Cuts = 1;
            opt.Seperators = 1;
            opt.Iterations = 10;
            
            opt.Seed = -1;
            opt.DebugLevel = DebugLevel.NONE;

            opt.UFactor = -1;

            return opt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MetisOptions"/> class with default mesh partitioning options.
        /// </summary>
        public static MetisOptions MeshPartitionDefault()
        {
            var opt = GraphPartitionDefault();
            
            // Use the same defaults as for graph partitioning.

            return opt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MetisOptions"/> class with default nested dissection ordering options.
        /// </summary>
        public static MetisOptions NestedDissectionDefault()
        {
            var opt = new MetisOptions();

            opt.Coarsening = CoarseningType.SHEM;
            opt.InitialPartitioning = InitialPartitioningType.NODE;
            opt.Refinement = RefinementType.SEP2SIDED;

            opt.UFactor = 200;
            opt.PFactor = 0;
            opt.Compress = true;
            opt.ComponentOrder = false;
            opt.No2Hop = false;

            opt.Cuts = 1;
            opt.Seperators = 1;
            opt.Iterations = 10;

            opt.Seed = -1;
            opt.DebugLevel = DebugLevel.NONE;

            return opt;
        }

        #endregion
    }
}
