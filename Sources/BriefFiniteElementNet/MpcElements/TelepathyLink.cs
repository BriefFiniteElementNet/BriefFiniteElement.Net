using System.Runtime.Serialization;
using System.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;

namespace BriefFiniteElementNet.MpcElements
{
    /// <summary>
    /// Represents a telepathy link between DoF's of two nodes. connected DoF s will have equal displacement after analysis (like equal dof)
    /// </summary>
    [Serializable]
    [Obsolete("Not usable yet, under development")]
    public class TelepathyLink : MpcElement
    {

        public TelepathyLink(params Node[] nodes)
        {
            base.Nodes.AddRange(nodes);
        }



        private bool _connectDx, _connectDy, _connectDz, _connectRx, _connectRy, _connectRz;

        /// <summary>
        /// Gets or sets a value indicating wether bind Dx DoF of all nodes in this element together or not.
        /// </summary>
        public bool ConnectDx
        {
            get { return _connectDx; }
            set { _connectDx = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether bind Dy DoF of all nodes in this element together or not.
        /// </summary>
        public bool ConnectDy
        {
            get { return _connectDy; }
            set { _connectDy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether bind Dz DoF of all nodes in this element together or not.
        /// </summary>
        public bool ConnectDz
        {
            get { return _connectDz; }
            set { _connectDz = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether bind Rx DoF of all nodes in this element together or not.
        /// </summary>
        public bool ConnectRx
        {
            get { return _connectRx; }
            set { _connectRx = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether bind Ry DoF of all nodes in this element together or not.
        /// </summary>
        public bool ConnectRy
        {
            get { return _connectRy; }
            set { _connectRy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether bind Rz DoF of all nodes in this element together or not.
        /// </summary>
        public bool ConnectRz
        {
            get { return _connectRz; }
            set { _connectRz = value; }
        }



        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_connectDx", _connectDx);
            info.AddValue("_connectDy", _connectDy);
            info.AddValue("_connectDz", _connectDz);

            info.AddValue("_connectRx", _connectRx);
            info.AddValue("_connectRy", _connectRy);
            info.AddValue("_connectRz", _connectRz);

            base.GetObjectData(info, context);
        }

        public override SparseMatrix GetExtraEquations()
        {
            var n = parent.Nodes.Count;

            var buf = new CSparse.Storage.CoordinateStorage<double>(GetExtraEquationsCount(), parent.Nodes.Count * 6 + 1, 10);

            
            var cnt = 0;

            //for (var i = 0; i < Nodes.Count; i++)
            {
                var ndei = Nodes[0];
                var stiIdx = ndei.Index * 6;

                for (var j = 1; j < Nodes.Count; j++)
                {
                    var ndej = Nodes[j];
                    var stjIdx = ndej.Index * 6;

                    if (_connectDx)
                    {
                        buf.At(cnt, stiIdx, 1);
                        buf.At(cnt, stjIdx,  -1);
                        cnt++;
                    }

                    if (_connectDy)
                    {
                        buf.At(cnt, stiIdx + 1,  1);
                        buf.At(cnt, stjIdx + 1,  -1);
                        cnt++;
                    }

                    if (_connectDz)
                    {
                        buf.At(cnt, stiIdx + 2,  1);
                        buf.At(cnt, stjIdx + 2,  -1);
                        cnt++;
                    }


                    if (_connectRx)
                    {
                        buf.At(cnt, stiIdx + 3,  1);
                        buf.At(cnt, stjIdx + 3,  -1);
                        cnt++;
                    }

                    if (_connectRy)
                    {
                        buf.At(cnt, stiIdx + 4,  1);
                        buf.At(cnt, stjIdx + 4,  -1);
                        cnt++;
                    }

                    if (_connectRz)
                    {
                        buf.At(cnt, stiIdx + 5,  1);
                        buf.At(cnt, stjIdx + 5,  -1);
                        cnt++;
                    }

                }
            }

            var buf2 = buf.ToCCs();


            return buf2;
        }

        public override int GetExtraEquationsCount()
        {
            var n = Nodes.Count;

            var buf = 0;

            var lst = new bool[] { _connectDx, _connectDy, _connectDz, _connectRx, _connectRy, _connectRz };

            foreach (var val in lst)
                if (val)
                    buf += (n - 1);

            return buf;
        }

        protected TelepathyLink(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _connectDx = info.GetBoolean("_connectDx");
            _connectDy = info.GetBoolean("_connectDy");
            _connectDz = info.GetBoolean("_connectDz");

            _connectRx = info.GetBoolean("_connectRx");
            _connectRy = info.GetBoolean("_connectRy");
            _connectRz = info.GetBoolean("_connectRz");
        }
    }
}
