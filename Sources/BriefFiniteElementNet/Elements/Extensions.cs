using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Some extension methods
    /// </summary>
    public static class Extensions
    {
        #region peace add methods
        /// <summary>
        /// get the global displacement at xi
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="loadCase"></param>
        /// <returns></returns>
        public static Displacement GetGlobalDisplacementAt(this BarElement element, double xi, LoadCase loadCase)
        {
            var localDisp = element.GetInternalDisplacementAt(xi, loadCase);
            var trMgr = element.GetTransformationManager();
            return trMgr.TransformLocalToGlobal(localDisp);
        }
        /// <summary>
        /// get the global displacement at xi for DefaultLoadCase
        /// </summary>
        /// <param name="xi"></param>
        /// <returns></returns>
        public static Displacement GetGlobalDisplacementAt(this BarElement element, double xi)
        {
            return element.GetGlobalDisplacementAt(xi, LoadCase.DefaultLoadCase);
        }

        #endregion


        #region  issue number 145 on github


        /// <summary>
        /// If target plane cuts the element into two parts, 
        /// returns the internal force of element if an infinite plane with defined normal and point cuts the element
        /// the force that is applied to positive side (same side and normal)
        /// If the equilibrium equations of all the union nodes between all the elements are considered jointly, a set of equations is obtained that represents the equilibrium of the entire structure. These equations are obtained by assembling the equilibrium equations of the different finite elements that form it, in the form: Ue Keξe = Ue Pe
        /// Ue: Indicate the assembly of the different magnitudes according to the degrees of freedom of the structure.
        /// Ke : Stiffness matrix of the elements
        /// ξe : Nodal Strain Matrix
        /// Pe: Equivalent Nodal Forces from different sources
        /// All this leads to the equation: K∆ = F : Global equilibrium equation
        /// Regardless of the source of the loads, this must be converted into nodal forces.
        /// </summary>
        /// <param name="elm">target element</param>
        /// <param name="normal"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception ></exception>
        public static Force GetCrossingForce(this Element elm, Vector normal, Point p, LoadCombination cmb, Point targePoint)
        {
            var n = elm.Nodes.Length;
            Force[] forces;//external force applied to each node, cause the displacement occured
            var nodes = new Point[n];

            //calculate forces
            {
                var K = elm.GetGlobalStifnessMatrix();

                var d = new Matrix(6 * n, 1);

                for (var i = 0; i < n; i++)
                    nodes[i] = elm.Nodes[i].Location;

                for (var i = 0; i < n; i++)
                {
                    var di = elm.Nodes[i].GetNodalDisplacement(cmb);
                    var vdi = Displacement.ToVector(di);

                    for (var j = 0; j < 6; j++)
                        d[6 * i + j, 0] = vdi[j];
                }

                var f = K * d;

                forces = new Force[n];

                for (var i = 0; i < n; i++)
                {
                    forces[i] = Force.FromVector(f.Values, 6 * i);
                }
            }

            var sides = new int[n];//each node in which side?

            //calculates each node is in which side of plane
            {
                for (var i = 0; i < n; i++)
                {
                    var pi = nodes[i];

                    var res = normal.X * (pi.X - p.X) + normal.Y * (pi.Y - p.Y) + normal.Z * (pi.Z - p.Z);

                    sides[i] = Math.Sign(res);
                }
            }


            var buf = Force.Zero;

            //move nodal force in positive side into target point
            {
                for (var i = 0; i < n; i++)
                {
                    if (sides[i] > 0)
                        buf += forces[i].Move(elm.Nodes[i].Location, targePoint);
                }
            }

            return buf;
        }

        /// <summary>
        /// If target plane cuts the element into two parts, 
        /// returns the internal force of element if an infinite plane with defined normal and point cuts the element
        /// the force that is applied to positive side (same side and normal)
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="normal"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Force GetCrossingForce(this Element elm, int[] nods, LoadCombination cmb, Point targePoint)
        {
            var n = elm.Nodes.Length;
            Force[] forces;//external force applied to each node, cause the displacement occured
            var nodes = new Point[n];

            //calculate forces
            {
                var K = elm.GetGlobalStifnessMatrix();

                var d = new Matrix(6 * n, 1);

                for (var i = 0; i < n; i++)
                    nodes[i] = elm.Nodes[i].Location;

                for (var i = 0; i < n; i++)
                {
                    var di = elm.Nodes[i].GetNodalDisplacement(cmb);
                    var vdi = Displacement.ToVector(di);

                    for (var j = 0; j < 6; j++)
                        d[6 * i + j, 0] = vdi[j];
                }

                var f = K * d;

                forces = new Force[n];

                for (var i = 0; i < n; i++)
                {
                    forces[i] = Force.FromVector(f.Values, 6 * i);
                }
            }

            var sides = new int[n];//each node in which side?

            //calculates each node is in which side of plane
            {
                for (var i = 0; i < n; i++)
                {
                    if (nods.Contains(i))
                        sides[i] = +1;
                    else
                        sides[i] = -1;
                }
            }


            var buf = Force.Zero;

            //move nodal force in positive side into target point
            {
                for (var i = 0; i < n; i++)
                {
                    if (sides[i] > 0)
                        buf += forces[i].Move(elm.Nodes[i].Location, targePoint);
                }
            }

            return buf;
        }

        #endregion

    }
}
