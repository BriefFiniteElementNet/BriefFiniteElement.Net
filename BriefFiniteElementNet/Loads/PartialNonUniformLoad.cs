using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Loads
{

    /// <summary>
    /// Represents a nonuniform load over rectangular area of element in 1d, or 2d or 3d
    /// </summary>
    [Serializable]
    [Obsolete("still in development, have bugs")]
    public class PartialNonUniformLoad:Load
    {
        #region props, fields

        private double[] _magnitudesAtNodes;
        private IsoPoint _startLocation;
        public IsoPoint _endLocation;
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

        /*
        /// <summary>
        /// Gets or sets the magnitude(s) at start location
        /// </summary>
        /// <value>
        /// the severities at start of trapezoidal load</value>
        public double StartMagnitude
        {
            get { return _startMagnitude; }
            set { _startMagnitude = value; }
        }

        /// <summary>
        /// Gets or sets the magnitude(s) at end location
        /// </summary>
        /// <value>
        /// the severities at end of trapezoidal load</value>
        public double EndMagnitude
        {
            get { return _endMagnitude; }
            set { _endMagnitude = value; }
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// gets or sets the load magnitude at each element's node
        /// </value>
        public double[] MagnitudesAtNodes
        {
            get { return _magnitudesAtNodes; }
            set { _magnitudesAtNodes = value; }
        }

        #endregion



        public double[] GetMagnitudesAt(Element targetelement, IsoPoint location)
        {
            //var buf = 0.0;//new double[EndMagnitude.Length];

            //var dims = StartMagnitude.Length;

            //var n = Math.Min(dims, isoCoords.Length);

            throw new NotImplementedException();
            /*
            //for (var i = 0; i < n; i++)
            {
                //var startOffset = this.StartLocation[0];
                //var endOffset = this.EndLocation[0];
                var startMag = this.StartMagnitude;
                var endMag = this.EndMagnitude;

                var p0 = new Point(StartLocation.Xi, StartLocation.Eta, StartLocation.Lambda);
                var p1 = new Point(EndLocation.Xi, EndLocation.Eta, EndLocation.Lambda);

                var v = (p1 - p0);


                //var 

                var xi0 = -1 + startOffset;
                var xi1 = 1 - endOffset;

                var q0 = startMag;
                var q1 = endMag;

                var m = (q0 - q1) / (xi0 - xi1);

                var magnitude = q0 + m * (isoCoords[i] - xi0);
                buf[i] = magnitude;
            }

            return buf;
            */
        }

        /*
        public double[] GetMagnitudesAt(params double[] isoCoords)
        {
            var buf = new double[EndMagnitude.Length];

            var dims = StartMagnitude.Length;

            var n = Math.Min(dims, isoCoords.Length);

            for (var i = 0; i < n; i++)
            {
                var startOffset = this.StartLocation[0];
                var endOffset = this.EndLocation[0];
                var startMag = this.StartMagnitude[0];
                var endMag = this.EndMagnitude[0];

                var xi0 = -1 + startOffset;
                var xi1 = 1 - endOffset;

                var q0 = startMag;
                var q1 = endMag;

                var m = (q0 - q1) / (xi0 - xi1);

                var magnitude = q0 + m * (isoCoords[i] - xi0);
                buf[i] = magnitude;
            }

            return buf;

        }
        */

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
            info.AddValue("_magnitudesAtNodes", _magnitudesAtNodes);
            info.AddValue("_startLocation", _startLocation);
            info.AddValue("_endLocation", _endLocation);
            info.AddValue("_startMagnitude", _startMagnitude);
            info.AddValue("_endMagnitude", _endMagnitude);
            info.AddValue("_direction", _direction);
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
            _magnitudesAtNodes = (double[])info.GetValue("_magnitudesAtNodes", typeof(double[]));
            _startLocation = (IsoPoint)info.GetValue("_startLocation", typeof(IsoPoint));
            _endLocation = (IsoPoint)info.GetValue("_endLocation", typeof(IsoPoint));
            _startMagnitude = (double)info.GetValue("_startMagnitude", typeof(double));
            _endMagnitude = (double)info.GetValue("_endMagnitude", typeof(double));
            _direction = (Vector)info.GetValue("_direction", typeof(Vector));
        }

        #endregion
    }
}