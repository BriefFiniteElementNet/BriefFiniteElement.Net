using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node
{
    public class Concentrated_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node))
                return false;

            if (!(load is Loads.ConcentratedLoad ))
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

            var thiss = hlpr as ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node;

            var tDeg = 0;

            if (bar.Section != null)
                tDeg += bar.Section.GetMaxFunctionOrder()[0];

            if (bar.Section != null)
                tDeg += bar.Material.GetMaxFunctionOrder()[0];

            var tr = bar.GetTransformationManager();

            {
                var cl = load as ConcentratedLoad;

                var localforce = cl.Force;

                if (cl.CoordinationSystem == CoordinationSystem.Global)
                    localforce = tr.TransformGlobalToLocal(localforce);

                var buf = new Force[n];

                var ns = thiss.GetNMatrixAt(bar, cl.ForceIsoLocation.Xi);

                var j = thiss.GetJMatrixAt(bar, cl.ForceIsoLocation.Xi);

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

                    if (thiss.Direction == BeamDirection.Z)
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
        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad ld, IsoPoint loc)
        {
            var bar = elm as BarElement;
            var thiss = hlpr as ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node;
            var load = ld as ConcentratedLoad;
            var xi = loc.Xi;

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
                var p0 = GetLocalEquivalentNodalLoads(bar, hlpr, load)[0];

                p0 = -p0;

                var localForce = load.Force;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);

                switch (thiss.Direction)
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
                var I = thiss.Direction == BeamDirection.Y ? sec.Iy : sec.Iz;
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

                if (thiss.Direction == BeamDirection.Y)
                    buf.DZ = d;
                else
                    buf.DY = d;

                return buf;
            }
        }

        public object GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            var thiss = hlpr as ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node;

            var bar = elm as BarElement;

            var targetElement = bar;

            var n = targetElement.Nodes.Length;

            var buff = new List<Tuple<DoF, double>>();

            var tr = targetElement.GetTransformationManager();

            var br = targetElement as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(targetElement, hlpr, load);

            for (var i = 0; i < n; i++)
                endForces[i] = -endForces[i];//(2,1) section

            var xi = loc.Xi;

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
                    if (xi_s[i] < xi)
                    {
                        var frc_i = endForces[i];// new Force();

                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }
                }
            }

            #endregion

            var to = thiss.Iso2Local(targetElement, xi)[0];

            #region concentrated

            {
                var cns = load as ConcentratedLoad;

                //var xi = isoLocation[0];
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


                Force buf;

                if (thiss.Direction == BeamDirection.Y)
                {
                    buf = new Force(0, 0, f2.Fz, 0, f2.My, 0);

                    //buff.Add(Tuple.Create(DoF.Ry, f2.My));
                    //buff.Add(Tuple.Create(DoF.Dz, f2.Fz));
                }
                else
                {
                    buf = new Force(0, f2.Fy, 0, 0, 0, f2.Mz);

                    //buff.Add(Tuple.Create(DoF.Rz, f2.Mz));
                    //buff.Add(Tuple.Create(DoF.Dy, f2.Fy));
                }

                return buf;
            }

            #endregion

            
        }
    }
}
