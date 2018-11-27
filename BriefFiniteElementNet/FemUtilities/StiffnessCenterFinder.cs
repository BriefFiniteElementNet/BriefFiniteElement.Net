using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;
using BriefFiniteElementNet.Solver;
using CSparse.Double;
using CSR = CSparse.Double.SparseMatrix;

namespace BriefFiniteElementNet.FemUtilies
{
    [Obsolete("Under development")]
    public class StiffnessCenterFinder
    {
        /// <summary>
        /// Gets the stiffness center of specified <see cref="element"/>.
        /// Stiffness center of rigid element.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="element"></param>
        /// <returns>stiffness centers</returns>
        public Point[] GetCenters(Model model, RigidElement_MPC element,LoadCase loadCase)
        {
            //model = model.Clone();

            var perm = CalcUtil.GenerateP_Delta_Mpc(model, loadCase, new Mathh.GaussRrefFinder());

            var adj = GetAdjacencyGraph(perm.Item1);

            var dofGroups = CalcUtil.EnumerateGraphPartsAsGroups(adj);


            //var parts=CalcUtil.

            var cse_x = new LoadCase("tmp_case_x1", LoadType.Other);

            var stl = new VirtualConstraint();

            stl.AppliedLoadCases.Add(cse_x);

            stl.Nodes.AddRange(element.Nodes);

            stl.Constraint = Constraints.Fixed;
            stl.Settlement = Displacement.Zero;

           

            model.MpcElements.Add(stl);

            var cnf = new SolverConfiguration(cse_x);

            cnf.SolverFactory = new CholeskySolverFactory();

            model.Solve_MPC(cnf);

            var frc = Force.Zero;

            foreach (var node in stl.Nodes)
            {
                var force = node.GetTotalExternalForces(cse_x);
                frc += force.Move(node.Location, Point.Origins);
            }

            //move frc to a place without moment, only with force

            {
                var fx = frc.Fx;
                var fy = frc.Fy;
                var fz = frc.Fz;

                var mx = frc.Mx;
                var my = frc.My;
                var mz = frc.Mz;



                var fv = new Vector(fx, fy, fz);
                var mv = new Vector(-mx, -my, -mz);

                var f = new Matrix(4, 3);

                f.FillRow(0, 0, fz, -fy);
                f.FillRow(1, -fz, 0, fx);
                f.FillRow(2, fy, -fx, 0);
                f.FillRow(3, fx, fy, fz);

                var p0 = new Matrix(3, 4);
                p0[0, 1] = p0[1, 2] = p0[2, 3] = 1;

                var p1 = new Matrix(3, 4);
                p1[0, 0] = p1[1, 2] = p1[2, 3] = 1;

                var p2 = new Matrix(3, 4);
                p2[0, 0] = p2[1, 1] = p2[2, 3] = 1;

                var d0 = p0 * f;
                var d1 = p1 * f;
                var d2 = p2 * f;

                var det0 = d0.Det3x3();
                var det1 = d1.Det3x3();
                var det2 = d2.Det3x3();

                var sols = new List<Vector>();

                if (det0 != 0)
                {
                    var sol0 = (d0.Inv3x3() * mv.ToMatrix()).ToVector();
                    sols.Add(sol0);
                }

                if (det1 != 0)
                {
                    var sol1 = (d1.Inv3x3() * mv.ToMatrix()).ToVector();
                    sols.Add(sol1);
                }

                if (det2 != 0)
                {
                    var sol2 = (d2.Inv3x3() * mv.ToMatrix()).ToVector();
                    sols.Add(sol2);
                }
            }


            throw new NotImplementedException();
        }

        public Point[] GetCenters2(Model model, List<Tuple<Node,DoF>> dofs,LoadCase cse)
        {
            /*
            var rnd = Guid.NewGuid().ToString("N").Substring(0, 5) + "_";

            model = model.Clone();


            foreach(var dof in dofs)
            {
                //var modelLo
            }

            var centralNode = dofs.FirstOrDefault().Item1;// element.Nodes.FirstOrDefault();

            foreach (var nde in model.Nodes)
            {
                nde.Settlements = Displacement.Zero;
                nde.Loads.Clear();

                if (element.Nodes.IndexOfReference(nde) == -1)
                    nde.Constraints = Constraints.Fixed;
                else
                    nde.Constraints = Constraints.Released;
            }


            foreach(var mpcElm in model.MpcElements)
            {
                if (mpcElm is RigidElement_MPC)
                    if (mpcElm.Nodes.Contains(central))
                        throw new Exception("invalid model");
            }


            var tls = new string[] { rnd + "dx", rnd + "dy", rnd + "dz", rnd + "rx", rnd + "ry", rnd + "rz" };

            var cnf = new SolverConfiguration();
            
            for (var i = 0; i < 6; i++)
            {
                var dVec = new double[6];

                dVec[i] = 1;

                var frc = Force.FromVector(dVec, 0);

                var cse = new LoadCase(tls[i], LoadType.Other);

                var ld = new NodalLoad(frc, cse);

                central.Loads.Add(ld);

                cnf.LoadCases.Add(cse);
            }

            cnf.SolverFactory = new CholeskySolverFactory();


            model.Solve_MPC(cnf);

            var flx = new Matrix(6, 6);

            for (var i = 0; i < 6; i++)
            {

            }


            var cse_x = new LoadCase("tmp_case_x1", LoadType.Other);

            var perm = CalcUtil.GenerateP_Delta_Mpc(model, cse_x, new Mathh.GaussRrefFinder());

            var np = perm.Item1.ColumnCount;//master count

            var rd = perm.Item2;

            var pd = perm.Item1;

            var kt = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);

            var pf = pd.Transpose();

            var kr = pf.Multiply(kt).Multiply(pd);

            var kr_d = kr.ToDenseMatrix();


            
            */

            throw new NotImplementedException();
        }

        public CSR GetAdjacencyGraph(CSR P_delta)
        {
            var p = P_delta.CloneMatrix();
            
            for (var i = 0; i < p.NonZerosCount; i++)
                p.Values[i] = 1;
            
            var buf = (CSR)p.Multiply(p.Transpose());

            return buf;
        }
    }
}
