using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class StiffnessCenterFinder
    {
        /// <summary>
        /// Gets the stiffness center of specified <see cref="element"/>.
        /// Stiffness center of rigid element.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="element"></param>
        /// <returns>stiffness centers</returns>
        public Point[] GetCenters(Model model, RigidElement_MPC element)
        {
            var cse_x = new LoadCase("tmp_case_x1", LoadType.Other);

            var stl = new VirtualConstraint();

            stl.AppliedLoadCases.Add(cse_x);


            stl.Nodes.AddRange(element.Nodes);

            stl.Constraint = Constraints.MovementFixed;
            stl.Settlement = new Displacement(new Vector(1, 1, 1), Vector.Zero);

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
    }
}
