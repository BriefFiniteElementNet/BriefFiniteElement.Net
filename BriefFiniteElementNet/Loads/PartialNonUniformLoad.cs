using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Runtime.Serialization;
using System.Security.Permissions;
using BriefFiniteElementNet.Mathh;

namespace BriefFiniteElementNet.Loads
{

    /// <summary>
    /// Represents a nonuniform load on an element's body. Partial means not covering whole element's body, but only one region and nonuniform means load severity changes along any location.
    /// The partial part of element is defined in iso parametric coordination system
    /// </summary>
    [Serializable]
    [Obsolete("still in development, have bugs")]
    public class PartialNonUniformLoad:ElementalLoad
    {
        #region props, fields

        //private double[] _magnitudesAtNodes;
        private IsoPoint _startLocation;
        private IsoPoint _endLocation;
        //public double _startMagnitude;
        //public double _endMagnitude;
        private Vector _direction;
        private CoordinationSystem _coordinationSystem;
        //private Func<IsoPoint, double> severityFunction;

        

        /// <summary>
        /// Sets or gets the direction of load
        /// </summary>
        public Vector Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Gets or sets the coordination system that <see cref="Direction"/> is defined in.
        /// </summary>
        /// <value>
        /// The coordination system.
        /// </value>
        public CoordinationSystem CoordinationSystem
        {
            get { return _coordinationSystem; }
            set { _coordinationSystem = value; }
        }

        /// <summary>
        /// Gets or sets the start of load range
        /// </summary>
        /// <value>
        /// The offset from start node(s) regarding isoparametric coordination system (in rangle [-1,1])
        /// </value>
        public IsoPoint StartLocation
        {
            get { return _startLocation; }
            set { _startLocation = value; }
        }

        /// <summary>
        /// Gets or sets the end of load range
        /// </summary>
        /// <value>
        /// The offset from end node(s) regarding isoparametric coordination system (in rangle [-1,1])
        /// </value>
        public IsoPoint EndLocation
        {
            get { return _endLocation; }
            set { _endLocation = value; }
        }

        /**/
        /// <summary>
        /// Gets or sets the severity function
        /// </summary>
        public Mathh.SingleVariablePolynomial SeverityFunction
        {
            get { return _severityFunction; }
            set
            {
                _severityFunction = value;
            }
        }

        private Mathh.SingleVariablePolynomial _severityFunction;
        /**/


        /// <summary>
        /// Gets or sets the severity function
        /// </summary>
        public IPolynomial SeverityFunction_old
        {
            get { return _severityFunction_old; }
            set
            {
                _severityFunction_old = value;
            }
        }

        private IPolynomial _severityFunction_old;


        #endregion



        public double GetMagnitudeAt(Element targetelement, IsoPoint location)
        {
            //return _severityFunction(location);


            throw new NotImplementedException();
        }

        public override IsoPoint[] GetInternalForceDiscretationPoints()
        {
            //no discrete points
            return new IsoPoint[0];
        }

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_startLocation", _startLocation);
            info.AddValue("_endLocation", _endLocation);
            info.AddValue("_coordinationSystem", (int)_coordinationSystem);
            info.AddValue("_direction", _direction);
            info.AddValue("_severityFunction", _severityFunction);
        }

        #endregion


        #region Constructor

        public PartialNonUniformLoad()
        {
        }

        #endregion


        #region Deserialization Constructor

        protected PartialNonUniformLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _coordinationSystem = (CoordinationSystem)(int)info.GetValue("_coordinationSystem", typeof(int));
            _startLocation = (IsoPoint)info.GetValue("_startLocation", typeof(IsoPoint));
            _endLocation = (IsoPoint)info.GetValue("_endLocation", typeof(IsoPoint));
            _direction = (Vector)info.GetValue("_direction", typeof(Vector));
            _severityFunction = (Mathh.SingleVariablePolynomial)info.GetValue("_direction", typeof(Mathh.SingleVariablePolynomial));
        }

        #endregion
    }
}