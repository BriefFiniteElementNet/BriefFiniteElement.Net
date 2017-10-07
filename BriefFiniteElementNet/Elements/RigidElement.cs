using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using CSparse.Double;
using CSparse.Storage;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents an element with rigid body with no relative deformation of <see cref="Nodes"/> (the MPC version)
    /// </summary>
    [Serializable]
    [Obsolete("use RigidElement instead. MPC version is under development.")]
    public sealed class RigidElement_MPC : MpcElement
    {
        public RigidElement_MPC():base()
        {

        }

        private RigidElement_MPC(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override CompressedColumnStorage GetExtraEquations()
        {
            var distinctNodes = Nodes.Distinct().Where(ii => !ReferenceEquals(ii, null)).ToList();

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
                var cRx = centralNode.Index * 6 + 3;
                var cRy = centralNode.Index * 6 + 4;
                var cRz = centralNode.Index * 6 + 5;

                foreach (var nde in distinctNodes)
                {

                    if ( ReferenceEquals(nde, centralNode))
                    {

                    }
                    else
                    {
                        var iDx = nde.Index * 6 + 0;
                        var iDy = nde.Index * 6 + 1;
                        var iDz = nde.Index * 6 + 2;
                        var iRx = nde.Index * 6 + 3;
                        var iRy = nde.Index * 6 + 4;
                        var iRz = nde.Index * 6 + 5;


                        var d = nde.Location - centralNode.Location;

                        #region i'th Dx

                        //7.15 p 16, ref[1] sais i'th node's:
                        //nde.Dx = Central.Dx + d.Z * Central.Ry - d.Y * Central.Rz
                        // So
                        //-nde.Dx + Central.Dx + d.Z * Central.Ry - d.Y * Central.Rz
                        coord.At(i, cDx, 1);//Central.Dx
                        coord.At(i, cRy, d.Z);//d.Z * Central.Ry
                        coord.At(i, cRz, -d.Y);//- d.Y * Central.Rz

                        coord.At(i, iDx, -1);//Nde.Dx

                        #endregion

                        i++;

                        #region i'th Dy

                        //nde.Dy = Central.Dy - d.Z * Central.Rx + d.X * Central.Rz
                        //so
                        //-nde.Dy + Central.Dy - d.Z * Central.Rx + d.X * Central.Rz
                        coord.At(i, cDy, 1);//Central.Dy
                        coord.At(i, cRx, -d.Z);//-d.Z * Central.Rx
                        coord.At(i, cRz, d.X);//d.X * Central.Rz

                        coord.At(i, iDy, -1);//Nde.Dy

                        #endregion

                        i++;

                        #region i'th Dz

                        //nde.Dz = Central.Dz + d.Y * Central.Rx - d.X * Central.Ry
                        //so
                        //-nde.Dz + Central.Dz + d.Y * Central.Rx - d.X * Central.Ry
                        coord.At(i, cDz, 1);//Central.Dz 
                        coord.At(i, cRx, d.Y);//d.Y * Central.Rx
                        coord.At(i, cRy, -d.X);//- d.X * Central.Ry

                        coord.At(i, iDz, -1);//Nde.Dz

                        #endregion

                        i++;

                        #region i'th Rx
                        //nde.Rx = Central.Rx
                        coord.At(i, cRx, 1);

                        coord.At(i, iRx, -1);
                        #endregion

                        i++;

                        #region i'th Ry
                        //nde.Ry = Central.Ry
                        coord.At(i, cRy, 1);

                        coord.At(i, iRy, -1);
                        #endregion

                        i++;

                        #region i'th Rz
                        //nde.Rz = Central.Rz
                        coord.At(i, cRz, 1);

                        coord.At(i, iRz, -1);
                        #endregion

                        i++;
                    }
                }
            }

            var buf =  coord.ToCCs();

            var empties = buf.EmptyRowCount();

            if(empties!=0)
            {
                throw new Exception();
            }
            
            return buf;
        }

        public override int GetExtraEquationsCount()
        {
            //count is equal to slave DoFs
            //except one node, all others are slaves.
            return (Nodes.Count - 1) * 6;
        }
    }
}