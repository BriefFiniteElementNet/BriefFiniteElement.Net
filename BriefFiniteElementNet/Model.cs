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
    public sealed class Model:ISerializable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        public Model()
        {
            this.nodes = new NodeCollection(this);
            this.elements = new ElementCollection(this);
        }

        #endregion

        #region Members

        private NodeCollection nodes;
        private ElementCollection elements;
        private StaticLinearAnalysisResult lastResult;

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
            private set { lastResult = value; }
        }

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
            var str = File.OpenWrite(fileAddress);
            Save(str, model);
        }

        /// <summary>
        /// Saves the model to stream.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="model">The model.</param>
        public static void Save(Stream st, Model model)
        {
            var formatter = new BinaryFormatter() ;
            formatter.Serialize(st, model);
        }

        /// <summary>
        /// Loads the Model from specified file address.
        /// </summary>
        /// <param name="fileAddress">The file address.</param>
        /// <returns></returns>
        public static Model Load(string fileAddress)
        {
            var str = File.OpenRead(fileAddress);
            return Load(str);
        }

        /// <summary>
        /// Loads the <see cref="Model"/> from specified stream.
        /// </summary>
        /// <param name="str">The stream.</param>
        /// <returns></returns>
        public static Model Load(Stream str)
        {
            var formatter = new BinaryFormatter();

            var buf = (Model)formatter.Deserialize(str) ;


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
            var formatter = new BinaryFormatter() { Binder = new FemNetSerializationBinder() };

            var buf = (Model)formatter.Deserialize(str);


            return buf;
        }

        #endregion

        #region LinearSolve method and overrides

        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for default load case.
        /// </summary>
        public void Solve()
        {
            Solve(new SolverConfiguration(LoadCase.DefaultLoadCase));
        }


        /// <summary>
        /// Solves the instance with specified <see cref="SolverType" /> and assuming linear behavior (both geometric and material) and for default load case.
        /// </summary>
        /// <param name="solverType">The solver type.</param>
        public void Solve(SolverType solverType)
        {
            Solve(new SolverConfiguration(LoadCase.DefaultLoadCase) {SolverType = solverType});
        }


        /// <summary>
        /// Solves the instance with specified <see cref="solver" /> and assuming linear behavior (both geometric and material) and for default load case.
        /// </summary>
        /// <param name="solver">The solver.</param>
        public void Solve(ISolver solver)
        {
            Solve(new SolverConfiguration(LoadCase.DefaultLoadCase) { CustomSolver = solver});
        }

        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for specified cases.
        /// </summary>
        /// <param name="cases">The cases.</param>
        public void Solve(params LoadCase[] cases)
        {
            Solve(new SolverConfiguration(cases));
        }


        /// <summary>
        /// Solves the instance assuming linear behavior (both geometric and material) for specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void Solve(SolverConfiguration config)
        {
            TraceUtil.WritePerformanceTrace("Started solving model");

            var sp = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Index = i;

            var n = nodes.Count;
            var c = 6*n;


            var maxNodePerElement = elements.Select(i => i.Nodes.Length).Max();
            var rElmMap = new int[maxNodePerElement*6];
            var kt = new CoordinateStorage<double>(c, c, 1);

            #region Determining count of fixed and free DoFs

            var fixedDofCount =
                nodes.Select(
                    i =>
                        (int) i.Constraints.Dx + (int) i.Constraints.Dy + (int) i.Constraints.Dz +
                        (int) i.Constraints.Rx + (int) i.Constraints.Ry + (int) i.Constraints.Rz).Sum();

            var freeDofCount = c - fixedDofCount;

            TraceUtil.WritePerformanceTrace("Model with {0} free DoFs and {1} fixed DoFs", freeDofCount, fixedDofCount);

            #endregion

            var fMap = new int[c];
            var rMap = new int[c];
            var rrmap = new int[freeDofCount];
            var rfmap = new int[fixedDofCount];
            
            #region Assembling Kt

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6*i + 0] = elm.Nodes[i].Index*6 + 0;
                    rElmMap[6*i + 1] = elm.Nodes[i].Index*6 + 1;
                    rElmMap[6*i + 2] = elm.Nodes[i].Index*6 + 2;

                    rElmMap[6*i + 3] = elm.Nodes[i].Index*6 + 3;
                    rElmMap[6*i + 4] = elm.Nodes[i].Index*6 + 4;
                    rElmMap[6*i + 5] = elm.Nodes[i].Index*6 + 5;
                }

                var mtx = elm.GetGlobalStifnessMatrix();
                var d = c2*6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        kt.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }
            }

            sp.Stop();
            TraceUtil.WritePerformanceTrace("Assembling full stiffness matrix took about {0:#,##0} ms.",
                sp.ElapsedMilliseconds);
            sp.Restart();

            #endregion

            #region Extracting kff, kfs and kss

            var fixity = new bool[c];

            for (var i = 0; i < n; i++)
            {
                var cns = nodes[i].Constraints;

                if (cns.Dx == DofConstraint.Fixed) fixity[6*i + 0] = true;
                if (cns.Dy == DofConstraint.Fixed) fixity[6*i + 1] = true;
                if (cns.Dz == DofConstraint.Fixed) fixity[6*i + 2] = true;


                if (cns.Rx == DofConstraint.Fixed) fixity[6*i + 3] = true;
                if (cns.Ry == DofConstraint.Fixed) fixity[6*i + 4] = true;
                if (cns.Rz == DofConstraint.Fixed) fixity[6*i + 5] = true;
            }

            

            int fCnt = 0, rCnt = 0;

            /** /
            for (var i = 0; i < c; i++)
            {
                if (fixity[i])
                    fMap[i] = fCnt++;
                else
                    rMap[i] = rCnt++;
            }
            /**/

            /**/
            for (var i = 0; i < c; i++)
            {
                if (fixity[i])
                    rfmap[fMap[i] = fCnt++] = i;
                else
                    rrmap[rMap[i] = rCnt++] = i;
            }
            /**/

            var ktSparse = Converter.ToCompressedColumnStorage(kt);


            var ind = ktSparse.ColumnPointers;
            var v = ktSparse.RowIndices;
            var values = ktSparse.Values;

            var cnt = values.Count(i => i == 0.0);

            var kffCoord = new CoordinateStorage<double>(freeDofCount, freeDofCount, 128 + freeDofCount);
            var kfsCoord = new CoordinateStorage<double>(freeDofCount, fixedDofCount, 128);
            var ksfCoord = new CoordinateStorage<double>(fixedDofCount, freeDofCount, 128);
            var kssCoord = new CoordinateStorage<double>(fixedDofCount, fixedDofCount, 128);

            var cnr = 0;

            for (var i = 0; i < ind.Length - 1; i++)
            {
                var st = ind[i];
                var en = ind[i + 1];

                for (var j = st; j < en; j++)
                {
                    cnr++;
                    var row = i;
                    var col = v[j];
                    var val = values[j];

                    if (!fixity[row] && !fixity[col])
                    {
                        kffCoord.At(rMap[row], rMap[col], val);
                        continue;
                    }

                    if (!fixity[row] && fixity[col])
                    {
                        kfsCoord.At(rMap[row], fMap[col], val);
                        continue;
                    }

                    if (fixity[row] && !fixity[col])
                    {
                        ksfCoord.At(fMap[row], rMap[col], val);
                        continue;
                    }

                    if (fixity[row] && fixity[col])
                    {
                        kssCoord.At(fMap[row], fMap[col], val);
                        continue;
                    }


                    Guid.NewGuid();

                }
            }

            var tmp = kffCoord.NonZerosCount + kfsCoord.NonZerosCount + ksfCoord.NonZerosCount + kssCoord.NonZerosCount;



            sp.Stop();
            TraceUtil.WritePerformanceTrace("Extracting kff,kfs and kss from Kt matrix took about {0:#,##0} ms",
                sp.ElapsedMilliseconds);
            sp.Restart();

            #endregion

            #region converting kff, Kfs and Kss to compressed column storage

            var kff = (CSparse.Double.CompressedColumnStorage) Converter.ToCompressedColumnStorage(kffCoord);
            var kfs = (CSparse.Double.CompressedColumnStorage) Converter.ToCompressedColumnStorage(kfsCoord);
            var kss = (CSparse.Double.CompressedColumnStorage) Converter.ToCompressedColumnStorage(kssCoord);

            sp.Stop();

            //TraceUtil.WritePerformanceTrace("cholesky decomposition of Kff took about {0:#,##0} ms",
            //    sp.ElapsedMilliseconds);

            
            TraceUtil.WritePerformanceTrace("nnz of kff is {0:#,##0}, ~{1:0.0000}%", kff.Values.Length,
                ((double) kff.Values.Length)/((double) kff.RowCount*kff.ColumnCount));

            sp.Restart();

            #endregion

            var result = new StaticLinearAnalysisResult();

            if (config.CustomSolver != null)
                result.Solver = config.CustomSolver;
            else
                result.Solver = GenerateSolver(config.SolverType, kff);

            result.Kfs = kfs;
            result.Kss = kss;
            result.Parent = this;
            result.SettlementsLoadCase = config.SettlementsLoadCase;
            
            result.ReleasedMap = rMap;
            result.FixedMap = fMap;
            result.ReversedReleasedMap = rrmap;
            result.ReversedFixedMap = rfmap;

            foreach (var cse in config.LoadCases)
            {
                result.AddAnalysisResult(cse);
            }


            this.lastResult = result;

            foreach (var elm in elements)
            {
                foreach (var nde in elm.Nodes)
                {
                    nde.ConnectedElements.Add(elm);
                }
            }
        }


        private static ISolver GenerateSolver(SolverType solverType, CompressedColumnStorage kff)
        {
            switch (solverType)
            {
                case SolverType.CholeskyDecomposition:
                    return new CholeskySolver() {A = kff};
                    break;
                case SolverType.ConjugateGradient:
                    return new PCG(new SSOR()) {A = kff};
                    break;
                default:
                    throw new ArgumentOutOfRangeException("solverType");
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
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Index = i;

            info.AddValue("elements", elements);
            info.AddValue("nodes", nodes);
        }

        private Model(SerializationInfo info, StreamingContext context)
        {
            elements = info.GetValue<ElementCollection>("elements");
            nodes = info.GetValue<NodeCollection>("nodes");
        }

        [OnDeserialized]
        private void ReassignNodeReferences(StreamingContext context)
        {
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

            
        }

        #endregion
    }
}
