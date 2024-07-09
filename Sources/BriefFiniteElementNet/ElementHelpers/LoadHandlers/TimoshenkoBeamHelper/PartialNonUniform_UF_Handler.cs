using BriefFiniteElementNet.ElementHelpers.Bar;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.TimoshenkoBeamHelper
{
    public class PartialNonUniform_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is Bar.TimoshenkoBeamHelper))
                return false;

            if (!(load is Loads.PartialNonUniformLoad))
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
            var n = bar.Nodes.Length;

            var bhlpr = hlpr as Bar.TimoshenkoBeamHelper;

            //https://www.quora.com/How-should-I-perform-element-forces-or-distributed-forces-to-node-forces-translation-in-the-beam-element

            var tr = elm.GetTransformationManager();

            #region uniform 

            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                int degree;//polynomial degree of magnitude function

                #region inits
                
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.SeverityFunction.Evaluate(xi);
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];// Coefficients.Length;
                }

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = bhlpr.GetNMaxOrder(elm).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                    {
                        var shp = hlpr.GetNMatrixAt(elm, xi, 0, 0);

                        /*
                        if (_direction == BeamDirection.Y)
                        {
                            for (var i = 0; i < shp.ColumnCount; i++)
                            {
                                if (i % 2 == 1)
                                    shp.MultiplyColumnByConstant(i, -1);
                            }
                        }*/

                        var q__ = magnitude(xi);
                        var j = hlpr.GetJMatrixAt(elm, xi, 0, 0);
                        shp.Scale(j.Determinant());

                        var q_ = localDir * q__;

                        if (bhlpr.Direction == BeamDirection.Y)
                            shp.Scale(q_.Z);
                        else
                            shp.Scale(q_.Y);

                        return shp;
                    }, xi0, xi1, gpt);

                    var res = intg.Integrate();

                    if (n > 2)
                        throw new Exception("beam with more than 2 node not supported!");

                    var localForces = new Force[n];



                    if (bhlpr.Direction == BeamDirection.Y)
                    {
                        var fz0 = res[0, 0];
                        var my0 = res[0, 1];
                        var fz1 = res[0, 2];
                        var my1 = res[0, 3];

                        localForces[0] = new Force(0, 0, fz0, 0, my0, 0);
                        localForces[1] = new Force(0, 0, fz1, 0, my1, 0);
                    }
                    else
                    {

                        var fy0 = res[0, 0];
                        var mz0 = res[0, 1];
                        var fy1 = res[0, 2];
                        var mz1 = res[0, 3];

                        localForces[0] = new Force(0, fy0, 0, 0, 0, mz0);
                        localForces[1] = new Force(0, fy1, 0, 0, 0, mz1);
                    }

                    return localForces;
                }
            }

            #endregion
        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }

        public object GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)

        {
            var n = elm.Nodes.Length;

            var buff = new List<Tuple<DoF, double>>();

            var tr = elm.GetTransformationManager();

            var br = elm as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(elm, hlpr, load);

            var bhlpr = hlpr as Bar.TimoshenkoBeamHelper;

            for (var i = 0; i < n; i++)
                endForces[i] = -endForces[i];//(2,1) section

            #region 2,1 (due to inverse of equivalent nodal loads)

            Force ends;//internal force in x=0 due to inverse of equivalent nodal loads will store in this variable, 

            {
                var xi_s = new double[br.Nodes.Length];//xi loc of each force
                var x_s = new double[br.Nodes.Length];//x loc of each force

                for (var i = 0; i < xi_s.Length; i++)
                {
                    var x_i = elm.Nodes[i].Location - elm.Nodes[0].Location;
                    var xi_i = br.LocalCoordsToIsoCoords(x_i.Length)[0];

                    xi_s[i] = xi_i;
                    x_s[i] = x_i.X;
                }

                ends = new Force();//sum of moved end forces to destination


                for (var i = 0; i < n; i++)
                {
                    if (xi_s[i] < loc.Xi)
                    {
                        var frc_i = endForces[i];// new Force();

                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }
                }
            }

            #endregion

            var to = bhlpr.Iso2Local(elm, loc.Xi)[0];


            #region uniform & trapezoid, uses integration method

            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                int degree;//polynomial degree of magnitude function

                #region inits
                
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.GetMagnitudeAt(elm, new IsoPoint(xi));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];
                }
                
                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = 0;// GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 3;//gauss point count

                    Matrix integral;

                    double i0 = 0, i1 = 0;//span for integration

                    var xi_t = loc.Xi;

                    #region span of integration
                    {

                        if (xi_t < xi0)
                        {
                            i0 = i1 = xi0;
                        }

                        if (xi_t > xi1)
                        {
                            i0 = xi0;
                            i1 = xi1;
                        }

                        if (xi_t < xi1 && xi_t > xi0)
                        {
                            i0 = xi0;
                            i1 = xi_t;
                        }
                    }
                    #endregion

                    #region integration
                    {
                        if (i1 == i0)
                        {
                            integral = elm.MatrixPool.Allocate(2, 1);
                        }
                        else
                        {
                            var x0 = br.IsoCoordsToLocalCoords(i0)[0];
                            var x1 = br.IsoCoordsToLocalCoords(i1)[0];

                            var intgV = GaussianIntegrator.CreateFor1DProblem(xx =>
                            {
                                //var xi = Local2Iso(targetElement, x)[0];
                                //var j = GetJMatrixAt(targetElement, xi);

                                var xi = br.LocalCoordsToIsoCoords(xx)[0];

                                var q__ = magnitude(xi);
                                var q_ = localDir * q__;

                                double df, dm;

                                if (bhlpr.Direction == BeamDirection.Y)
                                {
                                    df = q_.Z;
                                    dm = -q_.Z * xx;
                                }
                                else
                                {
                                    df = q_.Y;
                                    dm = q_.Y * xx;
                                }

                                var buf_ = elm.MatrixPool.Allocate(new double[] { df, dm });

                                return buf_;
                            }, x0, x1, gpt);

                            integral = intgV.Integrate();
                        }
                    }
                    #endregion


                    var v_i = integral[0, 0];//total shear
                    var m_i = integral[1, 0];//total moment of load about the start node

                    var x = bhlpr.Iso2Local(elm, loc.Xi)[0];

                    var f = new Force();//total moment about start node, total shear 


                    if (bhlpr.Direction == BeamDirection.Y)
                    {
                        f.Fz = v_i;
                        f.My = m_i;//negative is taken into account earlier
                    }
                    else
                    {
                        f.Fy = v_i;
                        f.Mz = m_i;
                    }



                    {
                        //this block is commented to fix the issue #48 on github
                        //when this block is commented out, then issue 48 is fixed
                        /*
                        if (br.StartReleaseCondition.DY == DofConstraint.Released)
                            f.Fy = 0;

                        if (br.StartReleaseCondition.DZ == DofConstraint.Released)
                            f.Fz = 0;

                        if (br.StartReleaseCondition.RY == DofConstraint.Released)
                            f.My = 0;

                        if (br.StartReleaseCondition.RZ == DofConstraint.Released)
                            f.Mz = 0;
                        */
                    }


                    var f2 = f + ends;

                    f2 = f2.Move(new Point(0, 0, 0), new Point(x, 0, 0));

                    f2 *= -1;

                    if (bhlpr.Direction == BeamDirection.Y)
                    {
                        buff.Add(Tuple.Create(DoF.Ry, f2.My));
                        buff.Add(Tuple.Create(DoF.Dz, f2.Fz));
                    }
                    else
                    {
                        buff.Add(Tuple.Create(DoF.Rz, f2.Mz));
                        buff.Add(Tuple.Create(DoF.Dy, f2.Fy));
                    }


                    return buff;
                }
            }



            #endregion

        }
    }
}
