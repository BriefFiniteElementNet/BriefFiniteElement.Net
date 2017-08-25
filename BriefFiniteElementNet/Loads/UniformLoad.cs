using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Loads
{
    [Serializable]
    [Obsolete("still in development")]
    public class UniformLoad:Load
    {
        private Vector _direction;
        private double _magnitude;

        /// <summary>
        /// Sets or gets the direction of load
        /// </summary>
        public Vector Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Sets or gets the magnitude of load
        /// </summary>
        public double Magnitude
        {
            get { return _magnitude; }
            set { _magnitude = value; }
        }


        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            throw new NotImplementedException();
        }


        #region Constructor

        public UniformLoad()
        {
        }

        #endregion


        #region Deserialization Constructor

        protected UniformLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _direction = (Vector)info.GetValue("_direction", typeof(Vector));
            _magnitude = (double)info.GetValue("_magnitude", typeof(double));
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_direction", _direction);
            info.AddValue("_magnitude", _magnitude);
        }

        #endregion
    }
}
