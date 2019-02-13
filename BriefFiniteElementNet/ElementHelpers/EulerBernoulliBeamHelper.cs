using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
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
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var NN = GetNMatrixAt(targetElement, isoCoords);

            if (_direction == BeamDirection.Y)
            {
                for (var i = 0; i < NN.ColumnCount; i++)
                {
                    if (i%2 == 1)
                        NN.MultiplyColumnByConstant(i, -1);

                    //NN.MultiplyColumnByConstant(1, -1);
                    //NN.MultiplyColumnByConstant(3, -1);
                }
                
                
            }
                        

            double[] arr = NN.ExtractRow(2).CoreArray;
            //arr is d²N/dξ²
            //but B is d²N/dx²
            //so B will be arr * dξ²/dx² = arr * 1/ j.det ^2
            //based on http://www-g.eng.cam.ac.uk/csml/teaching/4d9/4D9_handout2.pdf

            arr.MultiplyWithConstant(-1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();
            var j2 = j * j;


            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] *= 1 / j2;
            }
            
            var buf = new Matrix(1, arr.Length);

            buf.FillRow(0, arr);

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

        private class Condition
        {
            public Condition()
            {
            }

            public Condition(int nodeNumber, FunctionType type, double xi, int differentialDegree, double value)
            {
                NodeNumber = nodeNumber;
                Type = type;
                Xi = xi;
                DifferentialDegree = differentialDegree;
                RightSide = value;
            }

            public enum FunctionType
            {
                M,
                N
            }



            public int NodeNumber;

            public FunctionType Type;

            public double Xi;

            public int DifferentialDegree;

            public double RightSide;

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.Append(Type);
                sb.Append(NodeNumber);

                for (int i = 0; i < DifferentialDegree; i++)
                {
                    sb.Append("'");
                }
                sb.AppendFormat("({0}) = {1}", Xi, RightSide);
                return sb.ToString();
            }

            public class ConditionEqualityComparer : IComparer<Condition>
            {
                public int Compare(Condition x, Condition y)
                {
                    var t = 0;

                    if ((t = x.Type.CompareTo(y.Type)) != 0)
                        return t;

                    if ((t = x.NodeNumber.CompareTo(y.NodeNumber)) != 0)
                        return t;

                    if ((t = x.Xi.CompareTo(y.Xi)) != 0)
                        return t;

                    if ((t = x.DifferentialDegree.CompareTo(y.DifferentialDegree)) != 0)
                        return t;

                    if ((t = x.RightSide.CompareTo(y.RightSide)) != 0)
                        return t;

                    return t;
                }

               
            }



           
        }



        private Condition[] GetMCondition(Element targetElement,int baseNode, int targetNode,
            DofConstraint[] cv,
            DofConstraint[] cm)
        {
            //conditions for M_basenode(targetNode) and M_basenode`(targetNode)

            var nodeCount = cv.Length;

            var n = nodeCount;

            var xis = new Func<int, double>(i =>
            {
                var delta = 2.0 / (n - 1);

                return -1 + delta * i;
            });

            var lst = new List<Condition>();


            if (cm[baseNode] == DofConstraint.Fixed)
            {
                if (cv[targetNode] == DofConstraint.Fixed)
                {
                    lst.Add(new Condition(baseNode, Condition.FunctionType.M, xis(targetNode), 0, 0));
                }

                if (cm[targetNode] == DofConstraint.Fixed)
                {
                    var j = GetJMatrixAt(targetElement,xis(baseNode))[0, 0];

                    var val = baseNode == targetNode ? j : 0.0;

                    lst.Add(new Condition(baseNode, Condition.FunctionType.M, xis(targetNode), 1, val));
                }
            }

            return lst.ToArray();
        }


        private Condition[] GetNCondition(Element targetElement,int baseNode, int targetNode,
            DofConstraint[] cv,
            DofConstraint[] cm)
        {
            //conditions for M_basenode(targetNode) and M_basenode`(targetNode)

            var nodeCount = cv.Length;

            var n = nodeCount;

            var xis = new Func<int, double>(i =>
            {
                var delta = 2.0 / (n - 1);

                return -1 + delta * i;
            });

            var lst = new List<Condition>();


            if (cv[baseNode] == DofConstraint.Fixed)
            {
                if (cv[targetNode] == DofConstraint.Fixed)
                {
                    var val = baseNode == targetNode ? 1.0 : 0.0;

                    lst.Add(new Condition(baseNode, Condition.FunctionType.N, xis(targetNode), 0, val));
                }

                if (cm[targetNode] == DofConstraint.Fixed)
                {
                    lst.Add(new Condition(baseNode, Condition.FunctionType.N, xis(targetNode), 1, 0));
                }
            }

            return lst.ToArray();
        }


        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //note: this method gives the shape function on variable node count beam with variable constraint on each node 

            var cnds = new List<Condition>();


            var sb = new StringBuilder();
            //var conditions = new List<>();

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


            #region detect conditions

            for (var bnode = 0; bnode < n; bnode++)
            {
                //var mConds = new List<Condition>();

                for (var tnode = 0; tnode < n; tnode++)
                {
                    cnds.AddRange(GetMCondition(targetElement, bnode, tnode, cv, cm));
                    cnds.AddRange(GetNCondition(targetElement, bnode, tnode, cv, cm));
                }

                

            }
            #endregion

            cnds.Sort(new Condition.ConditionEqualityComparer());
            var grpd = cnds.GroupBy(i => Tuple.Create(i.NodeNumber, i.Type)).ToArray();

            var mss = new Polynomial[n];
            var nss = new Polynomial[n];

            foreach (var grp in grpd)
            {
                var nodeNum = grp.Key.Item1;
                var tp = grp.Key.Item2;
                var condCount = grp.Count();

                var mtx = new Matrix(condCount, condCount);
                var rightSide = new Matrix(condCount, 1);

                var arr = grp.ToArray();

                for (var i = 0; i < arr.Length; i++)
                {
                    var itm = arr[i];

                    mtx.SetRow(i, Diff(itm.Xi, condCount - 1, itm.DifferentialDegree));

                    rightSide.SetRow(i, itm.RightSide);
                }

                Polynomial pl;

                if (arr.Length != 0)
                {
                    var cfs = mtx.Inverse2() * rightSide;

                    pl = new Polynomial(cfs.CoreArray);
                }
                else
                {
                    pl = new Polynomial();
                }
                

                if (tp == Condition.FunctionType.M)
                    mss[nodeNum] = pl;

                if (tp == Condition.FunctionType.N)
                    nss[nodeNum] = pl;
            }

            var buf = new Matrix(4, 2 * n);

            for (var i = 0; i < n; i++)
            {
                if (nss[i] == null) nss[i] = new Polynomial();
                if (mss[i] == null) mss[i] = new Polynomial();
            }

            for (var i = 0; i < n; i++)
            {
                var cf = _direction == BeamDirection.Z ? 1 : -1;

                //var niCoefs = (ns[i].Inverse2() * rns[i]).CoreArray;
                //var miCoefs = (ms[i].Inverse2() * rms[i]).CoreArray;

                var ni = nss[i];// new Polynomial(niCoefs);
                var mi = mss[i];//new Polynomial(miCoefs);

                {
                    /*
                    var np = ni.GetDerivative(1);
                    var npp = ni.GetDerivative(2);

                    var mp = mi.GetDerivative(1);
                    var mpp = mi.GetDerivative(2);
                    */

                }
                for (var ii = 0; ii < 4; ii++)
                {
                    //var v1 = buf[ii, 2 * i + 0] = -ni.EvaluateDerivative(xi, ii);
                    //var v2 = buf[ii, 2 * i + 1] = cf * mi.EvaluateDerivative(xi, ii);

                    var nid = ni.GetDerivative(ii);
                    var mid =mi.GetDerivative(ii);

                    var v1 = buf[ii, 2*i + 0] = nid.Evaluate(xi);
                    var v2 = buf[ii, 2*i + 1] = mid.Evaluate(xi);
                }
            }

            return buf;
            throw new NotImplementedException();
        }


        public Matrix GetNMatrixAt_backup(Element targetElement, params double[] isoCoords)
        {
            //note: this method gives the shape function on variable node count beam with variable constraint on each node 

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
                            //N.SetRow(ncnt, Diff(xis(tnode), 2 * n - 1, 3));
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
                            //N.SetRow(ncnt, Diff(xis(tnode), 2 * n - 1, 2));
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
                            //M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 3));
                            if (bnode == tnode) mflags[bnode] = true;
                        }

                        mcnt++;

                        if (cm[tnode] == DofConstraint.Fixed)
                        {
                            M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 1));
                            if (bnode == tnode) rm.SetRow(mcnt, J[0, 0] * 1.0);
                        }
                        else
                        {
                            //M.SetRow(mcnt, Diff(xis(tnode), 2 * n - 1, 2));
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

                {
                    /*
                    var np = ni.GetDerivative(1);
                    var npp = ni.GetDerivative(2);

                    var mp = mi.GetDerivative(1);
                    var mpp = mi.GetDerivative(2);
                    */

                }
                for (var ii = 0; ii < 4; ii++)
                {
                    //var v1 = buf[ii, 2 * i + 0] = -ni.EvaluateDerivative(xi, ii);
                    //var v2 = buf[ii, 2 * i + 1] = cf * mi.EvaluateDerivative(xi, ii);

                    var v1 = buf[ii, 2 * i + 0] = ni.EvaluateDerivative(xi, ii);
                    var v2 = buf[ii, 2 * i + 1] = mi.EvaluateDerivative(xi, ii);
                }
            }

            return buf;
            throw new NotImplementedException();
        }
        private Polynomial GetNIth(Element targetElement, int ith, int disprot)
        {

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

            var conditions = new List<Tuple<double, double, int>>();
            //xi,value,diff deg

            for (var bnode = 0; bnode < n; bnode++)
            {
                var xi = xis(bnode);

                //test bnode + disp
                {
                    if (cv[bnode] == DofConstraint.Fixed)
                    {
                        conditions.Add(Tuple.Create(xi, (bnode == ith && disprot == 0) ? 1.0 : 0.0, 0));
                    }

                    /*
                    if (bnode == ith && disprot == 0)
                        conditions.Add(Tuple.Create(xi, 1.0, 0));
                    else if (cv[bnode] == DofConstraint.Fixed)
                    {
                        conditions.Add(Tuple.Create(xi, 0.0, 0));
                    }
                    */
                }

                //test bnode + rotation
                {
                    if (cm[bnode] == DofConstraint.Fixed)
                    {
                        conditions.Add(Tuple.Create(xi, (bnode == ith && disprot == 1) ? 1.0 : 0.0, 1));
                    }

                    /*
                    if (bnode == ith && disprot == 1)
                        conditions.Add(Tuple.Create(xi, 1.0, 1));
                    else if (cm[bnode] == DofConstraint.Fixed)
                    {
                        conditions.Add(Tuple.Create(xi, 0.0, 1));
                    }
                    */
                }
            }

            var d = conditions.Count;

            var mtx = new Matrix(d, d);
            var r = new Matrix(d, 1);

            for (var i = 0; i < d; i++)
            {
                var xi = conditions[i].Item1;

                for (var j = 0; j < d; j++)
                {
                    mtx[i, j] = Math.Pow(xi, j - d);
                    r[i, 0] = conditions[i].Item2;
                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the coeficient ??!
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="pOrder"></param>
        /// <param name="diffDegree"></param>
        /// <returns></returns>
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

            if (_direction == BeamDirection.Y)
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

            var x_xi = bar.GetIsoToLocalConverter();

            var buf = new Matrix(1, 1);
            //we need J = ∂X / ∂ξ = dX / dξ

            buf[0, 0] = x_xi.GetDerivative(1).Evaluate(isoCoords[0]);
            //var old = l / 2;
            return buf;

            /*old
            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var buf = new Matrix(1, 1);

            buf[0, 0] = (bar.EndNode.Location - bar.StartNode.Location).Length/2;

            return buf;
            */
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
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalMMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            return ElementHelperExtensions.CalcLocalCMatrix_Bar(this, targetElement);
        }

        /// <inheritdoc/>
        public FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            var buf = new List<ElementLocalDof>();

            for (var i = 0; i < targetElement.Nodes.Length; i++)
            {
                buf.Add(new ElementLocalDof(i, _direction == BeamDirection.Y ? DoF.Dz : DoF.Dy));
                buf.Add(new ElementLocalDof(i, _direction == BeamDirection.Y ? DoF.Ry : DoF.Rz));
            }

            return buf.ToArray();
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
        public int[] GetNMaxOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;
            return new int[] {2*n-1, 0, 0};
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;
            var t = 2*n - 3;
            return new[] {2*t,0,0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;
            return new int[] {n-1, 0, 0};
        }


        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, Load load,
            double[] isoLocation)
        {
            var n = targetElement.Nodes.Length;

            var buff = new List<Tuple<DoF, double>>();
            
            var tr = targetElement.GetTransformationManager();

            var br = targetElement as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(targetElement, load);

            //var buf = new Force();

            

            var xi_s = new double[br.Nodes.Length];//xi loc of each force
            var x_s = new double[br.Nodes.Length];//x loc of each force

            for (var i = 0; i < xi_s.Length; i++)
            {
                var x_i = targetElement.Nodes[i].Location - targetElement.Nodes[0].Location;
                var xi_i = br.LocalCoordsToIsoCoords(x_i.Length)[0];

                xi_s[i] = xi_i;
                x_s[i] = x_i.X;
            }

            var ends = new Force();//sum of moved end forces to destination

            for (var i = 0; i < n; i++)
            {
                if (xi_s[i] < isoLocation[0])
                {
                    var frc_i = new Force();

                    if (this._direction == BeamDirection.Y)
                    {
                        frc_i.My = endForces[i].My;
                        frc_i.Fz = endForces[i].Fz;
                    }
                    else
                    {
                        frc_i.Mz = endForces[i].Mz;
                        frc_i.Fy = endForces[i].Fy;
                    }

                    ends += frc_i.Move(Point.Origins, new Point(x_s[i], 0, 0));
                }
                
            }


            var to = Iso2Local(targetElement, isoLocation)[0];
            


            #region uniform & trapezoid, uses integration method

            if (load is UniformLoad)
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
                    throw new NotImplementedException();
                    /*
                    var tld = (load as PartialTrapezoidalLoad);

                    magnitude = (xi => (load as PartialTrapezoidalLoad).GetMagnitudesAt(xi, 0, 0)[0]);
                    localDir = tld.Direction;

                    if (tld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = tld.StartLocation[0];
                    xi1 = tld.EndLocation[0];
                    degree = 1;
                    */
                }

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = 0;// GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 3;//gauss point count

                    Matrix integral;

                    double i0=0, i1=0;//span for integration

                    var xi_t = isoLocation[0];

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


                    if (i1 == i0)
                    {
                        integral = new Matrix(2, 1);
                    }
                    else
                    {
                        var intgV = GaussianIntegrator.CreateFor1DProblem(xi =>
                        {
                            //var xi = Local2Iso(targetElement, x)[0];
                            var j = GetJMatrixAt(targetElement, xi);
                            
                            var q__ = magnitude(xi);
                            var q_ = localDir * q__;

                            double df, dm;

                            if (this._direction == BeamDirection.Y)
                            {
                                df = q_.Z;
                                dm = -q_.Z * (1+xi);
                            }
                            else
                            {
                                df = q_.Y;
                                dm = q_.Y * (1+xi);
                            }

                            var buf_ = new Matrix(new double[] { df, dm });
                            var detj = j.Determinant();

                            buf_[0, 0] = buf_[0, 0] * detj;
                            buf_[1, 0] = buf_[1, 0] * detj*detj;


                            return buf_;
                        }, i0, i1, gpt);

                        integral = intgV.Integrate();
                    }

                    var v_i = integral[0, 0];
                    var m_i = integral[1, 0];

                    var frc = new Force();

                    var x = Iso2Local(targetElement, isoLocation)[0];

                    if (this._direction == BeamDirection.Y)
                    {
                        var v0 = ends.Fz;
                        var m0 = ends.My;

                        var v1 = v_i - v0;
                        var m1 = -m0 - v1 * x - m_i;

                        frc.My = m1;
                        frc.Fz = v1;
                    }
                    else
                    {
                        var v0 = ends.Fy;
                        var m0 = ends.Mz;

                        var v1 = v_i - v0;
                        var m1 = -m0 - v1 * x + m_i;

                        frc.Mz = m1;
                        frc.Fy = v1;
                    }

                    //frc = frc + ends;

                    buff.Add(Tuple.Create(DoF.Ry, frc.My));
                    buff.Add(Tuple.Create(DoF.Rz, frc.Mz));
                    buff.Add(Tuple.Create(DoF.Dy, frc.Fy));
                    buff.Add(Tuple.Create(DoF.Dz, frc.Fz));

                    return buff;
                }
            }



            #endregion

            #region concentrated

            if (load is ConcentratedLoad)
            {
                var xi = isoLocation[0];
                var targetX = br.IsoCoordsToLocalCoords(xi)[0];

                var f0 = Force.Zero;

                for(var i = 0;i<targetElement.Nodes.Length;i++)
                {
                    var x_i = targetElement.Nodes[i].Location - targetElement.Nodes[0].Location;

                    var xi_i = br.LocalCoordsToIsoCoords(x_i.Length)[0];

                    if (xi_i < xi)
                    {
                        f0 += endForces[i].Move(new Point(x_i.Length, 0, 0), new Point(targetX, 0, 0));
                    }
                }

                buff.Add(Tuple.Create(DoF.Ry, f0.My));
                buff.Add(Tuple.Create(DoF.Rz, f0.Mz));
                buff.Add(Tuple.Create(DoF.Dy, f0.Fy));
                buff.Add(Tuple.Create(DoF.Dz, f0.Fz));

                return buff;
            }

            #endregion

            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public Displacement GetLoadDisplacementAt(Element targetElement, Load load, double[] isoLocation)
        {
            var n = this.GetNMatrixAt(targetElement, isoLocation);
            
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

            if (_direction == BeamDirection.Y)
            {
                //n.MultiplyColumnByConstant(1, -1);
            }
            else
            {
                n.MultiplyColumnByConstant(1, -1);
                n.MultiplyColumnByConstant(3, -1);
            }


            /*
            var nOld = GetNMatrixBar2Node(targetElement, isoCoords);

            var dd = (n - nOld).CoreArray.Max(i => Math.Abs(i));

            if (dd > 1e-10)
                throw new Exception();
            */

            var d = GetDMatrixAt(targetElement, isoCoords);

            var u = new Matrix(2 * nc, 1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();

            if (_direction == BeamDirection.Y)
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
                buf.DZ = f[0, 0];
                buf.RY = f[1, 0];
            }
            else
            {
                buf.DY = f[0, 0];
                buf.RZ = -f[1, 0];
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


            if (_direction == BeamDirection.Y)
            {
                //n.MultiplyColumnByConstant(1, -1);
                //n.MultiplyColumnByConstant(3, -1);

                for (var i = 0; i < n.ColumnCount; i++)
                {
                    if (i % 2 == 1)
                        n.MultiplyColumnByConstant(i, -1);
                }

            }


            var d = GetDMatrixAt(targetElement, isoCoords);

            var u = new Matrix(2 * nc, 1);

            var j = GetJMatrixAt(targetElement, isoCoords).Determinant();

            if (_direction == BeamDirection.Y)
            {
                for (int i = 0; i < nc; i++)
                {
                    u[2*i + 0, 0] = ld[i].DZ;
                    u[2*i + 1, 0] = ld[i].RY;
                }
                //u.FillColumn(0, ld[0].DZ, ld[0].RY, ld[1].DZ, ld[1].RY);
            }
            else
            {
                for (int i = 0; i < nc; i++)
                {
                    u[ 2 * i + 0,0] = ld[i].DY;
                    u[ 2 * i + 1,0] = ld[i].RZ;
                }

                //u.FillColumn(0, ld[0].DY, ld[0].RZ, ld[1].DY, ld[1].RZ);
            }


            var f = n * u;
            var f2 = d*GetBMatrixAt(targetElement, isoCoords)*u;

            var ei = d[0, 0];

            f.MultiplyRowByConstant(1, 1 / j);
            f.MultiplyRowByConstant(2, ei / (j * j));
            f.MultiplyRowByConstant(3, ei / (j * j * j));

            f.MultiplyRowByConstant(2, -1);//m/ei = - n''*u

            //var buf = new Displacement();
            var buf = new List<Tuple<DoF, double>>();

            /*
            if (_direction == BeamDirection.Y)
            {
                buf.Add(Tuple.Create(DoF.Ry, -f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dz, f[3, 0]));
            }
            else
            {
                buf.Add(Tuple.Create(DoF.Rz, f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dy, f[3, 0]));
            }
            */


            if (_direction == BeamDirection.Y)
            {
                buf.Add(Tuple.Create(DoF.Ry, f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dz, -f[3, 0]));
            }
            else
            {
                buf.Add(Tuple.Create(DoF.Rz, -f[2, 0]));
                buf.Add(Tuple.Create(DoF.Dy, -f[3, 0]));
            }

            return buf;
        }


        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, Load load)
        {
            var bar = targetElement as BarElement;
            var n = bar.Nodes.Length;


            //https://www.quora.com/How-should-I-perform-element-forces-or-distributed-forces-to-node-forces-translation-in-the-beam-element

            var tr = targetElement.GetTransformationManager();

            #region uniform & trapezoid

            if (load is UniformLoad)
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
                    throw new NotImplementedException();
                    /*
                    var tld = (load as PartialTrapezoidalLoad);

                    magnitude = (xi => (load as PartialTrapezoidalLoad).GetMagnitudesAt(xi, 0, 0)[0]);
                    localDir = tld.Direction;

                    if (tld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = tld.StartLocation[0];
                    xi1 = tld.EndLocation[0];
                    degree = 1;
                    */
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

            #region uniform & trapezoid

            if (load is ConcentratedLoad)
            {
                var cl = load as ConcentratedLoad;

                var localforce = cl.Force;

                if (cl.CoordinationSystem == CoordinationSystem.Global)
                    localforce = tr.TransformGlobalToLocal(localforce);

                var buf = new Force[n];

                var ns = GetNMatrixAt(targetElement, cl.ForceIsoLocation);

                for (var i = 0; i < n; i++)
                {
                    var node = bar.Nodes[i];

                    var fi = new Force();

                    var ni = ns[0, 2 * i];
                    var mi = ns[0, 2 * i + 1];

                    var nip = ns[1, 2 * i];
                    var mip = ns[1, 2 * i + 1];

                    if (this._direction == BeamDirection.Z)
                    {
                        fi.Fy += localforce.Fy * ni;//concentrated force
                        fi.Mz += localforce.Fy * mi;//concentrated force

                        fi.Fy += -localforce.Mz * nip;//concentrated moment
                        fi.Mz += -localforce.Mz * mip;//concentrated moment
                    }
                    else
                    {
                        fi.Fz += localforce.Fz * ni;//concentrated force
                        fi.My += localforce.Fz * mi;//concentrated force

                        fi.Fz += -localforce.My * nip;//concentrated moment
                        fi.My += -localforce.My * mip;//concentrated moment
                    }

                    buf[i] = fi;
                }

                return buf;
            }
            



            #endregion

            throw new NotImplementedException();
        }
    }
}
