using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using CSparse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.ShaftHelper
{
    public class Concentrated_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.ShaftHelper2Node))
                return false;

            if (!(load is ConcentratedLoad))
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

            var tr = bar.GetTransformationManager();

            var cns = load as ConcentratedLoad;

            var shapes = hlpr.GetNMatrixAt(bar, cns.ForceIsoLocation.Xi);

            var localForce = cns.Force;

            if (cns.CoordinationSystem == CoordinationSystem.Global)
                localForce = tr.TransformGlobalToLocal(localForce);


            shapes.Scale(localForce.Mx);

            var fxs = shapes.Row(0);

            var n = bar.Nodes.Length;

            var buf = new Force[n];

            for (var i = 0; i < n; i++)
                buf[i] = new Force(0, 0, 0, fxs[i], 0, 0);

            return buf;

        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad ld, IsoPoint loc)
        {
            double L;
            double tt;//tprsion concentrated
            double t0;//inverse of eq nodal load, mx at start node
            double xt;//applied location


            var xi = loc.Xi;

            var bar = elm as BarElement;
            var load = ld as ConcentratedLoad;

            if (bar.NodeCount != 2)
                throw new Exception();

            {//step 0
                L = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

                xt = bar.IsoCoordsToLocalCoords(load.ForceIsoLocation.Xi)[0];
            }

            #region step 1

            {
                var p0 = GetLocalEquivalentNodalLoads(bar,hlpr, load)[0];

                p0 = -p0;

                t0 = p0.Mx;
            }

            {
                var localForce = load.Force;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);

                tt = localForce.Mx;
            }

            #endregion

            {
                var eiOrder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

                if (eiOrder != 0) 
                    throw new BriefFiniteElementNetException("Nonuniform EI");
            }

            {
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi, bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var g = mat.Ex / (2 * (1 + mat.NuYz));

                var j = sec.J;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                var d = 0.0;//TODO

                if (x <= xt)
                    d = -t0 * x / (g * j);
                else
                    d = -t0 * xt / (g * j) + (t0 + tt) * (x - xt) / (g * j);

                var buf = new Displacement();

                buf.RX = d;

                return buf;
            }
        }


        public CauchyStressTensor GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {

            var buff = new List<Tuple<DoF, double>>();

            //var buf = new FlatShellStressTensor();

            var bar = elm as BarElement;

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
                    if (xi_s[i] < loc.Xi)
                    {
                        var frc_i = endForces[i];// new Force();
                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }

                }
            }



            {
                var cns = load as ConcentratedLoad;

                var xi = loc.Xi;// isoLocation[0];
                var targetX = br.IsoCoordsToLocalCoords(xi)[0];

                var frc = Force.Zero;

                if (cns.ForceIsoLocation.Xi < xi)
                    frc = cns.Force;

                if (cns.CoordinationSystem == CoordinationSystem.Global)
                    frc = tr.TransformGlobalToLocal(frc);


                var frcX = br.IsoCoordsToLocalCoords(cns.ForceIsoLocation.Xi)[0];

                frc = frc.Move(new Point(frcX, 0, 0), new Point(0, 0, 0));

                if (br.StartReleaseCondition.RX == DofConstraint.Released)
                    frc.Mx = 0;

                frc = frc.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var movedEnds = ends.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var f2 = frc + movedEnds;
                f2 *= -1;

                buff.Add(Tuple.Create(DoF.Rx, f2.Mx));


                var tns = new CauchyStressTensor();

                //return buff;
                //TODO: convert to tensor
                throw new NotImplementedException();
                
            }

            #endregion

        }
    }
}
