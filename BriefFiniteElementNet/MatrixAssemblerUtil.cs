using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;
using BriefFiniteElementNet.CSparse.Storage;

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
        public static CompressedColumnStorage<double> AssembleFullStiffnessMatrix(Model model)
        {
            var elements = model.Elements.ToArray();

            var maxNodePerElement = elements.Select(i => i.Nodes.Length).Max();
            var rElmMap = new int[maxNodePerElement * 6];

            var c = model.Nodes.Count*6;

            var kt = new CoordinateStorage<double>(c, c, c);

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6 * i + 0] = elm.Nodes[i].Index * 6 + 0;
                    rElmMap[6 * i + 1] = elm.Nodes[i].Index * 6 + 1;
                    rElmMap[6 * i + 2] = elm.Nodes[i].Index * 6 + 2;

                    rElmMap[6 * i + 3] = elm.Nodes[i].Index * 6 + 3;
                    rElmMap[6 * i + 4] = elm.Nodes[i].Index * 6 + 4;
                    rElmMap[6 * i + 5] = elm.Nodes[i].Index * 6 + 5;
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
            }

            var stiffness = Converter.ToCompressedColumnStorage(kt, true);

            return stiffness;
        }


        /// <summary>
        /// Assembles the mass matrix of defined model and return it back.
        /// </summary>
        /// <returns>Assembled stiffness matrix</returns>
        public static CompressedColumnStorage<double> AssembleFullMassMatrix(Model model)
        {
            var elements = model.Elements.ToArray();

            var maxNodePerElement = elements.Select(i => i.Nodes.Length).Max();
            var rElmMap = new int[maxNodePerElement * 6];

            var c = model.Nodes.Count * 6;

            var mt = new CoordinateStorage<double>(c, c, c);

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6 * i + 0] = elm.Nodes[i].Index * 6 + 0;
                    rElmMap[6 * i + 1] = elm.Nodes[i].Index * 6 + 1;
                    rElmMap[6 * i + 2] = elm.Nodes[i].Index * 6 + 2;

                    rElmMap[6 * i + 3] = elm.Nodes[i].Index * 6 + 3;
                    rElmMap[6 * i + 4] = elm.Nodes[i].Index * 6 + 4;
                    rElmMap[6 * i + 5] = elm.Nodes[i].Index * 6 + 5;
                }

                var mtx = elm.GetGlobalMassMatrix();
                var d = c2 * 6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        mt.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }
            }

            var mass = Converter.ToCompressedColumnStorage(mt, true);

            return mass;
        }


        /// <summary>
        /// Assembles the damping matrix of defined model and return it back.
        /// </summary>
        /// <returns>Assembled stiffness matrix</returns>
        public static CompressedColumnStorage<double> AssembleFullDampingMatrix(Model model)
        {
            var elements = model.Elements.ToArray();

            var maxNodePerElement = elements.Select(i => i.Nodes.Length).Max();
            var rElmMap = new int[maxNodePerElement * 6];

            var c = model.Nodes.Count * 6;

            var ct = new CoordinateStorage<double>(c, c, c);

            foreach (var elm in elements)
            {
                var c2 = elm.Nodes.Length;

                for (var i = 0; i < c2; i++)
                {
                    rElmMap[6 * i + 0] = elm.Nodes[i].Index * 6 + 0;
                    rElmMap[6 * i + 1] = elm.Nodes[i].Index * 6 + 1;
                    rElmMap[6 * i + 2] = elm.Nodes[i].Index * 6 + 2;

                    rElmMap[6 * i + 3] = elm.Nodes[i].Index * 6 + 3;
                    rElmMap[6 * i + 4] = elm.Nodes[i].Index * 6 + 4;
                    rElmMap[6 * i + 5] = elm.Nodes[i].Index * 6 + 5;
                }

                var mtx = elm.GetGlobalMassMatrix();
                var d = c2 * 6;

                for (var i = 0; i < d; i++)
                {
                    for (var j = 0; j < d; j++)
                    {
                        ct.At(rElmMap[i], rElmMap[j], mtx[i, j]);
                    }
                }
            }

            var damp = Converter.ToCompressedColumnStorage(ct, true);

            return damp;
        }


    }
}
