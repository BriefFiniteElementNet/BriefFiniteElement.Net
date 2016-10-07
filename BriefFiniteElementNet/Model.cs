using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using BriefFiniteElementNet.CSparse;
using BriefFiniteElementNet.CSparse.Double;
using BriefFiniteElementNet.CSparse.Double.Factorization;
using BriefFiniteElementNet.CSparse.Storage;
using BriefFiniteElementNet.Solver;

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
        }

        #endregion

        #region Members

        private NodeCollection nodes;
        private ElementCollection elements;
        private StaticLinearAnalysisResult lastResult;
        private RigidElementCollection rigidElements;
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

        public Trace Trace
        {
            get { return _trace; }
            private set { _trace = value; }
        }

       
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
            var gen = new Func<CompressedColumnStorage, ISolver>(i =>
            {
                var sl = CalcUtil.CreateBuiltInSolver(solverType);
                sl.A = i;
                return sl;
            });

            Solve(gen);
        }

        /*
        /// <summary>
        /// Solves the instance with specified <see cref="solver" /> and assuming linear behavior (both geometric and material) and for default load case.
        /// </summary>
        /// <remarks>
        /// This is not possible to use this, </remarks>
        /// <param name="solver">The solver.</param>
        [Obsolete("use Solve(Func<CompressedColumnStorage, ISolver> solverGenerator) instead")]
        public void Solve(ISolver solver)
        {
            PrecheckForErrors();

            var cfg = new SolverConfiguration();

            cfg.CustomSolver = solver;

            cfg.LoadCases = new List<LoadCase>() { LoadCase.DefaultLoadCase };

            Solve(cfg);
        }*/

        /// <summary>
        /// Solves the model using specified solver generator.
        /// </summary>
        /// <param name="solverGenerator">The solver generator.</param>
        public void Solve(Func<CompressedColumnStorage, ISolver> solverGenerator)
        {
            var cfg = new SolverConfiguration();

            cfg.SolverGenerator = solverGenerator;

            cfg.LoadCases = new List<LoadCase>() { LoadCase.DefaultLoadCase };

            Solve(cfg);
        }

        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for specified cases.
        /// </summary>
        /// <param name="cases">The cases.</param>
        public void Solve(params LoadCase[] cases)
        {
            var cfg = new SolverConfiguration();

            cfg.SolverGenerator = i =>
            {
                var sl = CalcUtil.CreateBuiltInSolver(BuiltInSolverType.CholeskyDecomposition);
                sl.A = i;
                return sl;
            };

            cfg.LoadCases = new List<LoadCase>(cases);

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
            lastResult.SolverGenerator = config.SolverGenerator;
            
            foreach (var loadCase in config.LoadCases)
            {
                lastResult.AddAnalysisResultIfNotExists(loadCase);
            }
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
                    if (!this.nodes.Contains(node))
                        throw new Exception("Node not belong to Model!");
                }
            }

            info.AddValue("elements", elements);
            info.AddValue("nodes", nodes);
            info.AddValue("rigidElements", rigidElements);
            info.AddValue("settlementLoadCase", settlementLoadCase);
        }

        private Model(SerializationInfo info, StreamingContext context)
        {
            elements = info.GetValue<ElementCollection>("elements");
            rigidElements = info.GetValue<RigidElementCollection>("rigidElements");
            nodes = info.GetValue<NodeCollection>("nodes");
            settlementLoadCase = info.GetValue<LoadCase>("settlementLoadCase");
        }

        [OnDeserialized]
        private void ReassignNodeReferences(StreamingContext context)
        {
            #region Element's nodes

            foreach (var elm in elements)
            {
                for (int i = 0; i < elm.nodeNumbers.Length; i++)
                {
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
                for (int i = 0; i < relm.nodeNumbers.Length; i++)
                {
                    relm.Nodes.Add(this.nodes[relm.nodeNumbers[i]]);
                }

                if(relm._centralNodeNumber.HasValue)
                {
                    relm.CentralNode = this.nodes[relm._centralNodeNumber.Value];
                }
            }

            #endregion

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

        }


        #endregion

    }
}
