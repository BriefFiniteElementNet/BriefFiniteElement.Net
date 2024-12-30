using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers.Bar;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElementLocalDof = BriefFiniteElementNet.ElementPermuteHelper.ElementLocalDof;

namespace BriefFiniteElementNet.ElementHelpers.BarHelpers
{
    public partial class EulerBernoulliBeamHelper2Node : BaseBar2NodeHelper
    {

        public static Matrix GetNMatrixAt(double xi, double l, DofConstraint D0, DofConstraint R0, DofConstraint D1, DofConstraint R1, BeamDirection dir)
        {
            SingleVariablePolynomial[] nss, mss;

            EulerBernouly2NodeShapeFunction.GetShapeFunctions(l, D0, R0, D1, R1, dir, out nss, out mss);

            var buf = new Matrix(4, 4);

            for (var i = 0; i < 2; i++)
            {
                if (nss[i] == null) nss[i] = new SingleVariablePolynomial();
                if (mss[i] == null) mss[i] = new SingleVariablePolynomial();
            }

            for (var i = 0; i < 2; i++)
            {
                var ni = nss[i];
                var mi = mss[i];

                for (var ii = 0; ii < 4; ii++)
                {
                    buf[ii, 2 * i + 0] = ni.EvaluateDerivative(xi, ii);
                    buf[ii, 2 * i + 1] = mi.EvaluateDerivative(xi, ii);
                }
            }


            return buf;
        }

        public static Matrix GetNMatrixAt(double xi, double l, BeamDirection dir)
        {
            return GetNMatrixAt(xi, l, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, dir);
        }


        public BeamDirection Direction;

        public EulerBernoulliBeamHelper2Node(BeamDirection direction, Element targetElement)
            : base(targetElement)
        {
            Direction = direction;
        }


        #region IElementHelper

        public override Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

            var buff = GetNMatrixAt(bar, xi);

            //buff is d²N/dξ²
            //but B is d²N/dx²
            //so B will be arr * dξ²/dx² = arr * 1/ j.det ^2
            //based on http://www-g.eng.cam.ac.uk/csml/teaching/4d9/4D9_handout2.pdf

            var j = GetJ(bar);

            var b = buff.ExtractRow(2);

            b.Scale(1 / (j * j));

            return b;
        }

        public override Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

            var c0 = bar.NodalReleaseConditions[0];
            var c1 = bar.NodalReleaseConditions[1];

            var order = GetDofOrder(targetElement);

            var d0 = c0.GetComponent(order[0].Dof);
            var r0 = c0.GetComponent(order[1].Dof);

            var d1 = c1.GetComponent(order[2].Dof);
            var r1 = c1.GetComponent(order[3].Dof);

            return GetNMatrixAt(xi, l, d0, r0, d1, r1, Direction);
        }


        [TodoDelete]
        /// <summary>
        /// Get the internal defomation of element due to applied <see cref="load"/>
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="load"></param>
        /// <param name="isoLocation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private /*override*/ Displacement GetLoadDisplacementAt_old(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            var bar = targetElement as BarElement;

            var n = targetElement.Nodes.Length;

            if (n != 2)
                throw new Exception("more than two nodes not supported");


            var eIorder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

            if (eIorder == 0 && bar.StartReleaseCondition == Constraints.Fixed && bar.EndReleaseCondition == Constraints.Fixed)//constant/uniform section along the length of beam
            {
                //EI is constant through the length of beam
                //end releases are fixed

                if (load is UniformLoad ul)
                    return GetLoadDisplacementAt_UniformLoad_uniformSection(bar, ul, isoLocation[0]);

                if (load is ConcentratedLoad cl)
                    return GetLoadDisplacementAt_ConcentratedLoad_uniformSection(bar, cl, isoLocation[0]);

                if (load is PartialNonUniformLoad pnl)
                    return GetLoadDisplacementAt_PartialNonUniformLoad_uniformSection(bar, pnl, isoLocation[0]);
            }

            throw new NotImplementedException();
        }

        #region GetLoadDisplacement

        private Displacement GetLoadDisplacementAt_UniformLoad_uniformSection(BarElement bar, UniformLoad load, double xi)
        {
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
                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if(load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        f0 = p0.Fz;
                        m0 = p0.My;//TODO: add possible negative sign
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BeamDirection.Z:
                        f0 = p0.Fy;
                        m0 = -p0.Mz;//TODO: add possible negative sign
                        w0 = localDir.Y * load.Magnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            #endregion

            {
                var eiOrder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

                if (eiOrder != 0) throw new BriefFiniteElementNetException("Nonuniform EI");
            }

            


            {
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi,bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var e = mat.Ex;
                var I = this.Direction == BeamDirection.Y ? sec.Iy : sec.Iz;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                //https://gs-post-images.grdp.co/2022/8/fixed-beam-deflection-img1661861640198-75-rs.PNG?noResize=1
                var d = w0 * x * x / (24.0 * e * I) * (L - x) * (L - x);

                var buf = new Displacement();

                if (this.Direction == BeamDirection.Y)
                    buf.DZ = d;
                else
                    buf.DY = d;

                return buf;
            }
        }

        //copied to ILoadHandler
        private Displacement GetLoadDisplacementAt_ConcentratedLoad_uniformSection(BarElement bar, ConcentratedLoad load, double xi)
        {
            //https://www.engineersedge.com/beam_bending/beam_bending19.htm
            double L;
            double ft, mt;//force and moment concentrated
            double f0, m0;//inverse of eq nodal loads
            double xt;//applied location

            if (bar.NodeCount != 2)
                throw new Exception();

            {//step 0
                L = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

                xt = bar.IsoCoordsToLocalCoords(xi)[0];
            }

            #region step 1
            {
                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                var localForce = load.Force;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);

                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        ft = localForce.Fz;
                        mt = localForce.My;//TODO: add possible negative sign
                        f0 = p0.Fz;
                        m0 = p0.My;//TODO: add possible negative sign
                        break;

                    case BeamDirection.Z:
                        ft = localForce.Fy;
                        mt = -localForce.Mz;//TODO: add possible negative sign

                        f0 = p0.Fy;
                        m0 = -p0.Mz;//TODO: add possible negative sign

                        break;

                    default:
                        throw new NotImplementedException();
                }


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
                var I = this.Direction == BeamDirection.Y ? sec.Iy : sec.Iz;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                var d = 0.0;//TODO

                {
                    var x2 = x * x;
                    var x3 = x2 * x;
                    var xt2 = xt * xt;
                    var xt3 = xt2 * xt;

                    var v0 = f0;
                    var vt = ft;

                    if (x <= xt)
                        d = m0 * x2 / 2 + v0 * x3 / 6;
                    else
                        d = m0 * xt2 / 2 + v0 * xt3 / 6 + x3 * (v0 / 6 + vt / 6) + x2 * (m0 / 2 + mt / 2 - vt * xt / 2) + x * (-mt * xt + vt * xt2 / 2) - xt3 * (v0 / 6 + vt / 6) - xt2 * (m0 / 2 + mt / 2 - vt * xt / 2) - xt * (-mt * xt + vt * xt2 / 2);

                    d = d / (e * I);
                }

                var buf = new Displacement();

                if (this.Direction == BeamDirection.Y)
                    buf.DZ = d;
                else
                    buf.DY = d;

                return buf;
            }
        }

        private Displacement GetLoadDisplacementAt_PartialNonUniformLoad_uniformSection(BarElement bar, PartialNonUniformLoad load, double xi)
        {
            throw new NotImplementedException();
            /*
            double ksi0, ksi1;//start and end of load region

            double f0, m0, w0;
            double L;

            if (bar.NodeCount != 2)
                throw new Exception();

            {//step 0
                L = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;
            }

            #region step 1
            {
                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        f0 = p0.Fz;
                        m0 = p0.My;//TODO: add possible negative sign
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BeamDirection.Z:
                        f0 = p0.Fy;
                        m0 = -p0.Mz;//TODO: add possible negative sign
                        w0 = localDir.Y * load.Magnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            #endregion

            #region step2
            double[] xs;

            {
                var n_ = bar.Section.GetMaxFunctionOrder()[0];
                var m_ = bar.Material.GetMaxFunctionOrder()[0];
                var sn_ = 3 + 2 * n_ + 2 * m_;

                xs = CalcUtil.DivideSpan(0, L, sn_ - 1);
            }
            #endregion

            double[] mOverEis;

            #region step3-4
            {
                var moverEi = new Func<double, double>(x =>
                {
                    var xi_ = (2 * x - L) / L;

                    var mat_ = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi_, 0, 0), bar);
                    var sec_ = bar.Section.GetCrossSectionPropertiesAt(xi_, bar);

                    var m_x = w0 * x * x / 2 + f0 * x + m0;

                    var e_x = mat_.Ex;
                    var i_x = this.Direction == BeamDirection.Y ? sec_.Iy : sec_.Iz;

                    return m_x / (e_x * i_x);
                });

                mOverEis = xs.Select(x_ => moverEi(x_)).ToArray();
            }
            #endregion


            Polynomial1D G;

            #region step5
            {
                //MathNet.Numerics.LinearAlgebra.
                G = Polynomial1D.FromPoints(xs, mOverEis);
            }
            #endregion

            double delta0, theta0;

            #region step6
            {
                delta0 = theta0 = 0;
            }
            #endregion

            Displacement buf;

            {
                buf = new Displacement();

                var x = (xi + 1) * L / 2;
                //var intg=G.EvaluateNthIntegralAt
                var delta = G.EvaluateNthIntegral(2, x)[0];

                if (Direction == BeamDirection.Y)
                    buf.DZ = delta;

                if (Direction == BeamDirection.Z)
                    buf.DY = delta;
            }

            return buf;
            */
        }

        #endregion

        /// <inheritdoc/>
        [TodoDelete]
        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt_old(Element targetElement, ElementalLoad load,
            double[] isoLocation)
        {

            var n = targetElement.Nodes.Length;

            var buff = new List<Tuple<DoF, double>>();

            var tr = targetElement.GetTransformationManager();

            var br = targetElement as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(targetElement, load);

            for (var i = 0; i < n; i++)
                endForces[i] = -endForces[i];//(2,1) section

            #region 2,1 (due to inverse of equivalent nodal loads)

            Force ends;//internal force in x=0 due to inverse of equivalent nodal loads will store in this variable, 

            {
                var xi_s = new double[br.Nodes.Length];//xi loc of each force
                var x_s = new double[br.Nodes.Length];//x loc of each force

                for (var i = 0; i < xi_s.Length; i++)
                {
                    var x_i = targetElement.Nodes[i].Location - targetElement.Nodes[0].Location;
                    var xi_i = br.LocalCoordsToIsoCoords(x_i.Length)[0];

                    xi_s[i] = xi_i;
                    x_s[i] = x_i.X;
                }

                ends = new Force();//sum of moved end forces to destination

                for (var i = 0; i < n; i++)
                {
                    if (xi_s[i] < isoLocation[0])
                    {
                        var frc_i = endForces[i];// new Force();

                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }
                }
            }

            #endregion

            var to = Iso2Local(targetElement, isoLocation)[0];


            #region uniform & trapezoid, uses integration method

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
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
                    xi1 = 1;
                    degree = 0;
                }
                else if (load is PartialNonUniformLoad)
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];
                }
                else
                    throw new NotImplementedException();

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = 0;// GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 3;//gauss point count

                    Matrix integral;

                    double i0 = 0, i1 = 0;//span for integration

                    var xi_t = isoLocation[0];

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
                            integral = targetElement.MatrixPool.Allocate(2, 1);
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

                                if (Direction == BeamDirection.Y)
                                {
                                    df = q_.Z;
                                    dm = -q_.Z * xx;
                                }
                                else
                                {
                                    df = q_.Y;
                                    dm = q_.Y * xx;
                                }

                                var buf_ = targetElement.MatrixPool.Allocate(new double[] { df, dm });

                                return buf_;
                            }, x0, x1, gpt);

                            integral = intgV.Integrate();
                        }
                    }
                    #endregion


                    var v_i = integral[0, 0];//total shear
                    var m_i = integral[1, 0];//total moment of load about the start node

                    var x = Iso2Local(targetElement, isoLocation)[0];

                    var f = new Force();//total moment about start node, total shear 


                    if (Direction == BeamDirection.Y)
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

                    if (Direction == BeamDirection.Y)
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

            #region concentrated

            if (load is ConcentratedLoad)
            {
                var cns = load as ConcentratedLoad;

                var xi = isoLocation[0];
                var targetX = br.IsoCoordsToLocalCoords(xi)[0];

                var frc = Force.Zero;

                if (cns.ForceIsoLocation.Xi < xi)
                    frc = cns.Force;

                if (cns.CoordinationSystem == CoordinationSystem.Global)
                    frc = tr.TransformGlobalToLocal(frc);

                var frcX = br.IsoCoordsToLocalCoords(cns.ForceIsoLocation.Xi)[0];

                if (frc != Force.Zero)
                {
                    frc = frc.Move(new Point(frcX, 0, 0), new Point(0, 0, 0));
                    frc = frc.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));
                }

                var movedEnds = ends.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var f2 = frc + movedEnds;
                f2 *= -1;


                if (Direction == BeamDirection.Y)
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

            #endregion

            throw new NotImplementedException();
        }

        #region GetLoadForce
        [TodoDelete]
        private Force GetLoadInternalForceAt_New(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            var bar = targetElement as BarElement;

            if (targetElement.Nodes.Length != 2)
                throw new Exception("more than two nodes not supported");

            if (load is UniformLoad ul)
                return GetLoadForceAt_UniformLoad(bar, ul, isoLocation[0]);

            if (load is PartialLinearLoad pnl)
                return GetLoadForceAt_PartialLinearLoad(bar, pnl, isoLocation[0]);


            throw new NotImplementedException();
        }
        [TodoDelete]
        private Force GetLoadForceAt_UniformLoad(BarElement bar, UniformLoad load, double xi)
        {
            double f0, m0, w0;
            double L;

            if (bar.NodeCount != 2)
                throw new Exception();

            {//the L
                L = bar.GetLength();
            }

            {//find f0,m0,w0, inverse of equivalent nodal loads applied to start node

                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        f0 = p0.Fz;
                        m0 = p0.My;//TODO: add possible negative sign
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BeamDirection.Z:
                        f0 = p0.Fy;
                        m0 = -p0.Mz;//TODO: add possible negative sign
                        w0 = localDir.Y * load.Magnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            double m_x, v_x;

            {//find M and V
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                m_x = w0 * x * x / 2 + f0 * x + m0;
                v_x = f0;
            }

            Force buf;

            {//substitude M and V to buf
                buf = new Force();

                if (Direction == BeamDirection.Y)
                {
                    buf.My = m_x;
                    buf.Fz = v_x;
                }

                if (Direction == BeamDirection.Z)
                {
                    buf.Mz = m_x;
                    buf.Fy = v_x;
                }
            }

            return buf;
        }
        [TodoDelete]
        private Force GetLoadForceAt_PartialLinearLoad(BarElement bar, PartialLinearLoad load, double xi)
        {
            double f0, m0, w0, w1, x0, x1;
            double L;

            if (bar.NodeCount != 2)
                throw new Exception();

            {//the L, x0, x1
                L = bar.GetLength();

                x0 = bar.IsoCoordsToLocalCoords(load.StartLocation.Xi)[0];
                x1 = bar.IsoCoordsToLocalCoords(load.EndLocation.Xi)[0];
            }

            {//find f0, m0, w0, w1 inverse of equivalent nodal loads applied to start node
                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();
               
                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        f0 = p0.Fz;
                        m0 = p0.My;//TODO: add possible negative sign
                        w0 = localDir.Z * load.StartMagnitude;
                        w1 = localDir.Z * load.EndMagnitude;
                        break;

                    case BeamDirection.Z:
                        f0 = p0.Fy;
                        m0 = -p0.Mz;//TODO: add possible negative sign
                        w0 = localDir.Y * load.StartMagnitude;
                        w1 = localDir.Y * load.EndMagnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            double m_x, v_x;

            {//find M and V
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                var poly = Polynomial1D.FromPoints(x0, w0, x1, w1);

                var intg = new StepFunctionIntegralCalculator();

                intg.Polynomial = poly;

                v_x = intg.CalculateIntegralAt(x0, x, 1) - intg.CalculateIntegralAt(x1, x, 1);
                m_x = intg.CalculateIntegralAt(x0, x, 2) - intg.CalculateIntegralAt(x1, x, 2);
            }

            Force buf;

            {//substitude M and V to buf
                buf = new Force();

                if (Direction == BeamDirection.Y)
                {
                    buf.My = m_x;
                    buf.Fz = v_x;
                }

                if (Direction == BeamDirection.Z)
                {
                    buf.Mz = m_x;
                    buf.Fy = v_x;
                }
            }

            return buf;
        }

        #endregion

        /// <inheritdoc/>
        public override Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var nc = targetElement.Nodes.Length;

            var ld = localDisplacements;

            Matrix B = targetElement.MatrixPool.Allocate(2, 4);

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException("isoCoords");

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var n = GetNMatrixAt(targetElement, isoCoords);

            //var d = GetDMatrixAt(targetElement, isoCoords);

            var u = targetElement.MatrixPool.Allocate(2 * nc, 1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();

            if (Direction == BeamDirection.Y)
            {
                for (int i = 0; i < nc; i++)
                {
                    u[2 * i + 0, 0] = ld[i].DZ;
                    u[2 * i + 1, 0] = ld[i].RY;
                }
            }
            else
            {
                for (int i = 0; i < nc; i++)
                {
                    u[2 * i + 0, 0] = ld[i].DY;
                    u[2 * i + 1, 0] = ld[i].RZ;
                }
            }

            var f = n * u;

            //var ei = d[0, 0];

            f.ScaleRow(1, 1 / j);

            var buf = new Displacement();

            if (Direction == BeamDirection.Y)
            {
                buf.DZ = f[0, 0];
                buf.RY = -f[1, 0];//TODO: Not sure why, but negative should be applied
            }
            else
            {
                buf.DY = f[0, 0];
                buf.RZ = f[1, 0];
            }

            return buf;
        }

        /// <inheritdoc/>
        public override IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var nc = targetElement.Nodes.Length;

            var ld = localDisplacements;

            Matrix B = targetElement.MatrixPool.Allocate(2, 4);

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException("isoCoords");

            var buf = new List<Tuple<DoF, double>>();

            if (Direction == BeamDirection.Y)
            {
                if (localDisplacements.All(i => i.DZ == 0 && i.RY == 0))
                    return buf;
            }
            else
            {
                if (localDisplacements.All(i => i.DY == 0 && i.RZ == 0))
                    return buf;
            }

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();


            var n = GetNMatrixAt(targetElement, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);

            var u = targetElement.MatrixPool.Allocate(2 * nc, 1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();

            if (Direction == BeamDirection.Y)
            {
                for (int i = 0; i < nc; i++)
                {
                    u[2 * i + 0, 0] = ld[i].DZ;
                    u[2 * i + 1, 0] = ld[i].RY;
                }
            }
            else
            {
                for (int i = 0; i < nc; i++)
                {
                    u[2 * i + 0, 0] = ld[i].DY;
                    u[2 * i + 1, 0] = ld[i].RZ;
                }
            }

            var f = n * u;

            var ei = d[0, 0];

            f.ScaleRow(1, 1 / j);
            f.ScaleRow(2, ei / (j * j));
            f.ScaleRow(3, ei / (j * j * j));

            if (Direction == BeamDirection.Y)
                f.ScaleRow(2, -1);//m/ei = - n''*u , due to where? TODO

            if (Direction == BeamDirection.Y)
            {
                buf.Add(Tuple.Create(DoF.Ry, f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dz, -f[3, 0]));
            }
            else
            {
                buf.Add(Tuple.Create(DoF.Rz, f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dy, -f[3, 0]));
            }

            return buf;
        }


        #region EQ nodal load

        [TodoDelete]
        /// <summary>
        /// load is uniform, but material or section is no uniform
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        private Force[] GetLocalEquivalentNodalLoads_uniformLoad_UniformMatSection(BarElement bar, UniformLoad load)
        {
            //where load is uniform and section or material is also uniform

            int c;
            double L;
            double w0;

            {//finding L
                L = bar.GetLength();
            }

            {//finding w0
                var localDir = load.Direction;

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = bar.GetTransformationManager().TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BeamDirection.Z:
                        w0 = localDir.Y * load.Magnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            var m0 = w0 * L * L / 12;
            var m1 = -w0 * L * L / 12;

            var f0 = -w0 * L / 2;
            var f1 = -w0 * L / 2;

            //m0,m1, f0,f1 are support reactions
            //eq nodal load is invert of support reaction

            var re0 = new Force();
            var re1 = new Force();

            if (this.Direction == BeamDirection.Y)
            {
                re0.Fz = f0;
                re1.Fz = f1;

                re0.My = m0;//possible negative
                re1.My = m1;//possible negative
            }
            else
            {
                re0.Fy = f0;
                re1.Fy = f1;

                re0.Mz = -m0;//possible negative
                re1.Mz = -m1;//possible negative
            }

            return new Force[] { -re0, -re1 };
        }


        [TodoDelete]
        /// <summary>
        /// load is uniform, but material or section is no uniform
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        private Force[] GetLocalEquivalentNodalLoads_uniformLoad_nonUniformMatSection(BarElement bar, UniformLoad load)
        {
            //where load is uniform and section or material is non uniform

            Polynomial1D oneOverEi, xOverEi, x2OverEi;

            int c;
            double L;
            double w0;

            {
                var n = bar.Material.GetMaxFunctionOrder()[0];
                var m = bar.Section.GetMaxFunctionOrder()[0];
                var p = 2;
                c = 2 * n + 2 * m + p;
                //c += 3;
                L = bar.GetLength();
            }

            {
                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (this.Direction)
                {
                    case BeamDirection.Y:
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BeamDirection.Z:
                        w0 = localDir.Y * load.Magnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            double[] xs;

            {
                xs = NumericUtils.DivideSpan(0, L, c);

                var y1s = new double[xs.Length];
                var y2s = new double[xs.Length];
                var y3s = new double[xs.Length];

                for (var i = 0; i < xs.Length; i++)
                {
                    var x = xs[i];
                    var xi = bar.LocalCoordsToIsoCoords(x)[0];

                    var mech = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);
                    var geo = bar.Section.GetCrossSectionPropertiesAt(xi, bar);

                    var I = this.Direction == BeamDirection.Y ? geo.Iy : geo.Iz;

                    var ei = mech.Ex * I;

                    y1s[i] = 1 / ei;
                    y2s[i] = x / ei;
                    y3s[i] = x * x / ei;
                }

                oneOverEi = Polynomial1D.FromPoints(xs, y1s);
                xOverEi = Polynomial1D.FromPoints(xs, y2s);
                x2OverEi = Polynomial1D.FromPoints(xs, y3s);

            }

            double y11, y22, y33;
            double y1, y2, y3;

            {
                y1 = oneOverEi.EvaluateNthIntegral(1, 0)[0] - oneOverEi.EvaluateNthIntegral(1, L)[0];
                y2 = xOverEi.EvaluateNthIntegral(1, 0)[0] - xOverEi.EvaluateNthIntegral(1, L)[0];
                y3 = x2OverEi.EvaluateNthIntegral(1, 0)[0] - x2OverEi.EvaluateNthIntegral(1, L)[0];

                y11 = oneOverEi.EvaluateNthIntegral(2, 0)[0] - oneOverEi.EvaluateNthIntegral(2, L)[0];
                y22 = xOverEi.EvaluateNthIntegral(2, 0)[0] - xOverEi.EvaluateNthIntegral(2, L)[0];
                y33 = x2OverEi.EvaluateNthIntegral(2, 0)[0] - x2OverEi.EvaluateNthIntegral(2, L)[0];
            }

            {
                if (bar.StartReleaseCondition != Constraints.Fixed || bar.EndReleaseCondition != Constraints.Fixed)
                    throw new Exception("Not supported end releases");
            }

            double f0, m0;

            {
                Utils.AlgebraUtils.Solve2x2(y1, y2, -w0 /2* y3,
                    y11, y22, -w0/2 * y33, out m0, out f0);
            }

            double f1, m1;

            {//for uniform load
                f1 = -(f0 + w0 * L);
                m1 = -(m0 + f0 * L + w0 * L * L / 2);
            }

            var p0 = new Force();
            var p1 = new Force();

            if (this.Direction == BeamDirection.Y)
            {
                p0.Fz = f0;
                p1.Fz = f1;

                p0.My = m0;//possible negative
                p1.My = m1;//possible negative
            }
            else
            {
                p0.Fy = f0;
                p1.Fy = f1;

                p0.Mz = -m0;//possible negative
                p1.Mz = -m1;//possible negative
            }
            /** /
            {//scale the output to reduce error

                var pt = (f1+f0) / (w0 * L );

                p0 = 1 / pt * p0 ;
                p1 = 1 / pt * p1 ;
            }
            /**/
            return new Force[] { -p0, -p1 };
        }

        #endregion

        [TodoDelete]
        /// <inheritdoc/>
        private Force[] GetLocalEquivalentNodalLoads_old(Element targetElement, ElementalLoad load)
        {
            var bar = targetElement as BarElement;
            var n = bar.Nodes.Length;

            var tDeg = 0;

            if (bar.Section != null)
                tDeg += bar.Section.GetMaxFunctionOrder()[0];

            if (bar.Section != null)
                tDeg += bar.Material.GetMaxFunctionOrder()[0];

            if (load is UniformLoad uldd )
            {
                if (tDeg == 0)
                    return GetLocalEquivalentNodalLoads_uniformLoad_UniformMatSection(bar, uldd);
                else
                    return GetLocalEquivalentNodalLoads_uniformLoad_nonUniformMatSection(bar, uldd);
            }
                //https://www.quora.com/How-should-I-perform-element-forces-or-distributed-forces-to-node-forces-translation-in-the-beam-element

                var tr = targetElement.GetTransformationManager();

            #region uniform 

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
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
                    xi1 = 1;
                    degree = 0;
                }
                else if (load is PartialNonUniformLoad)
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
                //else if (load is PartialLinearLoad)
                /*
                {
                    
                    var uld = load as PartialLinearLoad;

                    magnitude = xi => uld.GetMagnitudeAt.Evaluate(xi);
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];// Coefficients.Length;
                    
                }*/
                else
                    throw new NotImplementedException();

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                    {
                        var shp = GetNMatrixAt(targetElement, xi, 0, 0);

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
                        var j = GetJMatrixAt(targetElement, xi, 0, 0);
                        shp.Scale(j.Determinant());

                        var q_ = localDir * q__;

                        if (Direction == BeamDirection.Y)
                            shp.Scale(q_.Z);
                        else
                            shp.Scale(q_.Y);

                        return shp;
                    }, xi0, xi1, gpt);

                    var res = intg.Integrate();

                    if (n > 2)
                        throw new Exception("beam with more than 2 node not supported!");

                    var localForces = new Force[n];



                    if (Direction == BeamDirection.Y)
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

            #region ConcentratedLoad

            if (load is ConcentratedLoad)
            {
                var cl = load as ConcentratedLoad;

                var localforce = cl.Force;

                if (cl.CoordinationSystem == CoordinationSystem.Global)
                    localforce = tr.TransformGlobalToLocal(localforce);

                var buf = new Force[n];

                var ns = GetNMatrixAt(targetElement, cl.ForceIsoLocation.Xi);

                /*
                if (_direction == BeamDirection.Y)
                    for (var i = 0; i < ns.ColumnCount; i++)
                        if (i % 2 == 1)
                            ns.MultiplyColumnByConstant(i, -1);
                */


                var j = GetJMatrixAt(targetElement, cl.ForceIsoLocation.Xi);

                var detJ = j.Determinant();

                ns.ScaleRow(1, 1 / detJ);
                ns.ScaleRow(2, 1 / (detJ * detJ));
                ns.ScaleRow(3, 1 / (detJ * detJ * detJ));


                for (var i = 0; i < n; i++)
                {
                    var node = bar.Nodes[i];

                    var fi = new Force();

                    var ni = ns[0, 2 * i];
                    var mi = ns[0, 2 * i + 1];

                    var nip = ns[1, 2 * i];
                    var mip = ns[1, 2 * i + 1];

                    if (Direction == BeamDirection.Z)
                    {
                        fi.Fy += localforce.Fy * ni;//concentrated force
                        fi.Mz += localforce.Fy * mi;//concentrated force

                        fi.Fy += localforce.Mz * nip;//concentrated moment
                        fi.Mz += localforce.Mz * mip;//concentrated moment
                    }
                    else
                    {
                        fi.Fz += localforce.Fz * ni;//concentrated force
                        fi.My += localforce.Fz * mi;//concentrated force

                        fi.Fz += localforce.My * -nip;//concentrated moment
                        fi.My += localforce.My * -mip;//concentrated moment
                    }

                    buf[i] = fi;
                }

                return buf;
            }




            #endregion



            throw new NotImplementedException();
        }

        public override GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public override GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }



        public override DoF[] GetDofsPerNode()
        {
            var d = Direction == BeamDirection.Y ? DoF.Dz : DoF.Dy;
            var r = Direction == BeamDirection.Y ? DoF.Ry : DoF.Rz;

            return new DoF[] { d, r };
        }

        protected override int GetBOrder()
        {
            return 1;
        }

        protected override int GetNOrder()
        {
            return 3;
        }


        public override double GetMu(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mat = targetElement.Material.GetMaterialPropertiesAt(xi);

            return mat.Mu * geo.A;
        }

        public override double GetRho(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mat = targetElement.Material.GetMaterialPropertiesAt(xi);

            return mat.Rho * geo.A;
        }

        public override double GetD(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mech = targetElement.Material.GetMaterialPropertiesAt(xi);

            if (Direction == BeamDirection.Y)
                return geo.Iy * mech.Ex;
            else
                return geo.Iz * mech.Ex;
        }

        public override ILoadHandler[] GetLoadHandlers()
        {
            return new ILoadHandler[] {

                new LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler(),
                new LoadHandlers.EulerBernoulliBeamHelper2Node.ImposedStrain_UF_NUF_Handler(),
                new LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_UF_Handler(),
            };
        }

        public override int CalcLocalStiffnessMatrixSize(Element targetElement)
        {
            return 4;
        }



        #endregion
    }
}
