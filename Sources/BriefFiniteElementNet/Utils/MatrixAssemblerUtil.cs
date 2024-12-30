using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using CSparse;
using CSparse.Double;
using CSparse.Storage;
using CCS = CSparse.Double.SparseMatrix;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents utilities for assembling Mass, Damp and stiffness matrix
    /// </summary>
    public static class MatrixAssemblerUtil
    {
        /// <summary>
        /// Assembles the stiffness matrix of defined model and return it back.
        /// </summary>
        /// <returns>Assembled stiffness matrix</returns>
        public static CCS AssembleFullStiffnessMatrix(Model model)
        {
            model.ReIndexNodes();

            var elements = model.Elements;

            var maxNodePerElement = model.Elements.Any() ? model.Elements.Select(i => i.Nodes.Length).Max() : 1;
            var rElmMap = new int[maxNodePerElement*6];

            var c = model.Nodes.Count*6;

            var kt = new CoordinateStorage<double>(c, c, c);

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6*i + 0] = elm.Nodes[i].Index*6 + 0;
                    rElmMap[6*i + 1] = elm.Nodes[i].Index*6 + 1;
                    rElmMap[6*i + 2] = elm.Nodes[i].Index*6 + 2;

                    rElmMap[6*i + 3] = elm.Nodes[i].Index*6 + 3;
                    rElmMap[6*i + 4] = elm.Nodes[i].Index*6 + 4;
                    rElmMap[6*i + 5] = elm.Nodes[i].Index*6 + 5;
                }

                var mtx = elm.GetGlobalStifnessMatrix();
                var d = c2 * 6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        kt.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }

                mtx.ReturnToPool();
            }

            var stiffness = (CCS)Converter.ToCompressedColumnStorage(kt, true);

            return stiffness;
        }

        /// <summary>
        /// Assembles the mass matrix of defined model and return it back.
        /// </summary>
        /// <returns>Assembled stiffness matrix</returns>
        public static CCS AssembleFullMassMatrix(Model model)
        {
            var elements = model.Elements.ToArray();

            var maxNodePerElement = elements.Select(i => i.Nodes.Length).Max();
            var rElmMap = new int[maxNodePerElement*6];

            var c = model.Nodes.Count*6;

            var mt = new CoordinateStorage<double>(c, c, c);

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6*i + 0] = elm.Nodes[i].Index*6 + 0;
                    rElmMap[6*i + 1] = elm.Nodes[i].Index*6 + 1;
                    rElmMap[6*i + 2] = elm.Nodes[i].Index*6 + 2;

                    rElmMap[6*i + 3] = elm.Nodes[i].Index*6 + 3;
                    rElmMap[6*i + 4] = elm.Nodes[i].Index*6 + 4;
                    rElmMap[6*i + 5] = elm.Nodes[i].Index*6 + 5;
                }

                var mtx = elm.GetGlobalMassMatrix();
                var d = c2*6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        mt.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }
            }

            var mass = (CCS)Converter.ToCompressedColumnStorage(mt, true);

            return mass;
        }

        /// <summary>
        /// Assembles the damping matrix of defined model and return it back.
        /// </summary>
        /// <returns>Assembled stiffness matrix</returns>
        public static CCS AssembleFullDampingMatrix(Model model)
        {
            var elements = model.Elements.ToArray();

            var maxNodePerElement = elements.Select(i => i.Nodes.Length).Max();
            var rElmMap = new int[maxNodePerElement*6];

            var c = model.Nodes.Count*6;

            var ct = new CoordinateStorage<double>(c, c, c);

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6*i + 0] = elm.Nodes[i].Index*6 + 0;
                    rElmMap[6*i + 1] = elm.Nodes[i].Index*6 + 1;
                    rElmMap[6*i + 2] = elm.Nodes[i].Index*6 + 2;

                    rElmMap[6*i + 3] = elm.Nodes[i].Index*6 + 3;
                    rElmMap[6*i + 4] = elm.Nodes[i].Index*6 + 4;
                    rElmMap[6*i + 5] = elm.Nodes[i].Index*6 + 5;
                }

                var mtx = elm.GetGlobalDampingMatrix();
                var d = c2*6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        ct.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }
            }

            var damp = (CCS)Converter.ToCompressedColumnStorage(ct, true);

            return damp;
        }



        /// <summary>
        /// Creates the full displacement vector, consist of 'settlements' only
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cse"></param>
        /// <param name="d"></param>
        public static void AssembleFullDisplacementVector(Model parent, LoadCase cse,double[] d)
        {
            //displacement vector for both free and fixed dof
            //adds it to the f
            //only settlements, independent of being fixed or not

            var n = parent.Nodes.Count;
            var buf = d;// new double[6 * n];

            //var loads = new Displacement[n];//loads from connected element to node is stored in this array instead of Node.ElementLoads.

            for (int i = 0; i < n; i++)//re indexing
            {
                parent.Nodes[i].Index = i;
            }

            Displacement sum;

            for (var i = 0; i < n; i++)
            {
                sum = Displacement.Zero;

                //calculate sum
                foreach (var load in parent.Nodes[i].Settlements)
                {
                    if (load.LoadCase != cse)
                        continue;

                    var disp = load.Displacement;

                    sum += disp;
                    //loads[parent.Nodes[i].Index] += disp;
                }

                {//adding to buf
                    buf[6 * i + 0] = sum.DX;
                    buf[6 * i + 1] = sum.DY;
                    buf[6 * i + 2] = sum.DZ;

                    buf[6 * i + 3] = sum.RX;
                    buf[6 * i + 4] = sum.RY;
                    buf[6 * i + 5] = sum.RZ;
                }
            }

        }



        /// <summary>
        /// Creates the full force vector, consist of 'equivalent nodal loads' and 'nodal loads'
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lc"></param>
        /// <returns></returns>
        public static void AssembleFullForceVector(Model model, LoadCase lc,double[] d)
        {
            var n = model.Nodes.Count;
            var buf = d;

            GetNodalLoads(model, lc, buf);
            GetEquivalentNodalLoads(model, lc, buf);
        }


        /// <summary>
        /// Add the total concentrated forces on loads vector to the f
        /// </summary>
        /// <param name="cse">The load case.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        private static void GetNodalLoads(Model parent, LoadCase cse, double[] f)
        {
            //force vector for both free and fixed dof
            //adds it to the f

            var n = parent.Nodes.Count;

            //var loads = new Force[n];//loads from connected element to node is stored in this array instead of Node.ElementLoads.

            #region adding concentrated nodal loads

            Force sum;

            for (int i = 0; i < n; i++)
            {
                sum = Force.Zero;


                foreach (var load in parent.Nodes[i].Loads)
                {
                    if (load.Case != cse)
                        continue;

                    sum += load.Force;
                }

                {
                    var force = sum;
                    var buf = f;

                    buf[6 * i + 0] = force.Fx;
                    buf[6 * i + 1] = force.Fy;
                    buf[6 * i + 2] = force.Fz;

                    buf[6 * i + 3] = force.Mx;
                    buf[6 * i + 4] = force.My;
                    buf[6 * i + 5] = force.Mz;
                }
            }

            #endregion
            
        }

        private static void GetEquivalentNodalLoads(Model parent, LoadCase cse, double[] f)
        {
            //force vector for both free and fixed dof
            //adds it to the f

            var n = parent.Nodes.Count;

            var loads = new Force[n];//loads from connected element to node is stored in this array instead of Node.ElementLoads.

            for (int i = 0; i < n; i++)//re indexing
            {
                parent.Nodes[i].Index = i;
            }

            #region adding element loads

            //Force sum;

            foreach (var elm in parent.Elements)
            {
                var nc = elm.Nodes.Length;

                foreach (var ld in elm.Loads)
                {
                    if (ld.Case != cse)
                        continue;

                    var frcs =
                        elm.GetGlobalEquivalentNodalLoads(ld);
                    //ld.GetGlobalEquivalentNodalLoads(elm);

                    for (var i = 0; i < nc; i++)
                    {
                        var nde = elm.Nodes[i];

                        loads[nde.Index] += frcs[i];
                    }
                }
            }

            #endregion

        }





        /// <summary>
        /// Extracts the free free part.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="nodeMapping">The node mapping.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [Obsolete]
        public static CCS ExtractFreeFreePart(CCS matrix, int[] nodeMapping)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Divides the zones of reduced <see cref="matrix"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="matrix">The reduced matrix.</param>
        /// <param name="dofMap">The DoF map.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [Obsolete]
        public static ZoneDevidedMatrix DivideZones(Model model,CCS matrix, DofMappingManager dofMap)
        {
            //see Calcutil.GetReducedZoneDividedMatrix
            throw new NotImplementedException();
        }
    }
}