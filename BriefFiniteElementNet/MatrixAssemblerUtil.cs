using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using CSparse;
using CSparse.Double;
using CSparse.Storage;
using CCS = CSparse.Double.CompressedColumnStorage;

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

            var elements = model.Elements.ToArray();

            var maxNodePerElement = elements.Any() ? elements.Select(i => i.Nodes.Length).Max() : 1;
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
                var d = c2*6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        kt.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }
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
        /// Extracts the free free part.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="nodeMapping">The node mapping.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
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
        public static ZoneDevidedMatrix DivideZones(Model model,CCS matrix, DofMappingManager dofMap)
        {
            //see Calcutil.GetReducedZoneDividedMatrix
            throw new NotImplementedException();
        }
    }
}