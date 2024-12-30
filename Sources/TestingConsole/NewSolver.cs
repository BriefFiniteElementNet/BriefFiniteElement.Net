using BriefFiniteElementNet;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Common.Math;
using BriefFiniteElementNet.Solver;
using BriefFiniteElementNet.Utils;
using CSparse;
using CSparse.Double;
using CSparse.Double.Factorization;
using CSparse.Storage;
using MathNet.Numerics.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsole
{
    public class NewSolver
    {
        public bool DoTrace = true;


        private int N;

        #region matrices

        SparseMatrix Kp, K;
        SparseMatrix L, J, P;

        double[] F, D;//total force vector, Total displacement vector
        #endregion



        #region Permutations

        private int M_p_fx, M_p_rl, M_s_fx, M_s_rl;
        private int M_p, M_s;

        private int Rj;

        //private int[] Q, Qt, R, Rt;

        private HollowPermutationMatrix _Q, _Qt, _R, _Rt;

        //private int[] Psf_t, Psr_t, Ppf_t, Ppr_t;//_t: transposed

        private HollowPermutationMatrix _Psf_t, _Psr_t, _Ppf_t, _Ppr_t;//_t: transposed

        //private int[] Psf, Psr, Ppf, Ppr;

        private HollowPermutationMatrix _Psf, _Psr, _Ppf, _Ppr;

        //private int[] Ps_t, Pp_t;//_t: transposed

        private HollowPermutationMatrix _Ps_t, _Pp_t;//_t: transposed
        
        private HollowPermutationMatrix _Ps, _Pp;



        #endregion

        Model Model;

        LoadCase TargetLoadCase;

        private void InitMatrices()
        {
            var model = Model;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            {//assemble full stiffness matrix, full force and displacement vector
                K = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);

                TraceTime(">> Assmeble K matrix", sp);

                MatrixAssemblerUtil.AssembleFullForceVector(model, TargetLoadCase, F);

                TraceTime(">> Assmeble Force Vector", sp);

                MatrixAssemblerUtil.AssembleFullDisplacementVector(model, TargetLoadCase, D);

                TraceTime(">> Assmeble Disp Vector", sp);
            }

            sp.Stop();
        }

        public void Init(Model model)
        {
            N = model.Nodes.Count;

            //Q = new int[6 * N];
            //Qt = new int[6 * N];
            //R = new int[6 * N];
            //Rt = new int[6 * N];
            F = new double[6 * N];
            D = new double[6 * N];

            Model = model;

            var sp = System.Diagnostics.Stopwatch.StartNew();
            var sp2 = System.Diagnostics.Stopwatch.StartNew();

            InitMatrices();

            TraceTime("> Assmebling all matrices", sp);

            var j0 = GetJ(model);

            TraceTime("> Forming J ", sp);

            sp.Restart();


            var j = J = MakeJFullRank(j0);

            TraceTime("> Making J fullrank", sp);

            CalcPermutations();

            TraceTime("> Calculating Permutations", sp);

            CalcL();

            TraceTime("> Calculating L", sp);
            CalcP();
            TraceTime("> Calculating P", sp);

            sp.Stop();
            sp2.Stop();
            TraceTime("Total Init()", sp2);

            //Solve();
        }


        static SparseMatrix MakeJFullRank(SparseMatrix j)
        {
            //output of method should be a full rank sparse matrix
            //if unable to do so, then throw exception

            return j;

            throw new NotImplementedException();
        }

        static SparseMatrix ReduceJ(SparseMatrix j)
        {
            //Do reduce J as much as possible
            //at least Rj columns should have only one nonzero (kind of RREF)

            return j;

            throw new NotImplementedException();
        }

        static SparseMatrix GetJ(Model target)
        {
            var loadCase = LoadCase.DefaultLoadCase;

            var n = target.Nodes.Count * 6;
            var totDofCount = n;


            #region step 1
            var extraEqCount = 0;

            foreach (var mpcElm in target.MpcElements)
                if (mpcElm.AppliesForLoadCase(loadCase))
                    extraEqCount += mpcElm.GetExtraEquationsCount();

            var totalEqCount = extraEqCount;


            var allEqsCrd = new CoordinateStorage<double>(totalEqCount, n, 1);//rows: extra eqs, cols: 6*n+1 (+1 is for right hand side)

            var lastRow = 0;

            foreach (var mpcElm in target.MpcElements)
            {
                if (mpcElm.AppliesForLoadCase(loadCase))
                    if (mpcElm.GetExtraEquationsCount() != 0)
                    {
                        var extras = mpcElm.GetExtraEquations();

                        if (extras.ColumnCount != totDofCount + 1)
                            throw new Exception();

                        foreach (var tuple in extras.EnumerateIndexed())
                        {
                            var row = tuple.Item1;
                            var col = tuple.Item2;
                            var val = tuple.Item3;

                            allEqsCrd.At(row + lastRow, col, val);
                        }

                        lastRow += extras.RowCount;
                    }
            }

            var allEqs = (SparseMatrix)SparseMatrix.OfIndexed(allEqsCrd, true);


            return allEqs;

            #endregion
        }

        static SparseMatrix SpecialInvert(SparseMatrix matrix)
        {
            var buf = (SparseMatrix)matrix.Transpose();

            var rs = new int[matrix.RowCount];
            var cs = new int[matrix.ColumnCount];


            foreach (var item in matrix.EnumerateIndexed())
            {
                rs[item.Item1]++;
                cs[item.Item2]++;
            }


            if (rs.Any(i => i != 1))
                throw new Exception();

            if (cs.Any(i => i != 1))
                throw new Exception();

            for (int i = 0; i < matrix.Values.Length; i++)
            {
                buf.Values[i] = 1 / buf.Values[i];
            }

            return buf;
        }


        private void TraceTime(string title, System.Diagnostics.Stopwatch sp,bool resetSp=true)
        {
            if (!DoTrace)
                return;

            var trc = new TraceRecord();
            trc.Level = TraceLevel.Info;

            var mss = sp.Elapsed.TotalMilliseconds;

            if (mss > 100)
                mss -= mss % 1;

            else if (mss > 10)
                mss -= mss % 0.1;

            else if (mss > 1)
                mss -= mss % 0.01;

            trc.Message = string.Format("{0} took {1} ms", title, mss);

            Model.Trace.Write(trc);
            sp.Restart();
        }

        public void Solve()
        {
            var sp = System.Diagnostics.Stopwatch.StartNew();
            var sp2 = System.Diagnostics.Stopwatch.StartNew();


            var _Pt = (SparseMatrix)P.Transpose();

            var A = (SparseMatrix)_Pt.Multiply(P);

            Kp = HollowPermutationMatrix.PtaQ(_Q, _Q, K);

            var B = (SparseMatrix)_Pt.Multiply(Kp).Multiply(P);

            //var tmp = ToFriendlyString(A);

            var d_p_fx = new double[M_p_fx];
            var d_p_rl = new double[M_p_rl];

            var f_p_fx = new double[M_p_fx];
            var f_p_rl = new double[M_p_rl];

            {
                HollowPermutationMatrix.Pta(_Ppf, D, d_p_fx);
                HollowPermutationMatrix.Pta(_Ppr, F, f_p_rl);
            }
            
            HollowPermutationMatrix fx, rl, fxp, rlp;

            {
                int[] _fx, _rl;
                _fx = new int[M_p];
                _rl = new int[M_p];

                for (var i = 0; i < M_p; i++)
                {
                    _fx[i] = _rl[i] = -1;
                }

                for (var i = 0; i < M_p; i++)
                {
                    if (i < M_p_fx)
                        _fx[i] = i;
                    else
                        _rl[i] = i - M_p_fx;
                }

                fx = new HollowPermutationMatrix(_fx, M_p_fx);
                rl = new HollowPermutationMatrix(_rl, M_p_rl);

                fxp = fx.Transpose();
                rlp = rl.Transpose();
            }

            var a11 = HollowPermutationMatrix.PtaQ(fx, fx, A);
            var a12 = HollowPermutationMatrix.PtaQ(fx, rl, A);
            var a21 = HollowPermutationMatrix.PtaQ(rl, fx, A);
            var a22 = HollowPermutationMatrix.PtaQ(rl, rl, A);

            var b11 = HollowPermutationMatrix.PtaQ(fx, fx, B);
            var b12 = HollowPermutationMatrix.PtaQ(fx, rl, B);
            var b21 = HollowPermutationMatrix.PtaQ(rl, fx, B);
            var b22 = HollowPermutationMatrix.PtaQ(rl, rl, B);


            {
                if (a12.NonZerosCount != 0)
                {
                    var absMax12 = a12.Values.Max(Math.Abs);
                    var absMax = A.Values.Max(Math.Abs);


                    if (absMax12 > 1e-5 * absMax)
                        throw new Exception("Algorithm failure in solve procedure");//there is nonzero in A12 and A21
                }
            }



            {

                sp.Restart();

                var chol = SparseCholesky.Create(b22, ColumnOrdering.MinimumDegreeAtPlusA);

                TraceTime("Chol(b22) factorization", sp);

                var tmp22 = new double[M_p_rl];

                a22.Multiply(f_p_rl, tmp22);

                tmp22.InplaceNegate();
                b21.Multiply(d_p_fx, tmp22);
                tmp22.InplaceNegate();
                
                chol.Solve(tmp22, d_p_rl);
                TraceTime("Chol(b22) solve", sp);
            }

            {
                
                var qr = SparseQR.Create(a11, ColumnOrdering.MinimumDegreeAtPlusA);
                TraceTime("Qr(a11) factorization", sp);
                
                var tmp22 = new double[M_p_fx];

                b11.Multiply(d_p_fx, tmp22);

                b12.Multiply(d_p_rl, tmp22);

                qr.Solve(tmp22, f_p_fx);
                TraceTime("Qr(a11) solve", sp);
            }

            {
                var d_p = new double[M_p];
                var f_p = new double[M_p];

                var d_s = new double[M_s];
                var f_s = new double[M_s];

                d_p_fx.CopyTo(d_p, 0);
                d_p_rl.CopyTo(d_p, M_p_fx);

                f_p_fx.CopyTo(f_p, 0);
                f_p_rl.CopyTo(f_p, M_p_fx);

                L.Multiply(d_p, d_s);
                L.Multiply(f_p, f_s);

                var d_s_fx = new double[M_s_fx];
                var d_s_rl = new double[M_s_rl];

                var f_s_fx = new double[M_s_fx];
                var f_s_rl = new double[M_s_rl];

                Array.Copy(d_s, 0, d_s_fx, 0, M_s_fx);
                Array.Copy(d_s, M_s_fx, d_s_rl, 0, M_s_rl);

                Array.Copy(f_s, 0, f_s_fx, 0, M_s_fx);
                Array.Copy(f_s, M_s_fx, f_s_rl, 0, M_s_rl);

                {
                    HollowPermutationMatrix.Pa(_Ppr, d_p_rl, D);
                    HollowPermutationMatrix.Pa(_Ppf, d_p_fx, D);
                    HollowPermutationMatrix.Pa(_Psr, d_s_rl, D);
                    HollowPermutationMatrix.Pa(_Psf, d_s_fx, D);
                }

                {
                    HollowPermutationMatrix.Pa(_Ppr, f_p_rl, F);
                    HollowPermutationMatrix.Pa(_Ppf, f_p_fx, F);
                    HollowPermutationMatrix.Pa(_Psr, f_s_rl, F);
                    HollowPermutationMatrix.Pa(_Psf, f_s_fx, F);
                }
            }

            {
                TraceTime("Total Solve()", sp2);
            }


            if (false)
            {//compare

                Model.Solve_MPC();


                var d = Model.LastResult.Displacements[LoadCase.DefaultLoadCase];
                var f = Model.LastResult.Forces[LoadCase.DefaultLoadCase];

                var diffD = (double[])d.Clone();
                var diffF = (double[])f.Clone();

                diffD.InplaceNegate();
                diffF.InplaceNegate();

                diffD.AddToSelf(d);
                diffF.AddToSelf(f);

                var maxD = diffD.Max(Math.Abs);
                var maxF = diffF.Max(Math.Abs);

            }
        }


        private void CalcP()
        {
            var noSecondary = M_s == 0;//

            if (noSecondary)
            {
                P = (SparseMatrix)SparseMatrix.CreateIdentity(M_p);
            }
            else
            {
                var i1 = (SparseMatrix)SparseMatrix.CreateIdentity(M_p);

                P = VertConcat(L, i1);
            }
            
        }

        private void CalcL()
        {
            var noSecondary = M_s == 0;//

            var jps = HollowPermutationMatrix.AP(_Ps, J);
            var jpp = HollowPermutationMatrix.AP(_Pp, J);

            if (noSecondary)
            {
                L = new SparseMatrix(0, 0, 0);
            }
            else
            {
                var l = SpecialInvert(jps);

                l.InplaceNegate();

                l = (SparseMatrix)l.Multiply(jpp);

                L = l;
            }

        }


        private void CalcPermutations_old()
        {
            /*
            var n = N;
            var model = Model;

            var m_p_fx = 0;
            var m_p_rl = 0;
            var m_s_fx = 0;
            var m_s_rl = 0;

            {//calculating Q, L
                var fixity = DetermineFixedPree(model);
                var primSec = DeterminePrimarySecondary(model, J, out Rj);

                var ranks = new int[6 * n];
                //var ranks_r = new int[6 * n];

                {
                    /** /
                    for (var i = 0; i < 6 * n; i++)
                    {
                        var rank = i;
                        //var rank_r = i;

                        var f1 = fixity[i];
                        var f2 = primSec[i];

                        if (f1 == DofConstraint.Fixed && f2 == CalcType.Calculated)//fixed, sec
                        {
                            rank += 0 * (6 * n);
                            //rank_r += 2 * (6 * n);
                            m_s_fx++;
                        }

                        if (f1 == DofConstraint.Released && f2 == CalcType.Calculated)//free, sec
                        {
                            rank += 1 * (6 * n);
                            //rank_r += 3 * (6 * n);
                            m_s_rl++;
                        }

                        if (f1 == DofConstraint.Fixed && f2 == CalcType.Primary)//fixed, prim
                        {
                            rank += 2 * (6 * n);
                            //rank_r += 0 * (6 * n);
                            m_p_fx++;
                        }

                        if (f1 == DofConstraint.Released && f2 == CalcType.Primary)//free, prim
                        {
                            rank += 3 * (6 * n);
                            //rank_r += 1 * (6 * n);
                            m_p_rl++;
                        }

                        ranks[i] = rank;
                        //ranks_r[i] = rank_r;
                    }

                    Array.Sort(ranks);
                    //Array.Sort(ranks_r);

                    /** /

                    {
                        Psf_t = new int[m_s_fx];
                        Psr_t = new int[m_s_rl];
                        Ppf_t = new int[m_p_fx];
                        Ppr_t = new int[m_p_rl];

                        Ps_t = new int[m_s_fx + m_s_rl];
                        Pp_t = new int[m_p_fx + m_p_rl];
                    }



                    var sfc = 0;
                    var pfc = 0;
                    var src = 0;
                    var prc = 0;

                    for (var i = 0; i < ranks.Length; i++)
                    {
                        var rnk = ranks[i];

                        var level = rnk / (6 * n);

                        var dofId = rnk - level * 6 * n;

                        switch (level)
                        {
                            case 0:
                                Psf_t[sfc++] = dofId;
                                break;
                            case 1:
                                Psr_t[src++] = dofId;
                                break;
                            case 2:
                                Ppf_t[pfc++] = dofId;
                                break;
                            case 3:
                                Ppr_t[prc++] = dofId;
                                break;
                            default:
                                break;
                        }
                    }

                    {
                        Psf_t.CopyTo(Qt, 0);
                        Psr_t.CopyTo(Qt, Psf_t.Length);
                        Ppf_t.CopyTo(Qt, Psf_t.Length + Psr_t.Length);
                        Ppr_t.CopyTo(Qt, Psf_t.Length + Psr_t.Length + Ppf_t.Length);
                    }

                    {
                        Psf_t.CopyTo(Ps_t, 0);
                        Psr_t.CopyTo(Ps_t, Psf_t.Length);
                    }

                    {
                        Ppf_t.CopyTo(Pp_t, 0);
                        Ppr_t.CopyTo(Pp_t, Ppf_t.Length);
                    }

                    if (!Permutation.IsValid(Qt))
                        throw new Exception("algorithm failure");

                    {
                        Q = BfePermutation.InvertPermutation(Qt, 6 * N);
                        R = BfePermutation.InvertPermutation(Rt, 6 * N);
                    }

                    {
                        Ppf = BfePermutation.InvertPermutation(Ppf_t, 6 * N);
                        Ppr = BfePermutation.InvertPermutation(Ppr_t, 6 * N);
                        Psf = BfePermutation.InvertPermutation(Psf_t, 6 * N);
                        Psr = BfePermutation.InvertPermutation(Psr_t, 6 * N);
                    }

                    {
                        M_s_fx = m_s_fx;
                        M_s_rl = m_s_rl;
                        M_p_fx = m_p_fx;
                        M_p_rl = m_p_rl;

                        M_p = m_p_fx + m_p_rl;
                        M_s = m_s_fx + m_s_rl;
                    }

                    //we must consider Q and P as matrices not permutations
                    //so that applying them could be translated to matrix mult and
                    //could be done with Permutation.Apply
                    //csparsenet Permutation.Apply does the right ways, where gets the P and applies the P^T

                }
            }

            */
        }


        private void CalcPermutations()
        {
            var n = N;
            var model = Model;

            var m_p_fx = 0;
            var m_p_rl = 0;
            var m_s_fx = 0;
            var m_s_rl = 0;

            {//calculating Q, L
                var fixity = DetermineFixedPree(model);
                var primSec = DeterminePrimarySecondary(model, J, out Rj);

                var ranks = new int[6 * n];
                //var ranks_r = new int[6 * n];
                int[] Psf_t, Psr_t, Ppf_t, Ppr_t;
                int[] Ps_t, Pp_t;

                int[] Qt;

                {
                    /**/
                    for (var i = 0; i < 6 * n; i++)
                    {
                        var rank = i;
                        //var rank_r = i;

                        var f1 = fixity[i];
                        var f2 = primSec[i];

                        if (f1 == DofConstraint.Fixed && f2 == CalcType.Calculated)//fixed, sec
                        {
                            rank += 0 * (6 * n);
                            //rank_r += 2 * (6 * n);
                            m_s_fx++;
                        }

                        if (f1 == DofConstraint.Released && f2 == CalcType.Calculated)//free, sec
                        {
                            rank += 1 * (6 * n);
                            //rank_r += 3 * (6 * n);
                            m_s_rl++;
                        }

                        if (f1 == DofConstraint.Fixed && f2 == CalcType.Primary)//fixed, prim
                        {
                            rank += 2 * (6 * n);
                            //rank_r += 0 * (6 * n);
                            m_p_fx++;
                        }

                        if (f1 == DofConstraint.Released && f2 == CalcType.Primary)//free, prim
                        {
                            rank += 3 * (6 * n);
                            //rank_r += 1 * (6 * n);
                            m_p_rl++;
                        }

                        ranks[i] = rank;
                        //ranks_r[i] = rank_r;
                    }

                    Array.Sort(ranks);
                    //Array.Sort(ranks_r);

                    /**/

                    {
                        Psf_t = new int[m_s_fx];
                        Psr_t = new int[m_s_rl];
                        Ppf_t = new int[m_p_fx];
                        Ppr_t = new int[m_p_rl];

                        Ps_t = new int[m_s_fx + m_s_rl];
                        Pp_t = new int[m_p_fx + m_p_rl];
                    }

                    {
                        Qt = new int[6 * n];
                    }

                    var sfc = 0;
                    var pfc = 0;
                    var src = 0;
                    var prc = 0;

                    for (var i = 0; i < ranks.Length; i++)
                    {
                        var rnk = ranks[i];

                        var level = rnk / (6 * n);

                        var dofId = rnk - level * 6 * n;

                        switch (level)
                        {
                            case 0:
                                Psf_t[sfc++] = dofId;
                                break;
                            case 1:
                                Psr_t[src++] = dofId;
                                break;
                            case 2:
                                Ppf_t[pfc++] = dofId;
                                break;
                            case 3:
                                Ppr_t[prc++] = dofId;
                                break;
                            default:
                                break;
                        }
                    }


                    {
                        Psf_t.CopyTo(Qt, 0);
                        Psr_t.CopyTo(Qt, Psf_t.Length);
                        Ppf_t.CopyTo(Qt, Psf_t.Length + Psr_t.Length);
                        Ppr_t.CopyTo(Qt, Psf_t.Length + Psr_t.Length + Ppf_t.Length);
                    }

                    {
                        Psf_t.CopyTo(Ps_t, 0);
                        Psr_t.CopyTo(Ps_t, Psf_t.Length);
                    }

                    {
                        Ppf_t.CopyTo(Pp_t, 0);
                        Ppr_t.CopyTo(Pp_t, Ppf_t.Length);
                    }

                    if (!Permutation.IsValid(Qt))
                        throw new Exception("algorithm failure");

                    {
                        //Q = BfePermutation.InvertPermutation(Qt, 6 * N);
                        //R = BfePermutation.InvertPermutation(Rt, 6 * N);
                    }

                    {
                        _Qt = new HollowPermutationMatrix(Qt, 6 * N);
                        _Q = _Qt.Transpose();
                    }

                    {
                        //Ppf = BfePermutation.InvertPermutation(Ppf_t, 6 * N);
                        //Ppr = BfePermutation.InvertPermutation(Ppr_t, 6 * N);
                        //Psf = BfePermutation.InvertPermutation(Psf_t, 6 * N);
                        //Psr = BfePermutation.InvertPermutation(Psr_t, 6 * N);
                    }

                    {
                        _Psf_t = new HollowPermutationMatrix(Psf_t, 6 * N);
                        _Psf = _Psf_t.Transpose();

                        _Psr_t = new HollowPermutationMatrix(Psr_t, 6 * N);
                        _Psr = _Psr_t.Transpose();

                        _Ppf_t = new HollowPermutationMatrix(Ppf_t, 6 * N);
                        _Ppf = _Ppf_t.Transpose();

                        _Ppr_t = new HollowPermutationMatrix(Ppr_t, 6 * N);
                        _Ppr = _Ppr_t.Transpose();
                    }

                    {
                        _Ps_t = new HollowPermutationMatrix(Ps_t, 6 * N);
                        _Ps = _Ps_t.Transpose();
                    }

                    {
                        _Pp_t = new HollowPermutationMatrix(Pp_t, 6 * N);
                        _Pp = _Pp_t.Transpose();
                    }

                    {
                        M_s_fx = m_s_fx;
                        M_s_rl = m_s_rl;
                        M_p_fx = m_p_fx;
                        M_p_rl = m_p_rl;

                        M_p = m_p_fx + m_p_rl;
                        M_s = m_s_fx + m_s_rl;
                    }

                    //we must consider Q and P as matrices not permutations
                    //so that applying them could be translated to matrix mult and
                    //could be done with Permutation.Apply
                    //csparsenet Permutation.Apply does the right ways, where gets the P and applies the P^T

                }
            }
        }


        private static DofConstraint[] DetermineFixedPree(Model model)
        {
            var n = model.Nodes.Count;

            var buf = new DofConstraint[6 * n];

            {
                for (var i = 0; i < n; i++)
                {
                    var fixity = model.Nodes[i].Constraints;//.GetComponent((DoF)dof);

                    for (int j = 0; j < 6; j++)
                    {
                        buf[6 * i + j] = fixity.GetComponent((DoF)j);
                    }
                }
            }

            return buf;
        }


        private enum CalcType
        {
            Primary,
            Calculated
        }

        private static SparseMatrix VertConcat(SparseMatrix a, SparseMatrix b)
        {
            if (a.ColumnCount != b.ColumnCount)
                throw new Exception();

            var ar = a.RowCount;
            var ac = a.ColumnCount;

            var br = b.RowCount;

            var cc = new CoordinateStorage<double>(a.RowCount + b.RowCount, a.ColumnCount, a.NonZerosCount + b.NonZerosCount);

            foreach (var item in a.EnumerateIndexed())
            {
                cc.At(item.Item1, item.Item2, item.Item3);
            }

            foreach (var item in b.EnumerateIndexed())
            {
                cc.At(item.Item1 + ar, item.Item2, item.Item3);
            }

            return cc.ToCCs();
        }


        private static string ToFriendlyString(CSparse.Storage.CompressedColumnStorage<double> mtx)
        {
            var dn = BriefFiniteElementNet.Mathh.Extensions.ToDense(mtx);

            return dn.ToString();
        }

        private static CalcType[] DeterminePrimarySecondary(Model model, SparseMatrix j, out int rj)
        {
            //perm:
            //first n-rj members which are only nnz in their row and col are secondary
            //then rj primary

            //J should be full rank (no row duplicates)

            rj = j.RowCount;


            var nn = j.ColumnCount;
            var buf = new CalcType[nn];


            if (rj == 0)
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = CalcType.Primary;
                    return buf;
                }
            }

            var rowNnzCount = new int[rj];


            var colNnzCount = new int[nn];
            var colNnzMember = new int[nn];//row index of last nnz of column[i]

            {
                foreach (var item in j.EnumerateIndexed())
                {
                    rowNnzCount[item.Item1]++;
                    colNnzCount[item.Item2]++;
                    colNnzMember[item.Item2] = item.Item1;
                }
            }

            var rowFlags = new bool[rj];

           

            {
                Array.Clear(buf, 0, buf.Length);
            }
            //var c1 = 0;
            //var c2 = 0;

            var primCounter = 0;

            for (var col = 0; col < nn; col++)
            {
                var flag = false;

                if (colNnzCount[col] != 1)
                {
                    flag = true;
                }

                var nnzRow = colNnzMember[col];

                if (rowFlags[nnzRow])
                {
                    flag = true;
                }

                if (!flag)
                {
                    buf[col] = CalcType.Calculated;
                    rowFlags[nnzRow] = true;
                    primCounter++;
                }
                else
                {
                    buf[col] = CalcType.Primary;
                }

                if (primCounter == rj)
                    break;
            }


            if (primCounter != rj)
                throw new Exception();


            return buf;

        }

    }
}
