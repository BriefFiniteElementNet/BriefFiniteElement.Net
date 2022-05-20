using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for mapping DOFs
    /// </summary>
    [Obsolete("Not used anywhere")]
    class DofMap
    {
        /// <summary>
        /// The free map
        /// </summary>
        /// <remarks>
        /// Usage note:
        ///     ReleasedMap[global DoF index] = DoF index in free DoFs.
        ///     if(ReleasedMap[global DoF index] == -1) then it is not a free Dof
        /// Length = n, where n equals to total number of DoFs in model
        /// </remarks>
        public int[] ReleasedMap;

        /// <summary>
        /// The fixed map
        /// </summary>
        /// <remarks>
        /// Usage note:
        ///     FixedMap[global DoF index] = DoF index in fixed DoFs.
        ///     if(FixedMap[global DoF index] == -1) then it is not a fixed Dof
        /// Length = n, where n equals to total number of DoFs in model
        /// </remarks>
        public int[] FixedMap;

        /// <summary>
        /// The reversed free map
        /// </summary>
        /// <remarks>
        /// Usage note:
        ///     ReversedReleasedMap[DoF index in free DoFs] = global DoF index.
        /// Length = 
        /// </remarks>
        public int[] ReversedReleasedMap;

        /// <summary>
        /// The reversed fixed map
        /// </summary>
        /// <remarks>
        /// Usage note:
        ///     ReversedFixedMap[DoF index in fixed DoFs] = global DoF index.
        /// </remarks>
        public int[] ReversedFixedMap;

        /// <summary>
        /// The DoF fixity.
        /// <remarks>
        /// Length = n, where n equals to total number of DoFs in model
        /// </remarks>
        /// </summary>
        public bool[] DofFixity;


        /// <summary>
        /// The target model
        /// </summary>
        public Model TargetModel;


        /// <summary>
        /// Creates a new <see cref="DofMap"/> for specified <see cref="model"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Dof mapping of model</returns>
        public static DofMap Create(Model model)
        {
            var buf = new DofMap();
            buf.TargetModel = model;

            var n = model.Nodes.Count;


            for (var i = 0; i < n; i++)
            {
                var cns = model.Nodes[i].Constraints;

                if (cns.DX == DofConstraint.Fixed) buf.DofFixity[6*i + 0] = true;
                if (cns.DY == DofConstraint.Fixed) buf.DofFixity[6*i + 1] = true;
                if (cns.DZ == DofConstraint.Fixed) buf.DofFixity[6*i + 2] = true;


                if (cns.RX == DofConstraint.Fixed) buf.DofFixity[6*i + 3] = true;
                if (cns.RY == DofConstraint.Fixed) buf.DofFixity[6*i + 4] = true;
                if (cns.RZ == DofConstraint.Fixed) buf.DofFixity[6*i + 5] = true;
            }

            int fCnt = 0, rCnt = 0;

            var fixedDofCount =
                model.Nodes.Select(
                    i =>
                        (int) i.Constraints.DX + (int) i.Constraints.DY + (int) i.Constraints.DZ +
                        (int) i.Constraints.RX + (int) i.Constraints.RY + (int) i.Constraints.RZ).Sum();

            var c = 6*n;

            var freeDofCount = c - fixedDofCount;

            buf.ReleasedMap = new int[c];
            buf.FixedMap = new int[c];
            buf.ReversedReleasedMap = new int[freeDofCount];
            buf.ReversedFixedMap = new int[fixedDofCount];


            for (var i = 0; i < c; i++)
            {
                buf.ReleasedMap[i] = buf.FixedMap[i] = -1;
            }

            for (var i = 0; i < c; i++)
            {
                if (buf.DofFixity[i])
                    buf.ReversedFixedMap[buf.FixedMap[i] = fCnt++] = i;
                else
                    buf.ReversedReleasedMap[buf.ReleasedMap[i] = rCnt++] = i;
            }


            return buf;
        }
    }
}