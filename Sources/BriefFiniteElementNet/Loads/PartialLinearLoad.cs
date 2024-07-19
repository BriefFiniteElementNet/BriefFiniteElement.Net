using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Loads
{

    /// <summary>
    /// Represents a linearly varying load on an element's body. Partial means not covering whole element's body, but only one region and nonuniform means load severity changes along any location linearly.
    /// The partial part of element is defined in iso parametric coordination system
    /// </summary>
    [Serializable]
    [Obsolete("still in development, have bugs")]
    public class PartialLinearLoad : ElementalLoad
    {
        #region props, fields

        private IsoPoint _startLocation;
        private IsoPoint _endLocation;
        public double _startMagnitude;
        public double _endMagnitude;
        private Vector _direction;
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

        /// <summary>
        /// Gets or sets the load magnitude at start location
        /// </summary>
        public double StartMagnitude
        {
            get { return _startMagnitude; }
            set { _startMagnitude = value; }
        }

        /// <summary>
        /// Gets or sets the load magnitude at end location
        /// </summary>
        public double EndMagnitude
        {
            get { return _endMagnitude; }
            set { _endMagnitude = value; }
        }

        #endregion



        public double GetMagnitudeAt(Element targetelement, IsoPoint location)
        {
            var afterStart =
                location.Xi >= _startLocation.Xi &&
                location.Eta >= _startLocation.Eta &&
                location.Lambda >= _startLocation.Lambda;

            var beforeEnd =
                location.Xi <= _endLocation.Xi &&
                location.Eta <= _endLocation.Eta &&
                location.Lambda <= _endLocation.Lambda;

            if (afterStart && beforeEnd)
                return NumericUtils.LinearInterpolate(StartLocation.Xi, StartMagnitude, EndLocation.Xi, EndMagnitude, location.Xi);

            return 0;
        }

        public override IsoPoint[] GetInternalForceDiscretationPoints()
        {
            return new IsoPoint[] { StartLocation, EndLocation };
        }


        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_startLocation", _startLocation);
            info.AddValue("_endLocation", _endLocation);
            info.AddValue("_startMagnitude", _startMagnitude);
            info.AddValue("_endMagnitude", _endMagnitude);

            info.AddValue("_coordinationSystem", (int)_coordinationSystem);
            info.AddValue("_direction", _direction);
        }

        #endregion


        #region Constructor

        public PartialLinearLoad()
        {
        }

        #endregion


        #region Deserialization Constructor

        protected PartialLinearLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _coordinationSystem = (CoordinationSystem)(int)info.GetValue("_coordinationSystem", typeof(int));
            _startLocation = (IsoPoint)info.GetValue("_startLocation", typeof(IsoPoint));
            _endLocation = (IsoPoint)info.GetValue("_endLocation", typeof(IsoPoint));
            _direction = (Vector)info.GetValue("_direction", typeof(Vector));

            _startMagnitude = (double)info.GetValue("_startMagnitude", typeof(double));
            _endMagnitude = (double)info.GetValue("_endMagnitude", typeof(double));
        }

        #endregion
    }
}
