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
        public UniformLoad(LoadCase @case) : base(@case)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="case"></param>
        /// <param name="direction">the direction of load</param>
        /// <param name="magnitude">magnitude of load</param>
        /// <param name="coordinationSystem">e coordination system that <see cref="Direction"/> is defined in</param>
        public UniformLoad(LoadCase @case, Vector direction, double magnitude, CoordinationSystem coordinationSystem) : base(@case)
        {
            _direction = direction;
            _magnitude = magnitude;
            _coordinationSystem = coordinationSystem;
        }

        private Vector _direction;
        private double _magnitude;
        private CoordinationSystem _coordinationSystem;

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


        /// <summary>
        /// Gets or sets the coordination system that <see cref="Direction"/> is defined in.
        /// </summary>
        /// <value>
        /// The coordination system.
        /// </value>
        public CoordinationSystem CoordinationSystem
        {
            get
            {
                return _coordinationSystem;
            }

            set
            {
                _coordinationSystem = value;
            }
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
            _coordinationSystem = (CoordinationSystem)(int)info.GetValue("_coordinationSystem", typeof(int));
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_direction", _direction);
            info.AddValue("_magnitude", _magnitude);
            info.AddValue("_coordinationSystem", (int)_coordinationSystem);
        }

        #endregion
    }
}
