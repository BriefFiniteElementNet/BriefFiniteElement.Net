using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.Truss2Node
{
    /// <summary>
    /// UniformLoad with uniform material and section on truss helper
    /// </summary>
    public class Uniform_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.TrussHelper2Node))
                return false;

            if (!(load is UniformLoad))
                return false;

            if (!(elm is BarElement))
                return false;

            var bar = elm as BarElement;

            var mat = bar.Material;
            var sec = bar.Section;

            if (mat.GetMaxFunctionOrder()[0] != 0)//constant uniform material through length
                return false;

            if (sec.GetMaxFunctionOrder()[0] != 0)//constant uniform section through length
                return false;

            return true;
        }

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            var bar = elm as BarElement;

            var mat = bar.Material;
            var sec = bar.Section;

            throw new NotImplementedException();
        }

        public StrainTensor GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad ld, IsoPoint loc)
        {
            var bar = elm as BarElement;
            var load = ld as UniformLoad;
            var xi = loc.Xi;

            //var mat = bar.Material;
            //var sec = bar.Section;


            //https://byjusexamprep.com/gate-ce/fixed-beams
            double w0;
            double L;
            double f0, m0;

            if (bar.NodeCount != 2)
                throw new Exception();

            {//step 0
                L = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;
            }

            #region step 1
            {
                var p0 = GetLocalEquivalentNodalLoads(bar, hlpr, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                w0 = localDir.X * load.Magnitude;
            }
            #endregion

            {
                var eiOrder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

                if (eiOrder != 0) throw new BriefFiniteElementNetException("Nonuniform EI");
            }




            {
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi, bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var e = mat.Ex;
                var A = sec.A;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];


                var d = w0 * x / (2 * e * A) * (L - x);

                var buf = new Displacement();

                buf.DX = d;

                //todo: convert buf to strain tensor
                //return buf;
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public CauchyStressTensor GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            var bar = elm as BarElement;

            var mat = bar.Material;
            var sec = bar.Section;

            var isoLocation = loc;

            var buff = new List<Tuple<DoF, double>>();

            //var buf = new FlatShellStressTensor();

            var tr = bar.GetTransformationManager();

            var br = bar as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(bar, hlpr, load);

            var n = bar.Nodes.Length;

            for (var i = 0; i < n; i++)
                endForces[i] = -endForces[i];

            #region 2,1 (due to inverse of equivalent nodal loads)

            Force ends;//internal force in x=0 due to inverse of equivalent nodal loads will store in this variable, 

            {
                var xi_s = new double[br.Nodes.Length];//xi loc of each force
                var x_s = new double[br.Nodes.Length];//x loc of each force

                for (var i = 0; i < xi_s.Length; i++)
                {
                    var x_i = bar.Nodes[i].Location - bar.Nodes[0].Location;
                    var xi_i = br.LocalCoordsToIsoCoords(x_i.Length)[0];

                    xi_s[i] = xi_i;
                    x_s[i] = x_i.X;
                }

                ends = new Force();//sum of moved end forces to destination

                for (var i = 0; i < n; i++)
                {
                    if (xi_s[i] <= isoLocation.Xi)
                    {
                        var frc_i = endForces[i];// new Force();
                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }

                }
            }


            #endregion

            var hp = hlpr as TrussHelper2Node;

            var to = hp.Iso2Local(bar, isoLocation.Xi)[0];

            //var xi = isoLocation[0];

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0;
                int degree;//polynomial degree of magnitude function

                #region inits

                if (load is UniformLoad)
                {
                    var uld = load as UniformLoad;

                    magnitude = xi => uld.Magnitude;
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = -1;
                    //xi1 = to;
                    degree = 0;
                }
                else if (load is PartialNonUniformLoad)
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.GetMagnitudeAt(bar, new IsoPoint(xi));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;

                    to = Math.Min(to, uld.EndLocation.Xi);

                    degree = uld.SeverityFunction.Degree[0];
                }
                else
                    throw new NotImplementedException();

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = 0;// GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    Matrix integral;


                    if (isoLocation.Xi < xi0)
                    {
                        integral = new Matrix(2, 1);
                    }
                    else
                    {
                        var intgV = GaussianIntegrator.CreateFor1DProblem(x =>
                        {
                            var xi = hp.Local2Iso(bar, x);
                            var q__ = magnitude(xi[0]);
                            var q_ = localDir * q__;

                            var df = q_.X;

                            var buf_ = Matrix.OfVector(new double[] { df });

                            return buf_;
                        }, 0, to, gpt);

                        integral = intgV.Integrate();
                    }

                    var X = hp.Iso2Local(bar, isoLocation.Xi)[0];

                    var f_i = integral[0, 0];

                    var f = new Force();

                    f.Fx = f_i;


                    //this block is commented to fix the issue #48 on github
                    //when this block is commented out, then issue 48 is fixed
                    {

                        //if (br.StartReleaseCondition.DX == DofConstraint.Released)
                        //    f_i = 0;
                    }

                    var f2 = f + ends;

                    f2 = f2.Move(new Point(0, 0, 0), new Point(X, 0, 0));

                    f2 *= -1;

                    /*
                    var movedEnds = ends.Move(new Point(), new Point());//no need to move as it is truss without moments
                    var fMoved = new Force(f_i, 00, 00, 0, 0, 0);

                    var ft = movedEnds + fMoved;
                    */

                    //ft *= -1;

                    buff.Add(Tuple.Create(DoF.Dx, f2.Fx));

                }

                //todo: convert buff to tensor
                //return buff;
                throw new NotImplementedException();
            }

            #endregion


            throw new NotImplementedException();
        }
    }
}
