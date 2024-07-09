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
    public class Concentrated_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is Bar.TimoshenkoBeamHelper))
                return false;

            if (!(load is Loads.ConcentratedLoad))
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

            #region ConcentratedLoad

            {
                var cl = load as ConcentratedLoad;

                var localforce = cl.Force;

                if (cl.CoordinationSystem == CoordinationSystem.Global)
                    localforce = tr.TransformGlobalToLocal(localforce);

                var buf = new Force[n];

                var ns = hlpr.GetNMatrixAt(elm, cl.ForceIsoLocation.Xi);

                /*
                if (_direction == BeamDirection.Y)
                    for (var i = 0; i < ns.ColumnCount; i++)
                        if (i % 2 == 1)
                            ns.MultiplyColumnByConstant(i, -1);
                */


                var j = hlpr.GetJMatrixAt(elm, cl.ForceIsoLocation.Xi);

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

                    if (bhlpr.Direction == BeamDirection.Z)
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

            #region concentrated

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

                if (frc != Force.Zero)
                {
                    frc = frc.Move(new Point(frcX, 0, 0), new Point(0, 0, 0));
                    frc = frc.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));
                }

                var movedEnds = ends.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var f2 = frc + movedEnds;
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

            #endregion
        }
    }
}
