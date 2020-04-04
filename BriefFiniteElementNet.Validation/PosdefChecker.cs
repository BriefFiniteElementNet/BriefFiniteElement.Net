using BriefFiniteElementNet.Mathh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public static class PosdefChecker
    {
        public static void CheckModel(Model model,LoadCase lc)
        {
            model.ReIndexNodes();
            //LoadCase.DefaultLoadCase

            var fullst = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);

            var mgr = DofMappingManager.Create(model, lc);

            var dvd = CalcUtil.GetReducedZoneDividedMatrix(fullst, mgr);

            var stiffness = dvd.ReleasedReleasedPart;


            var n = stiffness.ColumnCount;

            for (int i = 0; i < n; i++)
            {
                var t = stiffness.At(i, i);

                if (t > 0)
                    continue;

                var m1 = mgr.RMap2[i];
                var m2 = mgr.RMap1[m1];

                var nodeNum = m2/6;

                if (t == 0)
                    model.Trace.Write(Common.TraceLevel.Warning, "DoF {0} of Node {1} not properly constrained", m2%6, nodeNum);
                else//t < 0
                    model.Trace.Write(Common.TraceLevel.Warning, "DoF {0} of Node {1} not member", m2 % 6, nodeNum);
            }

            var k = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);
            //CalcUtil.MakeMatrixSymetric(k);


            var kt = k.Transpose();
            Enumerable.Range(0, k.Values.Length).ToList().ForEach(i => kt.Values[i] = -kt.Values[i]);

            var sym = k.Add(kt);

            

            var max = sym.Values.Max(i => Math.Abs(i));
        }

        public static void CheckModel_mpc(Model model, LoadCase lc)
        {
            var n = model.Nodes.Count * 6;

            model.ReIndexNodes();
            model.ReIndexElements();
            //LoadCase.DefaultLoadCase

            var perm = CalcUtil.GenerateP_Delta_Mpc(model, lc, new CsparsenetQrDisplacementPermutationCalculator());

            var np = perm.Item1.ColumnCount;//master count

            var rd = perm.Item2;

            var pd = perm.Item1;

            var kt = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);


            if (perm.Item1.RowCount > 0 && perm.Item1.RowCount > 0)
            {
                var pf = pd.Transpose();


                var kr = pf.Multiply(kt).Multiply(pd);

                var nr = kr.RowCount;

                for (int i = 0; i < nr; i++)
                {
                    //model.Solve_MPC
                    //two conditions: 
                    // 1 - DoF[i] in reduced structure is a member of bounded dof group with MPC equations
                    // 2 - DoF[i] in reduced structure is not in first condition, it is standalone and not related to any other DoF or Fixed Value
                    
                    var t = kr.At(i, i);

                    if (t > 0)
                        continue;

                    var nums = pd.EnumerateColumnMembers(i);


                    foreach(var num in nums)
                    {
                        var dofT = num.Item1;
                        
                        var nodeNum = dofT / 6;
                        var dof = (DoF)(dofT % 6);


                        if (double.IsNaN(t))
                        {
                            var connectedElements = model.Elements.Where(ii => ii.Nodes.Any(jj => jj.Index == nodeNum));

                            var susps = "";

                            foreach (var elm in connectedElements)
                            {
                                var stf = elm.GetGlobalStifnessMatrix();

                                if (stf.CoreArray.Any(ii => double.IsNaN(ii)))
                                    susps += elm.Index + ",";
                            }

                            model.Trace.Write(Common.TraceLevel.Warning, "DoF {0} of Node with index {1} have Nan in stifness matrix, suspecious connected elements with Nan in stiffness matrix are {2}", dof, nodeNum, susps);
                        }
                            

                        if (t == 0)
                            model.Trace.Write(Common.TraceLevel.Warning, "DoF {0} of Node with index {1} not properly constrained", dof, nodeNum);
                        else//t < 0
                            model.Trace.Write(Common.TraceLevel.Warning, "DoF {0} of Node #{1} not member", dof, nodeNum);
                    }

                }

            }
        }


        public static void FixUnrestrainedDofs(Model model, LoadCase lc)
        {
            model.ReIndexNodes();


            var fullst = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);

            var mgr = DofMappingManager.Create(model, lc);

            var dvd = CalcUtil.GetReducedZoneDividedMatrix(fullst, mgr);

            var stiffness = dvd.ReleasedReleasedPart;


            var n = stiffness.ColumnCount;

            for (int i = 0; i < n; i++)
            {
                var t = stiffness.At(i, i);

                if (t > 0)
                    continue;

                var m1 = mgr.RMap2[i];
                var m2 = mgr.RMap1[m1];

                var nodeNum = m2 / 6;

                var targetDof = (DoF)(m2 % 6);

                var nde = model.Nodes[nodeNum];

                switch (targetDof)
                {
                    case DoF.Dx:
                        nde.Constraints = nde.Constraints & Constraint.FixedDX;
                        break;
                    case DoF.Dy:
                        nde.Constraints = nde.Constraints & Constraint.FixedDY;
                        break;
                    case DoF.Dz:
                        nde.Constraints = nde.Constraints & Constraint.FixedDZ;
                        break;

                    case DoF.Rx:
                        nde.Constraints = nde.Constraints & Constraint.FixedRX;
                        break;
                    case DoF.Ry:
                        nde.Constraints = nde.Constraints & Constraint.FixedRY;
                        break;
                    case DoF.Rz:
                        nde.Constraints = nde.Constraints & Constraint.FixedRZ;
                        break;
                }
               
            }
        }
    }
}
