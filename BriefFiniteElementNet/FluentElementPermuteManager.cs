using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;




namespace BriefFiniteElementNet
{
    public class FluentElementPermuteManager
    {
        public static Matrix FullyExpand(Matrix originalMatrix, IEnumerable<ElementLocalDof>  src, int totalNodes)
        {
            var source = src.ToArray();

            var dic = new Dictionary<int, ElementLocalDof[]>();

            source.GroupBy(i => i.NodeIndex).ToList().ForEach(i => dic[i.Key] = i.OrderBy(j => j.Dof).ToArray());

            var permArr = new List<int>();

            for (int i = 0; i < source.Length; i++)
            {
                var newLoc = source[i].NodeIndex * 6 + (int)source[i].Dof;

                permArr.Add(newLoc);
            }

            var permute = SimplePermuteManager.Make(i => permArr[i], source.Length, totalNodes * 6);

            var buf = permute.ApplyTo(originalMatrix);

            return buf;
        }

        [DebuggerDisplay("{Dof} - {NodeIndex}")]
        public struct ElementLocalDof
        {
            public int NodeIndex;
            public DoF Dof;

            public ElementLocalDof(int nodeIndex, DoF dof)
            {
                NodeIndex = nodeIndex;
                Dof = dof;
            }

            public static  List<ElementLocalDof> CreateForNodes(params int[] nodes)
            {
                var buf = new List<ElementLocalDof>();

                foreach (var index in nodes)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        buf.Add(new ElementLocalDof(index, (DoF)i));
                    }
                }

                return buf;
            }

            public static ElementLocalDof[] CreateForDofs(params int[] dofs)
            {
                var buf = new List<ElementLocalDof>();

                foreach (var dof in dofs)
                {
                    var dofNom = dof%6;
                    var nodeNum = dof/6;

                    buf.Add(new ElementLocalDof(nodeNum, (DoF) dofNom));
                }

                return buf.ToArray();
            }

            public bool Equals(ElementLocalDof other)
            {
                return NodeIndex == other.NodeIndex && Dof == other.Dof;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ElementLocalDof && Equals((ElementLocalDof) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (NodeIndex*397) ^ (int) Dof;
                }
            }

            public static bool operator ==(ElementLocalDof left, ElementLocalDof right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ElementLocalDof left, ElementLocalDof right)
            {
                return !left.Equals(right);
            }
        }
    }
}
