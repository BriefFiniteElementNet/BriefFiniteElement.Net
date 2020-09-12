
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using ElementLocalDof = BriefFiniteElementNet.ElementPermuteHelper.ElementLocalDof;
using BriefFiniteElementNet.Mathh;
using CSparse.Storage;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet;

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
        public EulerBernoulliBeamHelper(BeamDirection direction,Element targetElement)
        {
            _direction = direction;
            TargetElement = targetElement;

            {//loading condistions list pool from element's cache
                var listPoolKey = "26167C0A-1E58-4FA5-950D-5464ED6F264A";

                object obj;

                if (targetElement != null)
                    targetElement.TryGetCache(listPoolKey, out CondsListPool);

                if (CondsListPool == null)
                {
                    CondsListPool = new ListPool<Condition>();

                    if (targetElement != null)
                        targetElement.SetCache(listPoolKey, CondsListPool);
                }
            }
           
        }

        /// <summary>
        /// The target element
        /// </summary>
        public Element TargetElement { get; set; }


        /// <summary>
        /// Gets the direction of Beam (rotation in Y or Z direction)
        /// </summary>
        public BeamDirection Direction { get { return _direction; } }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            SingleVariablePolynomial[] nss = null;
            SingleVariablePolynomial[] mss = null;

            var bar = targetElement as BarElement;

            if (!GetShapeFunctions(targetElement, out nss, out mss))
                throw new Exception();

            var n = bar.NodeCount;

            var xi = isoCoords[0];

            var buf =
                targetElement.MatrixPool.Allocate(1, 2 * n);



            for (var i = 0; i < n; i++)
            {
                var ni = nss[i];
                var mi = mss[i];

                buf[0, 2 * i + 0] = ni.EvaluateDerivative(xi, 2);
                buf[0, 2 * i + 1] = mi.EvaluateDerivative(xi, 2);
            }

            //buff is d²N/dξ²
            //but B is d²N/dx²
            //so B will be arr * dξ²/dx² = arr * 1/ j.det ^2
            //based on http://www-g.eng.cam.ac.uk/csml/teaching/4d9/4D9_handout2.pdf

            var J = GetJMatrixAt(targetElement, isoCoords);
            var detJ = J.Determinant();
            J.ReturnToPool();

            buf.MultiplyRowByConstant(0, 1 / (detJ * detJ));

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

            var buf = 
                //new Matrix(1, 1);
                targetElement.MatrixPool.Allocate(1, 1);

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

            var buf = targetElement.MatrixPool.Allocate(1, 1);

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

            var buf = targetElement.MatrixPool.Allocate(1, 1);

            buf[0, 0] = geo.A * mech.Mu;

            return buf;
        }


        

        #region partial end release stuff

        internal class Condition
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
                M = 0,
                N = 1
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

        private List<Condition> GetMCondition(Element targetElement, int baseNode, int targetNode,
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

            var lst = CondsListPool.Allocate();


            if (cm[baseNode] == DofConstraint.Fixed)
            {
                if (cv[targetNode] == DofConstraint.Fixed)
                {
                    lst.Add(new Condition(baseNode, Condition.FunctionType.M, xis(targetNode), 0, 0));
                }
                else
                {
                    if (targetNode == 0 || targetNode == nodeCount - 1)
                        lst.Add(new Condition(baseNode, Condition.FunctionType.M, xis(targetNode), 3, 0));
                }

                if (cm[targetNode] == DofConstraint.Fixed)
                {
                    var jj = GetJMatrixAt(targetElement, xis(baseNode));
                    var j = jj[0, 0];
                    jj.ReturnToPool();

                    var val = baseNode == targetNode ? j : 0;

                    if (_direction == BeamDirection.Y)
                        val = -val;

                    lst.Add(new Condition(baseNode, Condition.FunctionType.M, xis(targetNode), 1, val));
                }
                else
                {
                    if (targetNode == 0 || targetNode == nodeCount - 1)
                    {
                        var jj = GetJMatrixAt(targetElement, xis(baseNode));
                        var j = jj[0, 0];
                        jj.ReturnToPool();

                        var val = baseNode == targetNode ? j : 0.5 * j;

                        if (_direction == BeamDirection.Y)
                            val = -val;

                        lst.Add(new Condition(baseNode, Condition.FunctionType.M, xis(targetNode), 2, 0));
                    }
                }
            }

            return lst;
        }

        [NonSerialized]
        private ListPool<Condition> CondsListPool = new ListPool<Condition>();

        private List<Condition> GetNCondition(Element targetElement, int baseNode, int targetNode,
            DofConstraint[] cv,
            DofConstraint[] cm)
        {
            //conditions for N_basenode(targetNode) and N_basenode`(targetNode)

            var nodeCount = cv.Length;

            var n = nodeCount;

            var xis = new Func<int, double>(i =>
            {
                var delta = 2.0 / (n - 1);

                return -1 + delta * i;
            });

            var lst = CondsListPool.Allocate();


            if (cv[baseNode] == DofConstraint.Fixed)
            {
                if (cv[targetNode] == DofConstraint.Fixed)
                {
                    var val = baseNode == targetNode ? 1.0 : 0.0;

                    lst.Add(new Condition(baseNode, Condition.FunctionType.N, xis(targetNode), 0, val));
                }
                else
                {
                    if (targetNode == 0 || targetNode == nodeCount - 1)
                    {
                        var val = baseNode == targetNode ? 1.0 : 0.0;

                        lst.Add(new Condition(baseNode, Condition.FunctionType.N, xis(targetNode), 3, val));
                    }
                }

                if (cm[targetNode] == DofConstraint.Fixed)
                {
                    lst.Add(new Condition(baseNode, Condition.FunctionType.N, xis(targetNode), 1, 0));
                }
                else
                {
                    if (targetNode == 0 || targetNode == nodeCount - 1)
                        lst.Add(new Condition(baseNode, Condition.FunctionType.N, xis(targetNode), 2, 0));
                }
            }

            return lst;
        }

        //private readonly string mssKey = "82069A88-26BD-4902-9CA6-7AE324193FE3:X";
        //private readonly string nssKey = "0EECCFF2-8CAE-4D65-935F-15E1AF31709B:X" ;

        public bool GetShapeFunctions(Element targetElement, out SingleVariablePolynomial[] nss, out SingleVariablePolynomial[] mss)
        {
            nss = null;
            mss = null;

            var mssKey = "7AE324193FE3:X"+this.Direction ;
            var nssKey = "15E1AF31709B:X" + this.Direction;


            var sb = new StringBuilder();

            var bar = targetElement as BarElement;

            var n = bar.NodeCount;


            List<Condition> cnds = null;

            {
                var xis = new Func<int, double>(i =>
                {
                    var delta = 2.0 / (n - 1);

                    return -1 + delta * i;
                });

                bar.TryGetCache(mssKey, out mss);
                bar.TryGetCache(nssKey, out nss);

                if (nss != null && mss != null)
                {
                    //return true;

                    cnds = GetShapeFunctionConditions(bar);

                    var flag = true;

                    foreach (var cnd in cnds)
                    {
                        var pn = (cnd.Type == Condition.FunctionType.N) ? nss[cnd.NodeNumber] : mss[cnd.NodeNumber];

                        var epsilon = (pn.EvaluateDerivative(cnd.Xi, cnd.DifferentialDegree) - cnd.RightSide);

                        if (epsilon < 0)
                            epsilon *= -1;

                        if (epsilon > 1e-10)
                        {
                            flag = false;
                            break;
                        }
                            
                    }

                    if (flag)
                    {
                        CondsListPool.Free(cnds);
                        return true;
                    }
                    else
                    {
                        mss = nss = null;
                    }
                }

                if (cnds == null)
                {
                    cnds = GetShapeFunctionConditions(bar);
                }
                    
            }

            
            var grpd = cnds.GroupBy(i => Tuple.Create(i.NodeNumber, i.Type)).ToArray();

            CondsListPool.Free(cnds);

            mss = new SingleVariablePolynomial[n];
            nss = new SingleVariablePolynomial[n];

            foreach (var grp in grpd)
            {
                var nodeNum = grp.Key.Item1;
                var tp = grp.Key.Item2;
                var condCount = grp.Count();

                var mtx =
                    bar.MatrixPool.Allocate(condCount, condCount);
                    //new Matrix(condCount, condCount);

                var rightSide =
                    bar.MatrixPool.Allocate(condCount, 1);
                    //new Matrix(condCount, 1);

                var arr = grp.ToArray();

                for (var i = 0; i < arr.Length; i++)
                {
                    var itm = arr[i];

                    mtx.SetRow(i, Diff(itm.Xi, condCount - 1, itm.DifferentialDegree));

                    rightSide.SetRow(i, itm.RightSide);
                }

                SingleVariablePolynomial pl;

                if (arr.Length != 0)
                {

                    //var cfs = mtx.Inverse2() * rightSide;
                    var cfs = mtx.Solve(rightSide.Values);//.Inverse2() * rightSide;

                    pl = new SingleVariablePolynomial(cfs);
                }
                else
                {
                    pl = new SingleVariablePolynomial();
                }

                //bar.ReturnMatrixToPool(rightSide, mtx);

                if (tp == Condition.FunctionType.M)
                    mss[nodeNum] = pl;

                if (tp == Condition.FunctionType.N)
                    nss[nodeNum] = pl;

                mtx.ReturnToPool();
                rightSide.ReturnToPool();
            }

            for (var i = 0; i < n; i++)
            {
                if (nss[i] == null)
                    nss[i] = new SingleVariablePolynomial();

                if (mss[i] == null)
                    mss[i] = new SingleVariablePolynomial();
            }

            {
                bar.SetCache(nssKey, nss);
                bar.SetCache(mssKey, mss);
            }


            return true;
        }


        internal List<Condition> GetShapeFunctionConditions(BarElement targetElement)
        {
            
            var cnds = CondsListPool.Allocate();

            var bar = targetElement as BarElement;

            //var l = (bar.StartNode.Location - bar.EndNode.Location).Length;

            var n = bar.NodeCount;

            /*
            var xis = new Func<int, double>(i =>
            {
                var delta = 2.0 / (n - 1);

                return -1 + delta * i;
            });
            */

            var cv = new DofConstraint[n];//shear constraint
            var cm = new DofConstraint[n];//moment constraints

            for (var i = 0; i < n; i++)
            {
                if (this._direction == BeamDirection.Z)
                {
                    cv[i] = bar._nodalReleaseConditions[i].DY;
                    cm[i] = bar._nodalReleaseConditions[i].RZ;
                }
                else
                {
                    cv[i] = bar._nodalReleaseConditions[i].DZ;
                    cm[i] = bar._nodalReleaseConditions[i].RY;
                }
            }

            #region detect conditions

            for (var bnode = 0; bnode < n; bnode++)
            {
                for (var tnode = 0; tnode < n; tnode++)
                {
                    var l1 = GetMCondition(targetElement, bnode, tnode, cv, cm);
                    cnds.AddRange(l1);
                    CondsListPool.Free(l1);

                    var l2 = GetNCondition(targetElement, bnode, tnode, cv, cm);
                    cnds.AddRange(l2);
                    CondsListPool.Free(l2);
                }
            }

            #endregion

            //cnds.Sort(new Condition.ConditionEqualityComparer());

            return cnds;
        }

        /// <summary>
        /// Gets the coeficient ??!
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="pOrder"></param>
        /// <param name="diffDegree"></param>
        /// <returns></returns>
        public double[] Diff(double xi, int pOrder, int diffDegree)
        {
            var buf = new double[pOrder + 1];

            for (var i = 0; i < buf.Length; i++)
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
        public double NthDer(double x, int n, int m)
        {
            var pval = Math.Pow(x, n - m);

            if (m > n) return 0;

            return Factorial(n) / Factorial(n - m) * pval;
        }

        long Factorial(int x)
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
                var buf = 1L;

                for (var i = 1; i <= x; i++)
                    buf = buf * i;

                return buf;
                //return x * Factorial(x - 1);
            }
        }
        #endregion


        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //note: this method gives the shape function on variable node count beam with variable constraint on each node 

            SingleVariablePolynomial[] nss = null;
            SingleVariablePolynomial[] mss = null;

            var bar = targetElement as BarElement;

            if (!GetShapeFunctions(targetElement, out nss, out mss))
                throw new Exception();

            var n = bar.NodeCount;

            var xi = isoCoords[0];

            var buf =
                targetElement.MatrixPool.Allocate(4, 2 * n);


            for (var i = 0; i < n; i++)
            {
                if (nss[i] == null) nss[i] = new SingleVariablePolynomial();
                if (mss[i] == null) mss[i] = new SingleVariablePolynomial();
            }

            for (var i = 0; i < n; i++)
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

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var buf =
                targetElement.MatrixPool.Allocate(1, 1);

            var x_xi = bar.GetIsoToLocalConverter();

            //we need J = ∂X / ∂ξ

            buf[0, 0] = x_xi.EvaluateDerivative(isoCoords[0], 1);
            
            return buf;

        }

        public double[] Iso2Local(Element targetElement, params double[] isoCoords)
        {
            //throw new NotImplementedException();
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
            //throw new NotImplementedException();
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
        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
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

        /// <inheritdoc/>
        public int[] GetBMaxOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;
            var t = (2*n - 1) - 2;
            return new[] {t,0,0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;
            return new int[] {n-1-1, 0, 0};
        }


        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load,
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
                if (load is UniformLoad )
                {
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
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
                    var uld = (load as PartialNonUniformLoad);

                    magnitude = (xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi)));
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

                                if (this._direction == BeamDirection.Y)
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


                    if (this._direction == BeamDirection.Y)
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

                    if (_direction == BeamDirection.Y)
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


                if(_direction == BeamDirection.Y)
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


        /// <inheritdoc/>
        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            var n = this.GetNMatrixAt(targetElement, isoLocation);
            
            throw new NotImplementedException();
        }

        
        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
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

            if (_direction == BeamDirection.Y)
                u.SetColumn(0, ld[0].DZ, ld[0].RY, ld[1].DZ, ld[1].RY);
            else
                u.SetColumn(0, ld[0].DY, ld[0].RZ, ld[1].DY, ld[1].RZ);

            var f = n * u;

            //var ei = d[0, 0];

            f.MultiplyRowByConstant(1, 1 / j);
            
            var buf = new Displacement();

            if (_direction == BeamDirection.Y)
            {
                buf.DZ = f[0, 0];
                buf.RY = f[1, 0];
            }
            else
            {
                buf.DY = f[0, 0];
                buf.RZ = f[1, 0];
            }
            
            return buf;
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var nc = targetElement.Nodes.Length;

            var ld = localDisplacements;

            Matrix B = targetElement.MatrixPool.Allocate(2, 4);

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException("isoCoords");

            var buf = new List<Tuple<DoF, double>>();

            if (_direction == BeamDirection.Y)
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

            if (_direction == BeamDirection.Y)
            {
                for (int i = 0; i < nc; i++)
                {
                    u[2*i + 0, 0] = ld[i].DZ;
                    u[2*i + 1, 0] = ld[i].RY;
                }
            }
            else
            {
                for (int i = 0; i < nc; i++)
                {
                    u[ 2 * i + 0,0] = ld[i].DY;
                    u[ 2 * i + 1,0] = ld[i].RZ;
                }
            }

            var f = n * u;

            var ei = d[0, 0];

            f.MultiplyRowByConstant(1, 1 / j);
            f.MultiplyRowByConstant(2, ei / (j * j));
            f.MultiplyRowByConstant(3, ei / (j * j * j));

            if (_direction == BeamDirection.Y)
                f.MultiplyRowByConstant(2, -1);//m/ei = - n''*u , due to where? TODO

            if (_direction == BeamDirection.Y)
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


        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            var bar = targetElement as BarElement;
            var n = bar.Nodes.Length;


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
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
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
                    var uld = (load as PartialNonUniformLoad);

                    magnitude = (xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi)));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];// Coefficients.Length;
                }
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

                ns.MultiplyRowByConstant(1, 1 / detJ);
                ns.MultiplyRowByConstant(2, 1 / (detJ * detJ));
                ns.MultiplyRowByConstant(3, 1 / (detJ * detJ * detJ));


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

                        fi.Fy += localforce.Mz * nip;//concentrated moment
                        fi.Mz += localforce.Mz * mip;//concentrated moment
                    }
                    else
                    {
                        fi.Fz += localforce.Fz * ni;//concentrated force
                        fi.My += localforce.Fz * mi;//concentrated force

                        fi.Fz += localforce.My * nip;//concentrated moment
                        fi.My += localforce.My * -mip;//concentrated moment
                    }

                    buf[i] = fi;
                }

                return buf;
            }




            #endregion

            

            throw new NotImplementedException();
        }

        public void AddStiffnessComponents(CoordinateStorage<double> globalStiffness)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
