using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Common;
using CSparse.Double;
using BriefFiniteElementNet.Solver;
using CCS = CSparse.Double.CompressedColumnStorage;

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

        public StaticLinearAnalysisResult(Model parent)
        {
            this.parent = parent;
        }

        #endregion

        #region Fields

        private Model parent;

        private Dictionary<LoadCase, double[]> _displacements = new Dictionary<LoadCase, double[]>();
        private Dictionary<LoadCase, double[]> _forces = new Dictionary<LoadCase, double[]>();
        private Dictionary<LoadCase, double[]> elementForces = new Dictionary<LoadCase, double[]>();
        private Dictionary<LoadCase, double[]> concentratedForces = new Dictionary<LoadCase, double[]>();
        private Dictionary<LoadCase, double[]> supportReactions = new Dictionary<LoadCase, double[]>();

        private LoadCase settlementsLoadCase { get; set; }

        [Obsolete]
        public int[] ReleasedMap { get; set; } //ReleasedMap[GlobalDofIndex] = DoF index in free DoFs
        
        [Obsolete]
        public int[] FixedMap; //FixedMap[GlobalDofIndex] = DoF index in fixed DoFs
        
        [Obsolete]
        public int[] ReversedReleasedMap; //ReversedReleasedMap[DoF index in free DoFs] = GlobalDofIndex
        
        [Obsolete]
        public int[] ReversedFixedMap; //ReversedFixedMap[DoF index in fixed DoFs] = GlobalDofIndex


        #region Stiffness matrices

        [Obsolete]
        internal CompressedColumnStorage Kff { get; set; }
        [Obsolete]
        internal CompressedColumnStorage Kfs { get; set; }
        [Obsolete]
        internal CompressedColumnStorage Kss { get; set; }

        #endregion

        /// <summary>
        /// The solvers with key of master mapping! Solvers[Master Mapping] = appropriated solver
        /// </summary>
        public Dictionary<int[], ISolver> Solvers = new Dictionary<int[], ISolver>(new IntArrayCompairer());

        /// <summary>
        /// The solvers with key of Pd matrix (delta permute) Solvers[Pd] = appropriated solver
        /// </summary>
        public Dictionary<CCS, ISolver> Solvers_New = new Dictionary<CCS, ISolver>();


        [Obsolete]
        public ISolver Solver { get; set; }

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
            get { return _displacements; }
            internal set { _displacements = value; }
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
            get { return _forces; }
            internal set { _forces = value; }
        }

        /// <summary>
        /// Gets the element forces.
        /// </summary>
        /// <value>
        /// The sum of element equivalent nodal loads on each node.
        /// </value>
        public Dictionary<LoadCase, double[]> ElementForces
        {
            get { return elementForces; }
            internal set { elementForces = value; }
        }

        /// <summary>
        /// Gets the concentrated forces.
        /// </summary>
        /// <value>
        /// The sum of concentrated nodal loads on each node.
        /// </value>
        public Dictionary<LoadCase, double[]> ConcentratedForces
        {
            get { return concentratedForces; }
            internal set { concentratedForces = value; }
        }

        /// <summary>
        /// Gets or sets the support reactions.
        /// </summary>
        /// <value>
        /// The support reactions due to loads on free DoFs.
        /// </value>
        public Dictionary<LoadCase, double[]> SupportReactions
        {
            get { return supportReactions; }
            set { supportReactions = value; }
        }


        internal Model Parent
        {
            get { return parent; }
            set { parent = value; }
        }



        /// <summary>
        /// Gets or sets the solver generator.
        /// </summary>
        /// <value>
        /// The solver generator which generates an <see cref="ISolver"/> for every <see cref="CompressedColumnStorage"/>.
        /// </value>
        [Obsolete("use SolverFactory instead")]
        public Func<CompressedColumnStorage, ISolver> SolverGenerator { get; set; }

        public ISolverFactory SolverFactory { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the analysis result if not exists.
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <remarks>If current instance do not contains the results related to <see cref="cse"/>, then this method will add result related to <see cref="cse"/> using <see cref="StaticLinearAnalysisResult.AddAnalResult"/> method</remarks>
        public void AddAnalysisResultIfNotExists(LoadCase cse)
        {
            var f1 = _displacements.ContainsKey(cse);
            var f2 = _forces.ContainsKey(cse);

            if (f1 != f2)
                throw new Exception("!");

            if (f1)
                return;

            AddAnalysisResult(cse);
            //AddAnalysisResult_v2(cse);
        }

        public void AddAnalysisResultIfNotExists_MPC(LoadCase cse)
        {
            var f1 = _displacements.ContainsKey(cse);
            var f2 = _forces.ContainsKey(cse);

            if (f1 != f2)
                throw new Exception("!");

            if (f1)
                return;

            //AddAnalysisResult(cse);
            AddAnalysisResult_MPC(cse);
        }

        /*
        /// <summary>
        /// Adds the analysis result.
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <remarks>if model is analyzed against specific load case, then displacements are available through <see cref="Displacements"/> property.
        /// If system is not analyses against a specific load case, then this method will analyses structure against <see cref="LoadCase"/>.
        /// While this method is using pre computed Cholesky Decomposition , its have a high performance in solving the system.
        /// </remarks>
        [Obsolete("Use AddAnalysisResultIfNotExists instead")]
        public void AddAnalysisResult(LoadCase cse)
        {
            throw new NotImplementedException();

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

                    for (var i = 0; i < nc; i++)
                    {
                        elm.Nodes[i].MembersLoads.Add(new NodalLoad(frc[i], cse));
                    }
                }
            }

            #endregion

            //TraceUtil.WritePerformanceTrace("Calculating end member forces took {0} ms", sp.ElapsedMilliseconds);

            parent.Trace.Write(TraceRecord.Create(TraceLevel.Info,
                    string.Format("Calculating end member forces took {0} ms",
                        sp.ElapsedMilliseconds)));

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

                if (cse == this.parent.SettlementLoadCase) disp = nodes[i].Settlements;


                #region DX

                if (cns.DX == DofConstraint.Released)
                {
                    pf[rmap[6*i + 0]] = force.Fx;
                    uf[rmap[6*i + 0]] = disp.DX;
                }
                else
                {
                    ps[fmap[6*i + 0]] = force.Fx;
                    us[fmap[6*i + 0]] = disp.DX;
                }

                #endregion

                #region DY

                if (cns.DY == DofConstraint.Released)
                {
                    pf[rmap[6*i + 1]] = force.Fy;
                    uf[rmap[6*i + 1]] = disp.DY;
                }
                else
                {
                    ps[fmap[6*i + 1]] = force.Fy;
                    us[fmap[6*i + 1]] = disp.DY;
                }

                #endregion

                #region DZ

                if (cns.DZ == DofConstraint.Released)
                {
                    pf[rmap[6*i + 2]] = force.Fz;
                    uf[rmap[6*i + 2]] = disp.DZ;
                }
                else
                {
                    ps[fmap[6*i + 2]] = force.Fz;
                    us[fmap[6*i + 2]] = disp.DZ;
                }

                #endregion



                #region RX

                if (cns.RX == DofConstraint.Released)
                {
                    pf[rmap[6*i + 3]] = force.Mx;
                    uf[rmap[6*i + 3]] = disp.RX;
                }
                else
                {
                    ps[fmap[6*i + 3]] = force.Mx;
                    us[fmap[6*i + 3]] = disp.RX;
                }

                #endregion

                #region RY

                if (cns.RY == DofConstraint.Released)
                {
                    pf[rmap[6*i + 4]] = force.My;
                    uf[rmap[6*i + 4]] = disp.RY;
                }
                else
                {
                    ps[fmap[6*i + 4]] = force.My;
                    us[fmap[6*i + 4]] = disp.RY;
                }

                #endregion

                #region RZ

                if (cns.RZ == DofConstraint.Released)
                {
                    pf[rmap[6*i + 5]] = force.Mz;
                    uf[rmap[6*i + 5]] = disp.RZ;
                }
                else
                {
                    ps[fmap[6*i + 5]] = force.Mz;
                    us[fmap[6*i + 5]] = disp.RZ;
                }

                #endregion
            }

            #endregion

            //TraceUtil.WritePerformanceTrace("forming Uf,Us,Ff,Fs took {0} ms", sp.ElapsedMilliseconds);

            parent.Trace.Write(TraceRecord.Create(TraceLevel.Info,
                   string.Format("forming Uf,Us,Ff,Fs took {0} ms",
                       sp.ElapsedMilliseconds)));

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


            ISolver solver;


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

            //TraceUtil.WritePerformanceTrace(
            //    "solver: {0}, duration: {1} ms, size: {2}x{3}, residual {4:g} ", Solver.SolverType,
            //    sp.ElapsedMilliseconds, Solver.A.RowCount, Solver.A.ColumnCount, residual);

            parent.Trace.Write(TraceRecord.Create(TraceLevel.Info,
                  string.Format("solver: {0}, duration: {1} ms, size: {2}x{3}, residual {4:g} ", Solver.SolverType,
                sp.ElapsedMilliseconds, Solver.A.RowCount, Solver.A.ColumnCount, residual)));

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

            //TraceUtil.WritePerformanceTrace("Assembling Ut, Pt from Uf,Ff,Us,Fs tooks {0} ms", sp.ElapsedMilliseconds);

            parent.Trace.Write(TraceRecord.Create(TraceLevel.Info,
                string.Format("Assembling Ut, Pt from Uf,Ff,Us,Fs tooks {0} ms", sp.ElapsedMilliseconds)));

            sp.Restart();

            displacements[cse] = ut;
            forces[cse] = ft;

            #endregion

        }
        */

        /// <summary>
        /// Adds the analysis result.
        /// </summary>
        /// <param name="loadCase">The load case.</param>
        /// <remarks>if model is analyzed against specific load case, then displacements are available through <see cref="Displacements"/> property.
        /// If system is not analyses against a specific load case, then this method will analyses structure against <see cref="LoadCase"/>.
        /// While this method is using pre computed Cholesky Decomposition , its have a high performance in solving the system.
        /// </remarks>
        [Obsolete("Use AddAnalysisResult instead")]
        private void AddAnalysisResult2(LoadCase loadCase)
        {
            ISolver solver;

            var map = DofMappingManager.Create(parent, loadCase);

            var n = parent.Nodes.Count;//node count
            var m = map.M;//master node count

            var dispPermute = PermutationGenerator.GetDisplacementPermute(parent, map);
            var forcePermute = PermutationGenerator.GetForcePermute(parent, map);

            var ft = GetTotalForceVector(loadCase, map);
            var ut = GetTotalDispVector(loadCase, map);
            var kt = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(parent);

            for (var i = 0; i < map.Fixity.Length; i++)
            {
                if (map.Fixity[i] == DofConstraint.Fixed)
                    ft[i] = 0;
                else
                    ut[i] = 0;
            }

            var kr = (CCS)((CCS)forcePermute.Multiply(kt)).Multiply(dispPermute);
            var fr = forcePermute.Multiply(ft);
            var ur = new double[fr.Length];

            for (var i = 0; i < 6*m; i++)
            {
                ur[i] = ut[map.RMap1[i]];
            }

            var krd = CalcUtil.GetReducedZoneDividedMatrix(kr, map);

            AnalyseStiffnessMatrixForWarnings(krd, map,loadCase);

            var ff_r = GetFreePartOfReducedVector(fr, map);
            var us_r = GetFixedPartOfReducedVector(ur, map);

            if (Solvers.ContainsKey(map.MasterMap))
            {
                solver = Solvers[map.MasterMap];
            }
            else
            {
                solver =
                    //SolverGenerator(krd.ReleasedReleasedPart);
                    SolverFactory.CreateSolver(krd.ReleasedReleasedPart);

                Solvers[map.MasterMap] = solver;
            }

            if (!solver.IsInitialized)
                solver.Initialize();

            #region ff - kfs * us
            //درسته، تغییرش نده گوس...

            var ff_r_negative = ff_r.Clone();

            for (var i = 0; i < ff_r.Length; i++)
                ff_r[i] = -ff_r[i];

            krd.ReleasedFixedPart.Multiply(us_r, ff_r); 

            for (var i = 0; i < ff_r.Length; i++)
                ff_r[i] = -ff_r[i];

            #endregion

            var urf = new double[map.RMap2.Length];

            string msg;

            //var res = 
            solver.Solve(ff_r, urf);

            //if (res != SolverResult.Success)
            //   throw new BriefFiniteElementNetException(msg);

            var frs = CalcUtil.Add(krd.FixedReleasedPart.Multiply(urf), krd.FixedFixedPart.Multiply(us_r));

            for (var i = 0; i < urf.Length; i++)
            {
                ur[map.RMap2[i]] = urf[i];
            }

            for (var i = 0; i < frs.Length; i++)
            {
                fr[map.RMap3[i]] = frs[i];
            }

            for (var i = 0; i < map.RMap3.Length; i++)
            {
                var ind = i;
                var gi = map.RMap1[map.RMap3[ind]];
                ft[gi] = frs[ind];
            }

            var ut2 = dispPermute.Multiply(ur);

            for (int i = 0; i < 6*n; i++)
            {
                if (map.Fixity[i] == DofConstraint.Fixed)
                    ut2[i] = ut[i];
            }

            _forces[loadCase] = ft;
            _displacements[loadCase] = ut2;
        }

        /// <summary>
        /// Adds the analysis result.
        /// </summary>
        /// <param name="loadCase">The load case.</param>
        /// <remarks>if model is analyzed against specific load case, then displacements are available through <see cref="Displacements"/> property.
        /// If system is not analyses against a specific load case, then this method will analyses structure against <see cref="LoadCase"/>.
        /// While this method is using pre computed Cholesky Decomposition , its have a high performance in solving the system.
        /// </remarks>
        public void AddAnalysisResult(LoadCase loadCase)
        {
            ISolver solver;

            var map = DofMappingManager.Create(parent, loadCase);

            var n = parent.Nodes.Count;//node count
            var m = map.M;//master node count

            var pu = PermutationGenerator.GetDisplacementPermute(parent, map);//permutation of U
            var pf = PermutationGenerator.GetForcePermute(parent, map);//permutation of F

            var fe = elementForces[loadCase] = GetTotalElementsForceVector(loadCase);
            var fc = concentratedForces[loadCase] = GetTotalConcentratedForceVector(loadCase);
            

            var ft = fe.Plus(fc);


            var fr = pf.Multiply(ft);
            var ffr = GetFreePartOfReducedVector(fr, map);
            var fsr = GetFixedPartOfReducedVector(fr, map);

            var kt = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(parent);
            var kr = (CCS)((CCS)pf.Multiply(kt)).Multiply(pu);

            #region  U_s,r
            var usr = new double[map.RMap3.Length];

            {
                //should fill usr
                var ut_temp = GetTotalDispVector(loadCase, map);

                for (int i = 0; i < usr.Length; i++)
                {
                    var t1 = map.RMap3[i];
                    var t2 = map.RMap1[t1];

                    usr[i] = ut_temp[t2];
                }

            }

            #endregion

            var krd = CalcUtil.GetReducedZoneDividedMatrix(kr, map);
            AnalyseStiffnessMatrixForWarnings(krd, map, loadCase);

            {//TODO: remove

                var minAbsDiag = double.MaxValue;

                foreach(var tpl in krd.ReleasedReleasedPart.EnumerateIndexed2())
                {
                    if (tpl.Item1 == tpl.Item2)
                        minAbsDiag = Math.Min(minAbsDiag, Math.Abs(tpl.Item3));
                }

                if (krd.ReleasedReleasedPart.RowCount != 0)
                {
                    //var kk = krd.ReleasedReleasedPart.ToDenseMatrix();
                }
            }

            #region  solver

            if (Solvers.ContainsKey(map.MasterMap))
            {
                solver = Solvers[map.MasterMap];
            }
            else
            {
                solver =
                    //SolverGenerator(krd.ReleasedReleasedPart);
                    SolverFactory.CreateSolver(krd.ReleasedReleasedPart);

                Solvers[map.MasterMap] = solver;
            }

            
            if (!solver.IsInitialized)
                solver.Initialize();

            #endregion

            double[] ufr = new double[map.RMap2.Length];
            string message;

            var input = ffr.Minus(krd.ReleasedFixedPart.Multiply(usr));


            solver.Solve(input, ufr);

            //if (res2 != SolverResult.Success)
            //    throw new BriefFiniteElementNetException(message);

            var fpsr = krd.FixedReleasedPart.Multiply(ufr).Plus(krd.FixedFixedPart.Multiply(usr));

            var fsrt = fpsr.Minus(fsr);// no needed

            var fx = supportReactions[loadCase] = new double[6*n];

            #region forming ft


            for (var i = 0; i < map.Fixity.Length; i++)
                if (map.Fixity[i] == DofConstraint.Fixed)
                    ft[i] = 0;
            
            for (var i = 0; i < fpsr.Length; i++)
            {
                var totDofNum = map.RMap1[map.RMap3[i]];

                ft[totDofNum] = fx[totDofNum] = fpsr[i];
            }
            
            
            #endregion

            #region forming ur

            var ur = new double[map.M*6];


            for (var i = 0; i < usr.Length; i++)
            {
                ur[map.RMap3[i]] = usr[i];
            }

            for (var i = 0; i < ufr.Length; i++)
            {
                ur[map.RMap2[i]] = ufr[i];
            }

            #endregion

            var ut = pu.Multiply(ur);

            _forces[loadCase] = ft;
            _displacements[loadCase] = ut;
        }


        /// <summary>
        /// Adds the analysis result.
        /// </summary>
        /// <param name="loadCase">The load case.</param>
        /// <remarks>if model is analyzed against specific load case, then displacements are available through <see cref="Displacements"/> property.
        /// If system is not analyses against a specific load case, then this method will analyses structure against <see cref="LoadCase"/>.
        /// While this method is using pre computed Cholesky Decomposition , its have a high performance in solving the system.
        /// </remarks>
        public void AddAnalysisResult_MPC(LoadCase loadCase)
        {
            var n = parent.Nodes.Count * 6;
            var dt = new double[n];//total delta 

            ISolver solver;

            var perm = CalcUtil.GenerateP_Delta_Mpc(parent, loadCase, new Mathh.GaussRrefFinder());

            var np = perm.Item1.ColumnCount;//master count

            var rd = perm.Item2;

            var pd = perm.Item1;

            var kt = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(parent);


            var ft = new double[n];

            {
                var fe = elementForces[loadCase] = GetTotalElementsForceVector(loadCase);
                var fc = concentratedForces[loadCase] = GetTotalConcentratedForceVector(loadCase);

                ft.AddToSelf(fe);
                ft.AddToSelf(fc);
            }

            if (perm.Item1.RowCount > 0 && perm.Item1.RowCount > 0)
            {
                var pf = pd.Transpose();

                
                var kr = pf.Multiply(kt).Multiply(pd);

                var a1 = new double[n];

                //a1.AddToSelf(rd);

                kt.Multiply(rd, a1);

                a1.AddToSelf(ft);

                var a2 = new double[np];

                pf.Multiply(a1, a2);

                var a3 = new double[np];

                #region load/generate solver

                if (Solvers_New.ContainsKey(pd))
                {
                    solver = Solvers_New[pd];
                }
                else
                {
                    solver = SolverFactory.CreateSolver((CCS)kr);

                    Solvers_New[pd] = solver;
                }


                if (!solver.IsInitialized)
                    solver.Initialize();

                #endregion

                solver.Solve(a2, a3);

                pd.Multiply(a3, dt);
            }

            dt.AddToSelf(rd, -1);

            kt.Multiply(dt, ft);

            _forces[loadCase] = ft;
            _displacements[loadCase] = dt;
            
        }

        /// <summary>
        /// Analyses the stiffness matrix for warnings.
        /// </summary>
        /// <param name="mtx">The MTX.</param>
        /// <param name="map">The map.</param>
        /// <param name="currentCase">The current load case which error is with it.</param>
        /// <remarks>
        /// Only searches for zero elements on matrix diagonal
        /// </remarks>
        private void AnalyseStiffnessMatrixForWarnings(ZoneDevidedMatrix mtx, DofMappingManager map,
            LoadCase currentCase)
        {
            var cs = mtx.ReleasedReleasedPart;

            var n = cs.ColumnCount;

            var t = new bool[n]; //true if i'th diagonal member nonzero, false if diagonal member zero!

            for (var i = 0; i < n; i++)
            {
                var st = cs.ColumnPointers[i];
                var en = cs.ColumnPointers[i + 1];

                var col = i;

                for (var j = st; j < en; j++)
                {
                    var row = cs.RowIndices[j];
                    //var val = cs.Values[j];

                    if (row == col)
                        t[row] = true;
                }
            }

            for (var i = 0; i < n; i++)
            {
                if (t[i])
                    continue;

                var globalDofNum = map.RMap1[map.RMap2[i]];

                var nodeNum = globalDofNum/6;
                var dof = (DoF) (globalDofNum%6);

                var rec = TraceRecords.GetRecord(30000, parent.Nodes[nodeNum].Label);

                rec.TargetIdentifier = string.Format(
                    "{0} DoF on node #{1} for load case with [name = '{2}'] and [nature = {3}]", dof, nodeNum,
                    currentCase.CaseName, currentCase.LoadType);

                parent.Trace.Write(rec);
            }
        }

        private void CheckForSymmetricity(CompressedColumnStorage cs)
        {
            var cr = cs.Transpose();

            for (int i = 0; i < cs.NonZerosCount; i++)
            {
                if (cr.RowIndices[i] != cs.RowIndices[i])
                    throw new Exception();

                if (Math.Abs(cr.Values[i] - cs.Values[i]) > 1e-5)
                    throw new Exception();
            }
        }

        /// <summary>
        /// Gets the total force vector for whole structure for specified <see cref="cse"/>.
        /// Force on fixed DoF s are zero.
        /// </summary>
        /// <param name="cse">The cse.</param>
        /// <param name="map">The map.</param>
        /// <remarks>
        /// This is not used for finding unknown forces (like support reactions). 
        /// Just is used for known forces (like forces on free DoFs).
        /// </remarks>
        /// <returns></returns>
        [Obsolete("use GetTotalConcentratedForceVector and GetTotalElementsForceVector instead")]
        private double[] GetTotalForceVector(LoadCase cse, DofMappingManager map)
        {
            //force vector for both free and fixed dof

            var n = parent.Nodes.Count;

            var loads = new Force[ n];//loads from connected element to node is stored in this array instead of Node.ElementLoads.


            for (int i = 0; i < n; i++)//re indexing
            {
                parent.Nodes[i].Index = i;
            }

            #region adding element loads

            foreach (var elm in parent.Elements)
            {
                var nc = elm.Nodes.Length;

                foreach (var ld in elm.Loads)
                {
                    if (ld.Case != cse)
                        continue;

                    var frcs = ld.GetGlobalEquivalentNodalLoads(elm);

                    for (var i = 0; i < nc; i++)
                    {
                        var nde = elm.Nodes[i];
                        loads[nde.Index] += frcs[i];
                    }
                }
            }

            #endregion



            #region adding concentrated nodal loads

            for (int i = 0; i < n; i++)
            {
                foreach (var load in parent.Nodes[i].Loads)
                {
                    if (load.Case != cse)
                        continue;

                    loads[parent.Nodes[i].Index] += load.Force;
                }
            }

            #endregion


            var buf = new double[6 * n];
            for (int i = 0; i < n; i++)
            {
                var force = loads[i];

                buf[6 * i + 0] = force.Fx;
                buf[6 * i + 1] = force.Fy;
                buf[6 * i + 2] = force.Fz;

                buf[6 * i + 3] = force.Mx;
                buf[6 * i + 4] = force.My;
                buf[6 * i + 5] = force.Mz;
            }

            return buf;
        }

        /// <summary>
        /// Gets the total concentrated forces on loads vector.
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        private double[] GetTotalConcentratedForceVector(LoadCase cse)
        {
            //force vector for both free and fixed dof

            var n = parent.Nodes.Count;

            var loads = new Force[n];//loads from connected element to node is stored in this array instead of Node.ElementLoads.

            for (int i = 0; i < n; i++)//re indexing
            {
                parent.Nodes[i].Index = i;
            }

            #region adding concentrated nodal loads

            for (int i = 0; i < n; i++)
            {
                foreach (var load in parent.Nodes[i].Loads)
                {
                    if (load.Case != cse)
                        continue;

                    loads[parent.Nodes[i].Index] += load.Force;
                }
            }

            #endregion


            var buf = new double[6 * n];
            for (int i = 0; i < n; i++)
            {
                var force = loads[i];

                buf[6 * i + 0] = force.Fx;
                buf[6 * i + 1] = force.Fy;
                buf[6 * i + 2] = force.Fz;

                buf[6 * i + 3] = force.Mx;
                buf[6 * i + 4] = force.My;
                buf[6 * i + 5] = force.Mz;
            }

            return buf;
        }

        /// <summary>
        /// Gets the total elements equivalent nodal force vector.
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        private double[] GetTotalElementsForceVector(LoadCase cse)
        {
            //force vector for both free and fixed dof

            var n = parent.Nodes.Count;

            var loads = new Force[n];//loads from connected element to node is stored in this array instead of Node.ElementLoads.

            for (int i = 0; i < n; i++)//re indexing
            {
                parent.Nodes[i].Index = i;
            }

            #region adding element loads

            foreach (var elm in parent.Elements)
            {
                var nc = elm.Nodes.Length;

                foreach (var ld in elm.Loads)
                {
                    if (ld.Case != cse)
                        continue;

                    var frcs = ld.GetGlobalEquivalentNodalLoads(elm);

                    for (var i = 0; i < nc; i++)
                    {
                        var nde = elm.Nodes[i];
                        loads[nde.Index] += frcs[i];
                    }
                }
            }

            #endregion


            var buf = new double[6 * n];
            for (int i = 0; i < n; i++)
            {
                var force = loads[i];

                buf[6 * i + 0] = force.Fx;
                buf[6 * i + 1] = force.Fy;
                buf[6 * i + 2] = force.Fz;

                buf[6 * i + 3] = force.Mx;
                buf[6 * i + 4] = force.My;
                buf[6 * i + 5] = force.Mz;
            }

            return buf;
        }

        /// <summary>
        /// Gets the total displacement vector for whole structure for specified <see cref="cse"/>
        /// </summary>
        /// <param name="cse">The cse.</param>
        /// <param name="map">The map.</param>
        /// <remarks>
        /// This is not used for finding unknown displacements (like displacement of free DoFs). 
        /// Just is used for known displacements (like settlements and only for settlement).
        /// </remarks>
        /// <returns></returns>
        private double[] GetTotalDispVector(LoadCase cse, DofMappingManager map)
        {
            //displacement vector. free part can be assume zero however didn't touch that

            var n = parent.Nodes.Count;

            var buf = new double[6 * n];

            if (parent.SettlementLoadCase!= cse)
                return buf;

            var nodes = parent.Nodes;

            for (var i = 0; i < n; i++)
            {
                var disp = nodes[i].Settlements;

                buf[6 * i + 0] = disp.DX;
                buf[6 * i + 1] = disp.DY;
                buf[6 * i + 2] = disp.DZ;

                buf[6 * i + 3] = disp.RX;
                buf[6 * i + 4] = disp.RY;
                buf[6 * i + 5] = disp.RZ;
            }


            return buf;
        }


        private double[] GetFreePartOfReducedVector(double[] vr, DofMappingManager map)
        {
            var buf = new double[map.RMap2.Length];

            for (var i = 0; i < buf.Length; i++)
            {
                buf[i] = vr[map.RMap2[i]];
            }

            return buf;
        }

        private double[] GetFixedPartOfReducedVector(double[] vr, DofMappingManager map)
        {
            var buf = new double[map.RMap3.Length];

            for (var i = 0; i < buf.Length; i++)
            {
                buf[i] = vr[map.RMap3[i]];
            }

            return buf;
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
