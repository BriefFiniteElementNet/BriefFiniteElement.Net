using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the result of linear analysis of structure against defined load combinations
    /// </summary>
    //[Serializable]
    public class StaticLinearAnalysisResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticLinearAnalysisResult"/> class.
        /// </summary>
        public StaticLinearAnalysisResult()
        {
        }

        #endregion

        #region Fields

        private Model parent;
        private Dictionary<LoadCase, double[]> displacements = new Dictionary<LoadCase, double[]>();
        private Dictionary<LoadCase, double[]> forces = new Dictionary<LoadCase, double[]>();
        private LoadCase settlementsLoadCase;

        internal int[] ReleasedMap; //ReleasedMap[GlobalDofIndex] = DoF index in free DoFs
        internal int[] FixedMap; //FixedMap[GlobalDofIndex] = DoF index in fixed DoFs
        internal int[] ReversedReleasedMap; //ReversedReleasedMap[DoF index in free DoFs] = GlobalDofIndex
        internal int[] ReversedFixedMap; //ReversedFixedMap[DoF index in fixed DoFs] = GlobalDofIndex

        


        /// <summary>
        /// The cholesky decomposition of Kff, will be used for fast solving the model under new load cases
        /// </summary>
        //internal CSparse.Double.Factorization.SparseCholesky KffCholesky;
        //internal CSparse.Double.Factorization.SparseLDL KffLdl;


        #region Stiffness matrixes

        internal CompressedColumnStorage Kff;
        internal CompressedColumnStorage Kfs;
        internal CompressedColumnStorage Kss;

        #endregion


        public ISolver Solver;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the displacements.
        /// </summary>
        /// <value>
        /// The displacements of DoFs under each <see cref="LoadCase"/>.
        /// </value>
        /// <remarks>
        /// model under each load case may have different displacements vector for system. the key and value pair in <see cref="Displacements"/> property 
        /// contains the displacement or settlements of DoFs (for released dofs, it is displacement and for constrainted dofs it is settlements)
        /// </remarks>
        public Dictionary<LoadCase, double[]> Displacements
        {
            get { return displacements; }
            internal set { displacements = value; }
        }

        /// <summary>
        /// Gets the forces.
        /// </summary>
        /// <value>
        /// The forces on DoFs under with each <see cref="LoadCase"/>.
        /// </value>
        /// <remarks>
        /// each load case may have different loads vector for system. the key and value pair in <see cref="Forces"/> property 
        /// contains the external load or support reactions (for released dofs, it is external load and for constrainted dofs it is support reaction)
        /// </remarks>
        public Dictionary<LoadCase, double[]> Forces
        {
            get { return forces; }
            internal set { forces = value; }
        }

        internal Model Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Gets or sets the settlements load case.
        /// </summary>
        /// <value>
        /// The load case that settlements should be threated
        /// </value>
        internal LoadCase SettlementsLoadCase
        {
            get { return settlementsLoadCase; }
            set { settlementsLoadCase = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the analysis result if not exists.
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <remarks>If current instance do not contains the results related to <see cref="cse"/>, then this method will add result related to <see cref="cse"/> using <see cref="StaticLinearAnalysisResult.AddAnalysisResult"/> method</remarks>
        public void AddAnalysisResultIfNotExists(LoadCase cse)
        {
            if (displacements.ContainsKey(cse))
                return;

            AddAnalysisResult(cse);
        }

        /// <summary>
        /// Adds the analysis result.
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <remarks>if model is analyzed against specific load case, then displacements are available through <see cref="Displacements"/> property.
        /// If system is not analyses against a specific load case, then this method will analyses structure against <see cref="LoadCase"/>.
        /// While this method is using pre computed Cholesky Decomposition (the <see cref="StiffnessMatrixCholeskyDecomposition"/> is meant) , its have a high performance in solving the system.
        /// </remarks>
        public void AddAnalysisResult(LoadCase cse)
        {
            var sp = Stopwatch.StartNew();

            var haveSettlement = false;

            #region Determining force and displacement vectors

            var fixCount = this.Kfs.ColumnCount;
            var freeCount = this.Kfs.RowCount;

            var nodes = parent.Nodes;

            var uf = new double[freeCount];
            var pf = new double[freeCount];

            var us = new double[fixCount];
            var ps = new double[fixCount];

            var n = parent.Nodes.Count;

            #region Initializing Node.MembersLoads

            for (var i = 0; i < n; i++) parent.Nodes[i].MembersLoads.Clear();

            foreach (var elm in parent.Elements)
            {
                var nc = elm.Nodes.Length;

                foreach (var ld in elm.Loads)
                {
                    if (ld.Case != cse)
                        continue;

                    var frc = ld.GetGlobalEquivalentNodalLoads(elm);

                    for (int i = 0; i < nc; i++)
                    {
                        elm.Nodes[i].MembersLoads.Add(new NodalLoad(frc[i], cse));
                    }
                }
            }

            #endregion

            TraceUtil.WritePerformanceTrace("Calculating end member forces took {0} ms", sp.ElapsedMilliseconds);
            sp.Restart();



            var fmap = this.FixedMap;
            var rmap = this.ReleasedMap;


            for (int i = 0; i < n; i++)
            {
                var force = new Force();

                foreach (var ld in nodes[i].MembersLoads)
                    force += ld.Force;


                foreach (var ld in nodes[i].Loads)
                    if (ld.Case == cse)
                        force += ld.Force;


                var cns = nodes[i].Constraints;
                var disp = new Displacement();

                if (cse == this.SettlementsLoadCase) disp = nodes[i].Settlements;


                #region DX

                if (cns.Dx == DofConstraint.Released)
                {
                    pf[rmap[6*i + 0]] = force.Fx;
                    uf[rmap[6*i + 0]] = disp.Dx;
                }
                else
                {
                    ps[fmap[6*i + 0]] = force.Fx;
                    us[fmap[6*i + 0]] = disp.Dx;
                }

                #endregion

                #region DY

                if (cns.Dy == DofConstraint.Released)
                {
                    pf[rmap[6*i + 1]] = force.Fy;
                    uf[rmap[6*i + 1]] = disp.Dy;
                }
                else
                {
                    ps[fmap[6*i + 1]] = force.Fy;
                    us[fmap[6*i + 1]] = disp.Dy;
                }

                #endregion

                #region DZ

                if (cns.Dz == DofConstraint.Released)
                {
                    pf[rmap[6*i + 2]] = force.Fz;
                    uf[rmap[6*i + 2]] = disp.Dz;
                }
                else
                {
                    ps[fmap[6*i + 2]] = force.Fz;
                    us[fmap[6*i + 2]] = disp.Dz;
                }

                #endregion



                #region RX

                if (cns.Rx == DofConstraint.Released)
                {
                    pf[rmap[6*i + 3]] = force.Mx;
                    uf[rmap[6*i + 3]] = disp.Rx;
                }
                else
                {
                    ps[fmap[6*i + 3]] = force.Mx;
                    us[fmap[6*i + 3]] = disp.Rx;
                }

                #endregion

                #region Ry

                if (cns.Ry == DofConstraint.Released)
                {
                    pf[rmap[6*i + 4]] = force.My;
                    uf[rmap[6*i + 4]] = disp.Ry;
                }
                else
                {
                    ps[fmap[6*i + 4]] = force.My;
                    us[fmap[6*i + 4]] = disp.Ry;
                }

                #endregion

                #region Rz

                if (cns.Rz == DofConstraint.Released)
                {
                    pf[rmap[6*i + 5]] = force.Mz;
                    uf[rmap[6*i + 5]] = disp.Rz;
                }
                else
                {
                    ps[fmap[6*i + 5]] = force.Mz;
                    us[fmap[6*i + 5]] = disp.Rz;
                }

                #endregion
            }

            #endregion

            TraceUtil.WritePerformanceTrace("forming Uf,Us,Ff,Fs took {0} ms", sp.ElapsedMilliseconds);
            sp.Restart();

            #region Determining that have settlement or not

            for (int i = 0; i < fixCount; i++)
                if (us[i] != 0)
                {
                    haveSettlement = true;
                    break;
                }

            #endregion

            #region Solving equation system

            if (Solver == null)
                throw new NullReferenceException("Solver");

            for (int i = 0; i < fixCount; i++)
                ps[i] = 0; //no need existing values


            string message;

            if (!Solver.IsInitialized)
                Solver.Initialize();

            var b = haveSettlement ? MathUtil.ArrayMinus(pf, MathUtil.Muly(Kfs, us)) : pf;

            if (Solver.Solve(b, uf, out message) !=
                SolverResult.Success)
                throw new SolverFailException(message); //uf = kff^-1(Pf-Kfs*us)

            var residual = CheckingUtil.GetResidual(Solver.A, uf, b);

            this.Kfs.TransposeMultiply(uf, ps); //ps += Kfs*Uf

            if (haveSettlement)
                this.Kss.Multiply(us, ps); //ps += Kss*Us

            #endregion

            TraceUtil.WritePerformanceTrace(
                "solver: {0}, duration: {1} ms, size: {2}x{3}, residual {4:g} ", Solver.SolverType,
                sp.ElapsedMilliseconds, Solver.A.RowCount, Solver.A.ColumnCount, residual);

            sp.Restart();

            #region Adding result to Displacements and Forces members

            var ut = new double[6*n];
            var ft = new double[6*n];

            var revFMap = this.ReversedFixedMap;
            var revRMap = this.ReversedReleasedMap;


            for (int i = 0; i < freeCount; i++)
            {
                ut[revRMap[i]] = uf[i];
                ft[revRMap[i]] = pf[i];
            }


            for (int i = 0; i < fixCount; i++)
            {
                ut[revFMap[i]] = us[i];
                ft[revFMap[i]] = ps[i];
            }

            TraceUtil.WritePerformanceTrace("Assembling Ut, Pt from Uf,Ff,Us,Fs tooks {0} ms", sp.ElapsedMilliseconds);
            sp.Restart();

            displacements[cse] = ut;
            forces[cse] = ft;

            #endregion

        }

        #endregion

        #region Serialization stuff

        /*
        #region fields for using in serialization - deserialization

        private List<double[]> DisplacementsValues;
        private LoadCase[] DisplacementsCases;

        private LoadCase[] ForcesCases;
        private List<double[]> ForcesValues;

        #endregion


        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ReleasedMap",ReleasedMap);
            info.AddValue("FixedMap",FixedMap);
            info.AddValue("ReversedReleasedMap",ReversedReleasedMap);
            info.AddValue("ReversedFixedMap",ReversedFixedMap);
            info.AddValue("settlementsLoadCase",settlementsLoadCase);

            FillArraysFromDictionary();

            info.AddValue("DisplacementsCases", DisplacementsCases);
            info.AddValue("DisplacementsValues", DisplacementsValues);
            info.AddValue("ForcesCases", ForcesCases);
            info.AddValue("ForcesValues", ForcesValues);

            //info.AddValue("KffCholesky", );
            //info.AddValue("KffLdl", KffLdl);
            info.AddValue("Kss", Kss);
            info.AddValue("Kfs", Kfs);
        }

        private void FillArraysFromDictionary()
        {
            DisplacementsCases = new LoadCase[displacements.Count];
            DisplacementsValues = new List<double[]>();

            var cnt = 0;

            foreach (var pair in displacements)
            {
                DisplacementsCases[cnt++] = pair.Key;
                DisplacementsValues.Add(pair.Value);
            }

            ForcesCases = new LoadCase[displacements.Count];
            ForcesValues = new List<double[]>();

            cnt = 0;

            foreach (var pair in forces)
            {
                ForcesCases[cnt++] = pair.Key;
                ForcesValues.Add(pair.Value);
            }
        }

        protected StaticLinearAnalysisResult(SerializationInfo info, StreamingContext context)
        {
            ReleasedMap = info.GetValue<int[]>("ReleasedMap");
            FixedMap = info.GetValue<int[]>("FixedMap");
            ReversedReleasedMap = info.GetValue<int[]>("ReversedReleasedMap");
            ReversedFixedMap = info.GetValue<int[]>("ReversedFixedMap");
            settlementsLoadCase = info.GetValue<LoadCase>("settlementsLoadCase");

            DisplacementsCases = info.GetValue<LoadCase[]>("DisplacementsCases");
            DisplacementsValues = info.GetValue<List<double[]>>("DisplacementsValues");
            ForcesCases = info.GetValue<LoadCase[]>("ForcesCases");
            ForcesValues = info.GetValue<List<double[]>>("ForcesValues");

            KffCholesky = info.GetValue<CSparse.Double.Factorization.SparseCholesky>("KffCholesky");
            //KffLdl = info.GetValue<CSparse.Double.Factorization.SparseLDL>("KffLdl");

            Kss = info.GetValue<CSparse.Double.CompressedColumnStorage>("Kss");
            Kfs = info.GetValue<CSparse.Double.CompressedColumnStorage>("Kfs");
        }

        [OnDeserialized]
        private void FillDictionaryFromArray(StreamingContext context)
        {
            displacements.Clear();
            forces.Clear();

            for (var i = 0; i < DisplacementsValues.Count; i++)
            {
                displacements[DisplacementsCases[i]] = DisplacementsValues[i];
            }

            for (var i = 0; i < ForcesValues.Count; i++)
            {
                forces[ForcesCases[i]] = ForcesValues[i];
            }
        }
        */
        #endregion
    }
}
