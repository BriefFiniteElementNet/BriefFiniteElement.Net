using System.Runtime.Serialization;
using System.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a telepathy link between DoF's of two nodes. connected DoF s will have equal displacement after analysis.
    /// </summary>
    [Serializable]
    [Obsolete("Not usable yet, under development")]
    public class TelepathyLink : MpcElement
    {
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

        public override CompressedColumnStorage GetExtraEquations()
        {
            throw new NotImplementedException();
        }

        public override int GetExtraEquationsCount()
        {
            throw new NotImplementedException();
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
