using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an abstract class for a 'Finite Element' with physical properties.
    /// </summary>
    [Serializable]
    //[CLSCompliant(true)]
    [DebuggerDisplay("{this.GetType().Name}, Label: {Label}")]
    
    public abstract class Element : StructurePart
    {
        /// <summary>
        /// An index through all elements, sets incrementally when Model.ReIndexElements() called
        /// </summary>
        [NonSerialized]
        private int index;
       

        #region pooling and caching

        /// <summary>
        /// Gets the matrix pool
        /// </summary>
        /// <remarks>
        /// The reference to matrix pool.
        /// </remarks>
        public MatrixPool MatrixPool
        {
            get
            {
                if (parent != null)
                    return parent.MatrixPool;

                return new MatrixPool(new ArrayPool<double>());
            }
        }

        
        [NonSerialized]
        public int CacheHit;
        
        [NonSerialized]
        public int CacheMiss;

        [NonSerialized]
        private Dictionary<string, object> cache = new Dictionary<string, object>();

        /// <summary>
        /// Gets the object from cache
        /// </summary>
        /// <typeparam name="T">type of expected object</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">output object</param>
        /// <returns></returns>
        public bool TryGetCache<T>(string key, out T value)
        {
            //value = default(T);
            //return false;

            lock (cache)
            {
                object obj;

                if (cache.TryGetValue(key, out obj))
                {
                    if (obj is T)
                    {
                        value = (T)obj;
                        CacheHit++;
                        return true;
                    }
                }

                CacheMiss++;
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Sets the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetCache(string key, object value)
        {
            lock (cache)
            {
                cache[key] = value;
            }
        }

        #endregion






        protected List<ElementalLoad> loads = new List<ElementalLoad>();

        /// <summary>
        /// Gets the loads.
        /// </summary>
        /// <value>
        /// The loads.
        /// </value>
        public List<ElementalLoad> Loads
        {
            get { return loads; }
        }

        //public List<ElementalLoad> LoAds { get; set; }//test clscompliant

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes.
        /// </value>
        public Node[] Nodes
        {
            get { return nodes; }
            private set { nodes = value; }
        }

        [NonSerialized]
        protected Node[] nodes;

        /// <summary>
        /// node numbers for deserialization process
        /// </summary>
        internal int[] nodeNumbers;


        /// <summary>
        /// Gets the stiffness matrix of member in global coordination system.
        /// </summary>
        /// <returns>The stiffness matrix</returns>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Element.Nodes"/>!</remarks>
        public abstract Matrix GetGlobalStifnessMatrix();


        /// <summary>
        /// Gets the stiffness matrix of member in global coordination system.
        /// </summary>
        /// <returns>The stiffness matrix</returns>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Element.Nodes"/>!</remarks>
        public abstract void GetGlobalStifnessMatrix(Matrix stiffness);

        public abstract int GetGlobalStifnessMatrixDimensions();

        /// <summary>
        /// Gets the mass matrix of element in global coordination system.
        /// </summary>
        /// <returns>The mass matrix</returns>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Element.Nodes"/>!</remarks>
        public abstract Matrix GetGlobalMassMatrix();


        /// <summary>
        /// Gets the damping matrix in global coordination system.
        /// </summary>
        /// <returns>the damping matrix</returns>
        public abstract Matrix GetGlobalDampingMatrix();        


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="nodes">The number of nodes that the <see cref="Element"/> connect together.</param>
        protected Element(int nodes)
        {
            this.nodes = new Node[nodes];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        protected Element()
        {
        }



        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.nodeNumbers = new int[this.nodes.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                nodeNumbers[i] = ReferenceEquals(nodes[i], null) ? -1 : nodes[i].Index;
            }
                

            //info.AddValue("elementType", (int)elementType);
            info.AddValue("loads", loads);
            info.AddValue("nodeNumbers", nodeNumbers);
            info.AddValue("_massFormulationType", (int) _massFormulationType);

            base.GetObjectData(info, context);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected Element(SerializationInfo info, StreamingContext context):base(info,context)
        {
            nodeNumbers = (int[]) info.GetValue("nodeNumbers", typeof(int[]));

            //elementType = (ElementType)info.GetInt32("elementType");
            loads = (List<ElementalLoad>) info.GetValue("loads", typeof(List<ElementalLoad>));


            foreach (var pair in info)
            {
                if (pair.Name == "_massFormulationType")
                    _massFormulationType = (MassFormulation) (int) info.GetValue("_massFormulationType", typeof(int));
            }
            

            this.nodes = new Node[nodeNumbers.Length];
        }


        #region MassFormulationType property and field

        private MassFormulation _massFormulationType;

        /// <summary>
        /// Gets or sets the type of the mass formulation.
        /// </summary>
        /// <value>
        /// The type of the mass formulation.
        /// </value>
        public MassFormulation MassFormulationType
        {
            get { return _massFormulationType; }
            set { _massFormulationType = value; }
        }

        public int Index
        {
            get { return index; }
            internal set
            {
                index = value;
            }
        }



        #endregion

        /// <summary>
        /// Gets the equivalent nodal loads due to specified <see cref="load"/>.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <returns>Equivalent nodal loads in global coordinate system</returns>
        public abstract Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load);

        /// <summary>
        /// Gets the lambda matrix of element (for transforming between local and global axis).
        /// For more info see TransformManagerL2G.md file
        /// </summary>
        /// <remarks>
        /// lambda * Local = Global
        /// </remarks>
        /// <returns></returns>
        public abstract Matrix GetLambdaMatrix();

        /// <summary>
        /// Gets the local to global (and vice versa) transformation manager for this element.
        /// </summary>
        /// <returns>the trasformation manager related to this element</returns>
        public TransformManagerL2G GetTransformationManager()
        {
            var lambda = GetLambdaMatrix();

            return TransformManagerL2G.MakeFromLambdaMatrix(lambda, MatrixPool);
        }

        /// <summary>
        /// Converts the Isometric coordinates to local coordinates.
        /// </summary>
        /// <param name="isoCoords">The isometric coords in element.</param>
        /// <returns>The location in element's local system system</returns>
        public abstract double[] IsoCoordsToLocalCoords(params double[] isoCoords);

        /// <summary>
        /// Gets the element helpers for the Element
        /// </summary>
        /// <returns></returns>
        public abstract IElementHelper[] GetHelpers();


        /// <summary>
        /// Used for reassigning nodes references after deserialization
        /// </summary>
        /// <param name="parent"></param>
        public override void ReAssignNodeReferences(Model parent)
        {
            var elm = this;

            for (int i = 0; i < elm.nodeNumbers.Length; i++)
            {
                var idx = elm.nodeNumbers[i];
                elm.nodes[i] = idx == -1 ? null : parent.Nodes[idx];
            }

            base.ReAssignNodeReferences(parent);
        }
        
    }
}
