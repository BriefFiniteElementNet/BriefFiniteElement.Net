using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Solver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using Trace = BriefFiniteElementNet.Common.Trace;


namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a structure which consists of nodes, elements and loads applied on its parts (parts means either nodes or elements)
    /// </summary>
    [Serializable]
    public sealed class Model : ISerializable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        public Model()
        {
            this.nodes = new NodeCollection(this);
            this.elements = new ElementCollection(this);
            this.rigidElements = new RigidElementCollection(this);
            this.mpcElements = new MpcElementCollection(this);
            InitPool();
        }

        #endregion

        #region Members

        private NodeCollection nodes;
        private ElementCollection elements;
        private MpcElementCollection mpcElements;

        [NonSerialized()]
        private StaticLinearAnalysisResult lastResult;
        [Obsolete("use MpcElements instead")]
        private RigidElementCollection rigidElements;
        //[Obsolete("use MpcElements instead")]
        //private TelepathyLinkCollection telepathyLinks;
        

        private LoadCase settlementLoadCase;


        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes.
        /// </value>
        public NodeCollection Nodes
        {
            get { return nodes; }
            set
            {
                nodes = value;
                if (value != null)
                    value.Parent = this;
            }
        }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        public ElementCollection Elements
        {
            get { return elements; }
            set
            {
                elements = value;
                if (value != null)
                    value.Parent = this;
            }
        }

        /// <summary>
        /// Gets the mpc elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        public MpcElementCollection MpcElements
        {
            get { return mpcElements; }
            set
            {
                mpcElements = value;
                if (value != null)
                    value.Parent = this;
            }
        }

        /// <summary>
        /// Gets the LastResult.
        /// </summary>
        /// <value>
        /// The result of last static analysis of model.
        /// </value>
        public StaticLinearAnalysisResult LastResult
        {
            get { return lastResult; }
            set { lastResult = value; }
        }

        public RigidElementCollection RigidElements
        {
            get { return rigidElements; }
        }


        /// <summary>
        /// Gets or sets the settlement load case.
        /// </summary>
        /// <value>
        /// The load case associated with settlements.
        /// </value>
        public LoadCase SettlementLoadCase
        {
            get { return settlementLoadCase; }
            set { settlementLoadCase = value; }
        }




        #region Trace property and field

        public  Trace Trace
        {
            get { return _trace; }
            private set { _trace = value; }
        }

       [NonSerialized]
        private Trace _trace = new Trace();

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="label"/> is valid for new <see cref="StructurePart"/> or not.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>yes if is valid, otherwise no</returns>
        internal bool IsValidLabel(string label)
        {
            foreach (var elm in elements)
            {
                if (FemNetStringCompairer.IsEqual(elm.Label, label))
                    return false;
            }


            foreach (var nde in nodes)
            {
                if (FemNetStringCompairer.IsEqual(nde.Label, label))
                    return false;
            }


            return true;
        }

        /// <summary>
        /// Saves the model to file.
        /// </summary>
        /// <param name="fileAddress">The file address.</param>
        /// <param name="model">The model.</param>
        public static void Save(string fileAddress, Model model)
        {
            using (var str = File.OpenWrite(fileAddress))
            {
                Save(str, model);
            }

        }


        /// <summary>
        /// Saves the model to stream.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="model">The model.</param>
        public static void Save(Stream st, Model model)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(st, model);
        }

        /// <summary>
        /// Serializes the model into a byte array.
        /// </summary>
        /// <param name="model">The model.</param>
        public static byte[] ToByteArray(Model model)
        {
            var memstr = new MemoryStream();

            Save(memstr, model);

            return memstr.GetBuffer();
        }

        /// <summary>
        /// Dematerializes the byte array into a <see cref="Model"/>.
        /// </summary>
        /// <param name="binaryData">The binary data.</param>
        /// <returns>retrieved model</returns>
        public static Model FromByteArray(byte[] binaryData)
        {
            var memstr = new MemoryStream(binaryData);

            var buf = Load(memstr);

            return buf;
        }

        /// <summary>
        /// Loads the Model from specified file address.
        /// </summary>
        /// <param name="fileAddress">The file address.</param>
        /// <returns></returns>
        public static Model Load(string fileAddress)
        {
            using (var str = File.OpenRead(fileAddress))
            {
                return Load(str);
            }
        }

        /// <summary>
        /// Loads the <see cref="Model"/> from specified stream.
        /// </summary>
        /// <param name="str">The stream.</param>
        /// <returns></returns>
        public static Model Load(Stream str)
        {
            var formatter = new BinaryFormatter();

            var buf = (Model) formatter.Deserialize(str);


            return buf;
        }

        /// <summary>
        /// Loads the with binder.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        [Obsolete("only Usage for VS Debugger display issue on 18 of July 2014")]
        public static Model LoadWithBinder(Stream str)
        {
            var formatter = new BinaryFormatter() {Binder = new FemNetSerializationBinder()};

            var buf = (Model) formatter.Deserialize(str);

            return buf;
        }



        /// <summary>
        /// Loads the model from binary stream using a serialization binder
        /// </summary>
        /// <param name="str"></param>
        /// <param name="binder"></param>
        /// <returns></returns>
        /// <remarks>Usually used for loading Model in older versions of BFE in new versions where a class is renamed in new BFE</remarks>
        public static Model LoadWithBinder(Stream str,SerializationBinder binder)
        {
            var formatter = new BinaryFormatter() { Binder = binder };

            var buf = (Model)formatter.Deserialize(str);

            return buf;
        }


        [Obsolete]
        private void PrecheckForErrors()
        {
            new ModelWarningChecker().CheckModel(this);
        }

        

        #endregion

        #region LinearSolve method and overrides

        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for default load case.
        /// </summary>
        public void Solve()
        {
            Solve(BuiltInSolverType.CholeskyDecomposition);
        }


        /// <summary>
        /// Solves the instance with specified <see cref="solverType" /> and assuming linear behavior (both geometric and material) and for default load case.
        /// </summary>
        /// <param name="solverType">The solver type.</param>
        public void Solve(BuiltInSolverType solverType)
        {
            Solve(CalcUtil.CreateBuiltInSolverFactory(solverType));
        }

        /*
        /// <summary>
        /// Solves the model using specified solver generator.
        /// </summary>
        /// <param name="solverGenerator">The solver generator.</param>
        [Obsolete()]
        public void Solve(Func<SparseMatrix, ISolver> solverGenerator)
        {
            throw new NotImplementedException();
        }
        */

        /// <summary>
        /// Solves the model using specified solver factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public void Solve(ISolverFactory factory)
        {
            var cfg = new SolverConfiguration();

            cfg.SolverFactory = factory;

            cfg.LoadCases.AddRange(new List<LoadCase>() { LoadCase.DefaultLoadCase });

            Solve(cfg);
        }

        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for specified cases.
        /// </summary>
        /// <param name="cases">The cases.</param>
        public void Solve(params LoadCase[] cases)
        {
            var cfg = new SolverConfiguration();

            cfg.SolverFactory = new CholeskySolverFactory();//if not defined, Cholesky by default

            cfg.LoadCases.AddRange(new List<LoadCase>(cases)   );

            Solve(cfg);
        }

        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void Solve(SolverConfiguration config)
        {
            //new version
            lastResult = new StaticLinearAnalysisResult();
            lastResult.Parent = this;
            
            lastResult.SolverFactory = config.SolverFactory;

            ReIndexNodes();

            if (this.mpcElements.Count > 0)
                throw new InvalidOperationException("Invalid solve for MPC element");// Model with MPC element should call Model.Solve_MPC()

            foreach (var loadCase in config.LoadCases)
            {
                lastResult.AddAnalysisResultIfNotExists(loadCase);
            }
        }


        public void Solve_MPC(SolverConfiguration config)
        {
            //new version

            if (lastResult == null)
            {
                lastResult = new StaticLinearAnalysisResult();
                lastResult.Parent = this;
                lastResult.SolverFactory = config.SolverFactory;
            }
                

            //if(elements.Any(i=>i is RigidElement))
            //    throw new Exception("Invalid solve for MPC element");// Model with RigidElement element should call Model.Solve()

            ReIndexNodes();

            
            foreach (var loadCase in config.LoadCases)
            {
                lastResult.AddAnalysisResultIfNotExists_MPC(loadCase);
            }
            
        }

        public void Solve_MPC(params LoadCase[] cases)
        {
            var fact = CalcUtil.CreateBuiltInSolverFactory(BuiltInSolverType.CholeskyDecomposition);

            var cfg = new SolverConfiguration();

            cfg.SolverFactory = fact;

            cfg.LoadCases.AddRange(cases);

            Solve_MPC(cfg);

        }

        public void Solve_MPC()
        {
            var fact = CalcUtil.CreateBuiltInSolverFactory(BuiltInSolverType.CholeskyDecomposition);

            var cfg = new SolverConfiguration();

            cfg.SolverFactory = fact;

            cfg.LoadCases.AddRange(new List<LoadCase>() { LoadCase.DefaultLoadCase });

            Solve_MPC(cfg);
        }


        /// <summary>
        /// Re-indexes the nodes incrementally.
        /// </summary>
        public void ReIndexNodes()
        {
            for (int i = 0; i < this.Nodes.Count; i++)
                this.Nodes[i].Index = i;
        }

        /// <summary>
        /// Re-indexes the elements and MPC elements incrementally.
        /// </summary>
        public void ReIndexElements()
        {
            for (int i = 0; i < this.Elements.Count; i++)
                this.Elements[i].Index = i;

            for (int i = 0; i < this.MpcElements.Count; i++)
                this.MpcElements[i].Index = i;
        }

        #endregion

        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Index = i;

            foreach (var elm in Elements)
            {
                foreach (var node in elm.Nodes)
                {
                    if (!ReferenceEquals(null, node))
                        if (!this.nodes.Contains(node))
                            throw new Exception("Node not belong to Model!");
                }
            }

            info.AddValue("elements", elements);
            info.AddValue("mpcElements", mpcElements);
            info.AddValue("nodes", nodes);

            info.AddValue("rigidElements", rigidElements);
            //info.AddValue("telepathyLinks", telepathyLinks);
            info.AddValue("settlementLoadCase", settlementLoadCase);
        }

        private Model(SerializationInfo info, StreamingContext context)
        {
            Elements = (ElementCollection)info.GetValue("elements",typeof(ElementCollection));
            MpcElements = (MpcElementCollection)info.GetValue("mpcElements",typeof(MpcElementCollection));

            rigidElements = (RigidElementCollection)info.GetValue("rigidElements",typeof(RigidElementCollection));
            //telepathyLinks = (TelepathyLinkCollection)info.GetValue("telepathyLinks", typeof(TelepathyLinkCollection));

            Nodes = (NodeCollection)info.GetValue("nodes",typeof(NodeCollection));
            settlementLoadCase = (LoadCase)info.GetValue("settlementLoadCase", typeof(LoadCase));

            InitPool();
        }

        [OnDeserialized]
        private void ReassignNodeReferences(StreamingContext context)
        {
            #region Element's nodes

            foreach (var elm in elements)
            {
                //elm.ReAssignNodeReferences(this);

                for (int i = 0; i < elm.nodeNumbers.Length; i++)
                {
                    if (elm.nodeNumbers[i] != -1)
                        elm.Nodes[i] = this.nodes[elm.nodeNumbers[i]];
                }

                elm.Parent = this;
                elm.nodeNumbers = null;
            }

            foreach (var nde in nodes)
                nde.Parent = this;

            #endregion

            #region RigidElement's nodes

            foreach (var relm in rigidElements)
            {
                relm.ReAssignNodeReferences(this);
            }

            #endregion

            #region mpcElements's nodes


            foreach (var relm in mpcElements)
            {
                relm.ReAssignNodeReferences(this);
            }

            #endregion

            /*
            #region UniformSurfaceLoadFor3DElement's surface nodes

            foreach (var elm in elements)
            {
                foreach (var ld in elm.Loads)
                {
                    if (ld is UniformSurfaceLoadFor3DElement)
                    {
                        var sld = ld as UniformSurfaceLoadFor3DElement;

                        if (sld.SurfaceNodes == null || sld.SurfaceNodes.Length != 3)
                            sld.SurfaceNodes = new Node[3];


                        for (var i = 0; i < sld.SurfaceNodes.Length; i++)
                        {
                            sld.SurfaceNodes[i] = this.nodes[sld.nodeNumbers[i]];
                        }
                    }
                }
            }

            #endregion
            */
            

        }


        #endregion

        #region pooling

        [NonSerialized]
        public ArrayPool<double> ArrayPool;

        [NonSerialized]
        public MatrixPool MatrixPool;

        /// <summary>
        /// Initiates the matrix and array pools
        /// </summary>
        private void InitPool()
        {
            ArrayPool = new ArrayPool<double>();
            MatrixPool = new MatrixPool(ArrayPool);
        }
        #endregion

        #region peace add 
        /// <summary>
        /// 获得具有某label的node
        /// Find and returns the node with certain label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public Node NodeWithLabel(string label)
        {
            Node node = null;
            foreach (Node nd in nodes)
            {
                if (nd.Label == label)
                {
                    node = nd;
                    break;
                }
            }

            return node;
        }
        /// <summary>
        /// 获得具有某label的element
        /// Find and returns the element with certain label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public Element ElementWithLabel(string label)
        {
            Element element = null;
            foreach (Element elm in elements)
            {
                if (elm.Label == label)
                {
                    element = elm;
                    break;
                }
            }

            return element;
        }
        #endregion
    }
}
