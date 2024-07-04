using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node
{
    public class Uniform_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node))
                return false;

            if (!(load is Loads.UniformLoad))
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

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad ld)
        {
            //where load is uniform and section or material is also uniform

            var load = ld as Loads.UniformLoad;
            int c;
            double L;
            double w0;

            var bar = elm as BarElement;
            var thiss = hlpr as BarHelpers.EulerBernoulliBeamHelper2Node;

            {//finding L
                L = bar.GetLength();
            }

            {//finding w0
                var localDir = load.Direction;

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = bar.GetTransformationManager().TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (thiss.Direction)
                {
                    case BarHelpers.BeamDirection.Y:
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BarHelpers.BeamDirection.Z:
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

            if (thiss.Direction == BarHelpers.BeamDirection.Y)
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

            throw new NotImplementedException();
        }

        public StrainTensor GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad ld, IsoPoint loc)
        {
            //https://byjusexamprep.com/gate-ce/fixed-beams
            double w0;
            double L;
            double f0, m0;

            var xi = loc.Xi;

            var load = ld as Loads.UniformLoad;
            var thiss = hlpr as BarHelpers.EulerBernoulliBeamHelper2Node;
            var bar = elm as BarElement;

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

                switch (thiss.Direction)
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
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi, bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var e = mat.Ex;
                var I = thiss.Direction == BeamDirection.Y ? sec.Iy : sec.Iz;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                //https://gs-post-images.grdp.co/2022/8/fixed-beam-deflection-img1661861640198-75-rs.PNG?noResize=1
                var d = w0 * x * x / (24.0 * e * I) * (L - x) * (L - x);

                var buf = new Displacement();

                if (thiss.Direction == BeamDirection.Y)
                    buf.DZ = d;
                else
                    buf.DY = d;

                //todo: convert to tensor
                throw new NotImplementedException();
            }
        }

        public CauchyStressTensor GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad ld, IsoPoint loc)
        {
            double f0, m0, w0;
            double L;

            var thiss = hlpr as BarHelpers.EulerBernoulliBeamHelper2Node;
            var load = ld as Loads.UniformLoad;
            var bar = elm as BarElement;
            var xi = loc.Xi;


            if (bar.NodeCount != 2)
                throw new Exception();

            {//the L
                L = bar.GetLength();
            }

            {//find f0,m0,w0, inverse of equivalent nodal loads applied to start node

                var p0 = GetLocalEquivalentNodalLoads(bar, hlpr, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (thiss.Direction)
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
                var hp = hlpr as BarHelpers.EulerBernoulliBeamHelper2Node;

                var x = hp.Iso2Local(bar, xi)[0];

                m_x = w0 * x * x / 2 + f0 * x + m0;
                v_x = f0;
            }

            Force buf;

            {//substitude M and V to buf
                buf = new Force();

                if (thiss.Direction == BeamDirection.Y)
                {
                    buf.My = m_x;
                    buf.Fz = v_x;
                }

                if (thiss.Direction == BeamDirection.Z)
                {
                    buf.Mz = m_x;
                    buf.Fy = v_x;
                }
            }

            //todo: convert to tensor
            throw new Exception();
            //return buf;
        }
    }
}
