using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CSparse.Double;
using System.Security.Permissions;
using CSparse.Storage;
using BriefFiniteElementNet.Utils;

namespace BriefFiniteElementNet.MpcElements
{
    /// <summary>
    /// Represents a hinge link between two nodes.
    /// For more info see HingeLink.md
    /// </summary>
    [Serializable]
    [Obsolete("use spring1d at the moment")]
    public class HingeLink: MpcElement
    {
        protected HingeLink(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HingeLink():base()
        {
        }

        public override int GetExtraEquations(CoordinateStorage<double> crd, int startLine)
        {
            var distLocation = Nodes.Select(i => i.Location).Distinct();

            if (distLocation.Count() != 1)
                throw new BriefFiniteElementNetException("Nodes within a HingeLink must have exact same location in 3d space");

            var distinctNodes = Nodes.Distinct().Where(ii => !ReferenceEquals(ii, null)).ToList();
            //Nodes.Select(i => i.Index).Distinct().ToList();


            var n = parent.Nodes.Count * 6;

            var coord = crd;// new CoordinateStorage<double>((distinctNodes.Count - 1) * 6, modelDofCount + 1, 1);

            var centralNode = distinctNodes.FirstOrDefault();

            if (centralNode == null)
                throw new Exception();

            {
                var cnt = 0;

                var cDx = centralNode.Index * 6 + 0;
                var cDy = centralNode.Index * 6 + 1;
                var cDz = centralNode.Index * 6 + 2;

                foreach (var nde in distinctNodes)
                {

                    if (ReferenceEquals(nde, centralNode))
                    {

                    }
                    else
                    {
                        var iDx = nde.Index * 6 + 0;
                        var iDy = nde.Index * 6 + 1;
                        var iDz = nde.Index * 6 + 2;

                        #region i'th Dx

                        //hing link is Dxi = DxCenter, then Dxi - DxCenter = 0, or vice versa

                        coord.At(cnt + startLine, cDx, 1);//Central.Dx
                        coord.At(cnt + startLine, iDx, -1);//Nde.Dx

                        #endregion

                        cnt++;

                        #region i'th Dy

                        //hing link is Dyi = DyCenter, then Dyi - DyCenter = 0, or vice versa
                        coord.At(cnt + startLine, cDy, 1);//Central.Dy
                        coord.At(cnt + startLine, iDy, -1);//Nde.Dy

                        #endregion

                        cnt++;

                        #region i'th Dz

                        //hing link is Dzi = DzCenter, then Dzi - DzCenter = 0, or vice versa
                        coord.At(cnt + startLine, cDz, 1);//Central.Dz 
                        coord.At(cnt + startLine, iDz, -1);//Nde.Dz

                        #endregion

                        cnt++;
                    }
                }

                return cnt;
            }

            
        }

        public override SparseMatrix GetExtraEquations()
        {
            var distLocation = Nodes.Select(i=>i.Location).Distinct();

            if (distLocation.Count() != 1)
                throw new BriefFiniteElementNetException("Nodes within a HingeLink must have exact same location in 3d space");
            
            var distinctNodes = Nodes.Distinct().Where(ii => !ReferenceEquals(ii, null)).ToList();
            //Nodes.Select(i => i.Index).Distinct().ToList();


            var modelDofCount = parent.Nodes.Count * 6;

            var coord = new CoordinateStorage<double>((distinctNodes.Count - 1) * 6, modelDofCount + 1, 1);

            var centralNode = distinctNodes.FirstOrDefault();

            if (centralNode == null)
                throw new Exception();

            {
                var i = 0;

                var cDx = centralNode.Index * 6 + 0;
                var cDy = centralNode.Index * 6 + 1;
                var cDz = centralNode.Index * 6 + 2;

                foreach (var nde in distinctNodes)
                {

                    if (ReferenceEquals(nde, centralNode))
                    {

                    }
                    else
                    {
                        var iDx = nde.Index * 6 + 0;
                        var iDy = nde.Index * 6 + 1;
                        var iDz = nde.Index * 6 + 2;

                        #region i'th Dx

                        //hing link is Dxi = DxCenter, then Dxi - DxCenter = 0, or vice versa

                        coord.At(i, cDx, 1);//Central.Dx
                        coord.At(i, iDx, -1);//Nde.Dx

                        #endregion

                        i++;

                        #region i'th Dy

                        //hing link is Dyi = DyCenter, then Dyi - DyCenter = 0, or vice versa
                        coord.At(i, cDy, 1);//Central.Dy
                        coord.At(i, iDy, -1);//Nde.Dy

                        #endregion

                        i++;

                        #region i'th Dz

                        //hing link is Dzi = DzCenter, then Dzi - DzCenter = 0, or vice versa
                        coord.At(i, cDz, 1);//Central.Dz 
                        coord.At(i, iDz, -1);//Nde.Dz

                        #endregion

                        i++;
                    }
                }
            }

            var buf = coord.ToCCs();

            var empties = buf.EmptyRowCount();

            if (empties != 0)
            {
                throw new Exception();
            }

            return buf;
        }

        public override int GetExtraEquationsCount()
        {
            //count is equal to slave DoFs
            //except one node, all others are slaves.
            return (Nodes.Count - 1) * 3;
        }

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }



        #endregion


     
    }
}
