using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using ElementLocalDof = BriefFiniteElementNet.FluentElementPermuteManager.ElementLocalDof;
using BriefFiniteElementNet.Mathh;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents a helper class for Euler - Bernoulli beam element
    /// </summary>
    public class EulerBernoulliBeamHelper : IElementHelper
    {
        private BeamDirection _direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EulerBernoulliBeamHelper"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public EulerBernoulliBeamHelper(BeamDirection direction)
        {
            _direction = direction;
        }

        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //TODO: Take end supports into consideration

            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var L = (elm.EndNode.Location - elm.StartNode.Location).Length;

            var L2 = L*L;

            

            var NN = GetNMatrixAt(targetElement, isoCoords);

            double[] arr = NN.ExtractRow(2).CoreArray;

            var arr2 = (double[])null;

            arr[0] *= 4 / L2;
            arr[1] *= 4 / L2;
            arr[2] *= 4 / L2;
            arr[3] *= 4 / L2;

            

            if (_direction == BeamDirection.Z)
            {
                arr2 = new double[] { -(6 * xi) / L2, (3 * xi) / L - 1 / L, +(6 * xi) / L2, (3 * xi) / L + 1 / L };
            }
            else
            {
                arr2 = new double[] { (6 * xi) / L2, (3 * xi) / L - 1 / L, -(6 * xi) / L2, (3 * xi) / L + 1 / L };
                arr.MultiplyWithConstant(-1);
            }


            var diff = (double[]) arr.Clone();

            CalcUtil.AddToSelf(diff, arr2, -1);

            var err = diff.Max(i => Math.Abs(i));

            if (err > 1e-10)
                throw new Exception();

            var buf = new Matrix(1, 4);

            buf.FillRow(0, arr2);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var L = (elm.EndNode.Location - elm.StartNode.Location).Length;

            var L2 = L * L;

            double bufVal;

            switch(i)
            {
                case 0:
                    bufVal = _direction == BeamDirection.Z ? -(6 * xi) / L2 : -(6 * xi) / L2;
                    break;

                case 1:
                    bufVal = (3 * xi) / L - 1 / L;
                    break;

                case 2:
                    bufVal = _direction == BeamDirection.Z ? +(6 * xi) / L2 : -(6 * xi) / L2;
                    break;

                case 3:
                    bufVal = (3 * xi) / L + 1 / L;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var buf = new Matrix(1, 1);

            buf.SetMember(0, 0, bufVal);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            var ei = 0.0;

            if (_direction == BeamDirection.Y)
                ei = geo.Iy * mech.Ex;
            else
                ei = geo.Iz * mech.Ex;

            buf[0, 0] = ei;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A*mech.Rho;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A * mech.Mu;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //if (targetElement is BarElement)
            //    return GetNMatrixBar2Node(targetElement, isoCoords);

            var xi = isoCoords[0];

            var bar = targetElement as BarElement;
            var l = (bar.StartNode.Location - bar.EndNode.Location).Length;

            if (bar == null)
                return null;

            var n = bar.NodeCount;

            var xis = new Func<int, double>(i =>
            {
                var delta = 2.0 / (n - 1);

                return -1 + delta * i;
            });


            var ms = new Matrix[n];
            var ns = new Matrix[n];



            var rms = new Matrix[n];
            var rns = new Matrix[n];

            var nflags = new bool[n];
            var mflags = new bool[n];

            var cv = new DofConstraint[n];//shear constraint
            var cm = new DofConstraint[n];//moment constraints

            for (var i = 0; i < n; i++)
            {
                if (this._direction == BeamDirection.Z)
                {
                    cv[i] = bar.NodalReleaseConditions[i].DY;
                    cm[i] = bar.NodalReleaseConditions[i].RZ;
                }
                else
                {
                    cv[i] = bar.NodalReleaseConditions[i].DZ;
                    cm[i] = bar.NodalReleaseConditions[i].RY;
                }
            }

            for (var bnode = 0; bnode < n; bnode++)
            {
                {
                    var N = ns[bnode] = new Matrix(2 * n, 2 * n);
                    var rn = rns[bnode] = new Matrix(2 * n, 1);
                    var ncnt = 0;

                    for (var tnode = 0; tnode < n; tnode++)
                    {
                        if (cv[tnode] == DofConstraint.Fixed)
                        {
                            N.SetRow(ncnt, Diff(xis(tnode), 2 * n - 1, 0));
                            if (bnode == tnode) rn.SetRow(ncnt, 1.0);
                        }
                        else
                        {
                            N.SetRow(ncnt, Diff(xis(tnode), 2 * n - 1, 3));
                            if (bnode == tnode) nflags[bnode] = true;
                        }

                        ncnt++;

                        if (cm[tnode] == DofConstraint.Fixed)
                        {
                            N.SetRow(ncnt, Diff(xis(tnode), 2 * n - 1, 1));
                            if (bnode == tnode) rn.SetRow(ncnt, 0.0);
                        }
                        else
                        {
                            N.SetRow(ncnt, Diff(xis(tnode), 2 * n - 1, 2));
                            if (bnode == tnode) nflags[bnode] = true;
                        }

                        ncnt++;
                    }
                }

                var J = GetJMatrixAt(targetElement, isoCoords);
                var detJ = J.Determinant();

                {
                    var M = ms[bnode] = new Matrix(2 * n, 2 * n);
                    var rm = rms[bnode] = new Matrix(2 * n, 1);
                    var mcnt = 0;

                    for (var tnode = 0; tnode < n; tnode++)
                    {
                        if (cv[tnode] == DofConstraint.Fixed)
                        {
                            M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 0));
                            if (bnode == tnode) rm.SetRow(mcnt, 0.0);
                        }
                        else
                        {
                            M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 3));
                            if (bnode == tnode) mflags[bnode] = true;
                        }

                        mcnt++;

                        if (cm[tnode] == DofConstraint.Fixed)
                        {
                            M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 1));
                            if (bnode == tnode) rm.SetRow(mcnt, detJ * 1.0);
                        }
                        else
                        {
                            M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 2));
                            if (bnode == tnode) mflags[bnode] = true;
                        }

                        mcnt++;
                    }
                }

            }

            var buf = new Matrix(4, 2 * n);

            for (var i = 0; i < n; i++)
            {
                var cf = _direction == BeamDirection.Z ? 1 : -1;

                var niCoefs = (ns[i].Inverse2() * rns[i]).CoreArray;
                var miCoefs = (ms[i].Inverse2() * rms[i]).CoreArray;

                var ni = new Polynomial(niCoefs);
                var mi = new Polynomial(miCoefs);

                for (var ii = 0; ii < 4; ii++)
                {
                    var v1 = buf[ii,2 * i + 0] = -ni.EvaluateDerivative(xi,ii);
                    var v2 = buf[ii, 2 * i + 1] = cf * mi.EvaluateDerivative(xi, ii);
                }
            }

            return buf;
            throw new NotImplementedException();
        }

        public double[] Diff(double xi,int pOrder,int diffDegree)
        {
            var buf = new double[pOrder + 1];

            for(var i = 0;i<buf.Length;i++)
            {
                var origPow = pOrder - i;

                var v = buf[i] = NthDer(xi, origPow, diffDegree);
            }

            return buf;
        }

        
        /// <summary>
        /// returns value of n'th derivative of F where F(x) = x ^ n
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public double NthDer(double x,int n,int m)
        {
            var pval = Math.Pow(x, n - m);

            if (m > n) return 0;

            return Factorial(n) / Factorial(n - m) * pval;
        }

        int Factorial(int x)
        {
            if (x < 0)
            {
                return -1;
            }
            else if (x == 1 || x == 0)
            {
                return 1;
            }
            else
            {
                return x * Factorial(x - 1);
            }
        }

        public Matrix GetNMatrixBar2Node(Element targetElement, params double[] isoCoords)
        {
            //for end release handling see http://www.serendi-cdi.org/serendipedia/index.php?title=Beam_Shape_Functions

           

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var L = (bar.EndNode.Location - bar.StartNode.Location).Length;

            var n1s = new double[] //N1,N1', N1'', N1'''
            {
                1.0 / 4.0 * (1 - xi) * (1 - xi) * (2 + xi),
                1.0 / 4.0 * (3 * xi * xi - 3),
                1.0 / 4.0 * (6 * xi),
                1.0 / 4.0 * (6)
            };

            var m1s = new double[] //M1,M1', M1'', M1'''
            {
                L / 8.0 * (1 - xi) * (1 - xi) * (xi + 1),
                L / 8.0 * (3 * xi * xi - 2 * xi - 1),
                L / 8.0 * (6 * xi - 2.0),
                L / 8.0 * (6),
            };

            var n2s = new double[] //N2,N2', N2'', N2'''
            {
                1.0 / 4.0 * (1 + xi)*(1 + xi)*(2 - xi),
                1.0 / 4.0 * (-3 * xi * xi + 3),
                1.0 / 4.0 * (-6 * xi),
                1.0 / 4.0 * (-6)
            };

            var m2s = new double[] //M1,M1', M1'', M1'''
            {
                L / 8.0 * (1 + xi)*(1 + xi)*(xi - 1),
                L / 8.0 * (3 * xi * xi + 2 * xi - 1),
                L / 8.0 * (6 * xi + 2.0),
                L / 8.0 * (6),
            };

            var buf2 = new Matrix(4, 4);


            var c1 = bar.StartReleaseCondition;
            var c2 = bar.EndReleaseCondition;

            if (_direction == BeamDirection.Z)
            {

            }
            else
            {
                m1s = m1s.Negate();
                m2s = m2s.Negate();

            }


            buf2.FillColumn(0, n1s);
            buf2.FillColumn(1, m1s);
            buf2.FillColumn(2, n2s);
            buf2.FillColumn(3, m2s);

            return buf2;
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var buf = new Matrix(1, 1);

            buf[0, 0] = (bar.EndNode.Location - bar.StartNode.Location).Length/2;

            return buf;
        }

        public double[] Iso2Local(Element targetElement, params double[] isoCoords)
        {
            var tg = targetElement as BarElement;


            if (tg != null)
            {
                var xi = isoCoords[0];

                if (tg.Nodes.Length == 2)
                {
                    var l = (tg.Nodes[1].Location - tg.Nodes[0].Location).Length;
                    return new[] { l * (xi + 1) / 2 };
                }
            }

            throw new NotImplementedException();
        }

        public double[] Local2Iso(Element targetElement, params double[] localCoords)
        {
            var tg = targetElement as BarElement;


            if (tg != null)
            {
                var x = localCoords[0];

                if (tg.Nodes.Length == 2)
                {
                    var l = (tg.Nodes[1].Location - tg.Nodes[0].Location).Length;
                    return new[] { 2 * x / l - 1 };
                }
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix CalcLocalKMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalMMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalMMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalCMatrix(Element targetElement)
        {
            return ElementHelperExtensions.CalcLocalCMatrix_Bar(this, targetElement);
        }


        /// <inheritdoc/>
        public FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new FluentElementPermuteManager.ElementLocalDof[]
            {
                new FluentElementPermuteManager.ElementLocalDof(0, _direction == BeamDirection.Y ? DoF.Dy : DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(0, _direction == BeamDirection.Y ? DoF.Rz : DoF.Ry),
                new FluentElementPermuteManager.ElementLocalDof(1, _direction == BeamDirection.Y ? DoF.Dy : DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(1, _direction == BeamDirection.Y ? DoF.Rz : DoF.Ry),
            };
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] {3, 0, 0};
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new[] {1,0,0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] {0, 0, 0};
        }


        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, Load load,
            double[] isoLocation)
        {
            var buf = new FlatShellStressTensor();
            
            var tr = targetElement.GetTransformationManager();

            var br = targetElement as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(targetElement, load);


            var v0 =
                this._direction == BeamDirection.Z ?
                endForces[0].Fy : endForces[0].Fz;

            var m0 = this._direction == BeamDirection.Z ?
                endForces[0].Mz : endForces[0].My;

            v0 = -v0;
            m0 = -m0;

            var to = Iso2Local(targetElement, isoLocation)[0];

            //var xi = isoLocation[0];

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialTrapezoidalLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                int degree;//polynomial degree of magnitude function

                #region inits
                if (load is UniformLoad)
                {
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = -1;
                    xi1 = 1;
                    degree = 0;
                }
                else
                {
                    var tld = (load as PartialTrapezoidalLoad);

                    magnitude = (xi => (load as PartialTrapezoidalLoad).GetMagnitudesAt(xi, 0, 0)[0]);
                    localDir = tld.Direction;

                    if (tld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = tld.StarIsoLocations[0];
                    xi1 = tld.EndIsoLocations[0];
                    degree = 1;
                }

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = 0;// GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    Matrix integral;


                    if(isoLocation[0]<xi0)
                    {
                        integral = new Matrix(2, 1);
                    }
                    else
                    {
                        var intgV = GaussianIntegrator.CreateFor1DProblem(x =>
                        {
                            var xi = Local2Iso(targetElement, x)[0];
                            var q__ = magnitude(xi);
                            var q_ = localDir * q__;

                            double df, dm;

                            if (this._direction == BeamDirection.Y)
                            {
                                df = q_.Z;
                                dm = -q_.Z * x;
                            }
                            else
                            {
                                df = q_.Y;
                                dm = q_.Y * x;
                            }

                            var buf_ = new Matrix(new double[] { df, dm });

                            return buf_;
                        }, 0, to, gpt);

                        integral = intgV.Integrate();
                    }

                    var v_i = integral[0, 0];
                    var m_i = integral[1, 0];

                    var memb = buf.MembraneTensor;
                    var bnd = buf.BendingTensor;

                    if (this._direction == BeamDirection.Y)
                    {
                        var v = memb.S12 = memb.S21 = -(v_i + v0);
                        bnd.M13 = bnd.M31 = -(m0 + m_i + (v * to * -1));
                    }
                    else
                    {
                        var v = memb.S13 = memb.S31 = -(v_i + v0);
                        bnd.M12 = bnd.M21 = -(m0 + m_i + (v * to * +1));
                    }

                    buf.MembraneTensor = memb;
                    buf.BendingTensor= bnd;

                    //return buf;
                }
            }



            #endregion

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Displacement GetLoadDisplacementAt(Element targetElement, Load load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        
        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var nc = targetElement.Nodes.Length;

            var ld = localDisplacements;

            Matrix B = new Matrix(2, 4);

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var n = GetNMatrixAt(targetElement, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);

            var u = new Matrix(2 * nc, 1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();

            if (_direction == BeamDirection.Z)
                u.FillColumn(0, ld[0].DZ, ld[0].RY, ld[1].DZ, ld[1].RY);
            else
                u.FillColumn(0, ld[0].DY, ld[0].RZ, ld[1].DY, ld[1].RZ);

            var f = n * u;

            var ei = d[0, 0];

            f.MultiplyRowByConstant(1, 1 / j);
            f.MultiplyRowByConstant(2, ei / (j * j));
            f.MultiplyRowByConstant(3, ei / (j * j * j));

            var buf = new Displacement();

            if (_direction == BeamDirection.Y)
            {
                buf.DY = f[0, 0];
                buf.RZ = -f[1, 0];
            }
            else
            {
                buf.DZ = f[0, 0];
                buf.RY = f[1, 0];
            }
            
            return buf;
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var nc = targetElement.Nodes.Length;

            var ld = localDisplacements;

            Matrix B = new Matrix(2, 4);

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var n = GetNMatrixAt(targetElement, isoCoords);

            var oldDir = this._direction;

            //TODO: this is very odd and not true.
            //TODO: should not change the _direction.
            this._direction = this._direction == BeamDirection.Y ? BeamDirection.Z : BeamDirection.Y;
            var d = GetDMatrixAt(targetElement, isoCoords);
            this._direction = oldDir;

            var u = new Matrix(2 * nc, 1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();

            if (_direction == BeamDirection.Y)
                u.FillColumn(0, ld[0].DZ, ld[0].RY, ld[1].DZ, ld[1].RY);
            else
                u.FillColumn(0, ld[0].DY, ld[0].RZ, ld[1].DY, ld[1].RZ);

            var ei = d[0, 0];

            n.MultiplyRowByConstant(1, 1 / j);
            n.MultiplyRowByConstant(2, ei / (j * j));
            n.MultiplyRowByConstant(3, ei / (j * j * j));

            var f =  n * u;

            f.MultiplyByConstant(-1);
            
            var buf = new List<Tuple<DoF, double>>();

            if (_direction == BeamDirection.Y)
            {
                buf.Add(Tuple.Create(DoF.Ry, f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dz, f[3, 0]));
            }
            else
            {
                buf.Add(Tuple.Create(DoF.Rz, -f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dy, f[3, 0]));
            }

            return buf;
        }


        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, Load load)
        {
            //https://www.quora.com/How-should-I-perform-element-forces-or-distributed-forces-to-node-forces-translation-in-the-beam-element

            var tr = targetElement.GetTransformationManager();

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialTrapezoidalLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                int degree;//polynomial degree of magnitude function

                #region inits
                if (load is UniformLoad)
                {
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = -1;
                    xi1 = 1;
                    degree = 0;
                }
                else
                {
                    var tld = (load as PartialTrapezoidalLoad);

                    magnitude = (xi => (load as PartialTrapezoidalLoad).GetMagnitudesAt(xi, 0, 0)[0]);
                    localDir = tld.Direction;

                    if (tld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = tld.StarIsoLocations[0];
                    xi1 = tld.EndIsoLocations[0];
                    degree = 1;
                }

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                    {
                        var shp = GetNMatrixAt(targetElement, xi, 0, 0);
                        var q__ = magnitude(xi);
                        var j = GetJMatrixAt(targetElement, xi, 0, 0);
                        shp.MultiplyByConstant(j.Determinant());

                        var q_ = localDir * q__;

                        if (this._direction == BeamDirection.Y)
                            shp.MultiplyByConstant(q_.Z);
                        else
                            shp.MultiplyByConstant(q_.Y);

                        return shp;
                    }, xi0, xi1, gpt);

                    var res = intg.Integrate();

                    var localForces = new Force[2];

                    if (this._direction == BeamDirection.Y)
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

            

            throw new NotImplementedException();
        }
    }
}
