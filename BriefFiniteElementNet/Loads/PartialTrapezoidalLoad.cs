using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Loads
{
   
    /// <summary>
    /// Represents a trapezoidal load with start and end offset.
    /// </summary>
    [Obsolete("still in development")]
    public class PartialTrapezoidalLoad:Load
    {
        #region props, fields

        private double[] _startOffsets;
        public double[] _endOffsets;
        public double[] _startSeverities;
        public double[] _endSeverities;
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
        public double[] StarIsoLocations
        {
            get { return _startOffsets; }
            set { _startOffsets = value; }
        }

        /// <summary>
        /// Gets or sets the end of load range
        /// </summary>
        /// <value>
        /// The offset from end node(s) regarding isoparametric coordination system (in rangle [-1,1])
        /// </value>
        public double[] EndIsoLocations
        {
            get { return _endOffsets; }
            set { _endOffsets = value; }
        }

        /// <summary>
        /// Gets or sets the magnitude(s) at start location
        /// </summary>
        /// <value>
        /// the severities at starting of trapezoidal load</value>
        public double[] StartMagnitudes
        {
            get { return _startSeverities; }
            set { _startSeverities = value; }
        }

        /// <summary>
        /// Gets or sets the magnitude(s) at end location
        /// </summary>
        /// <value>
        /// the severities at starting of trapezoidal load</value>
        public double[] EndMagnitudes
        {
            get { return _endSeverities; }
            set { _endSeverities = value; }
        }

        #endregion

        

        public double[] GetMagnitudesAt(params double[] isoCoords)
        {
            var buf = new double[EndMagnitudes.Length];

            var dims = StartMagnitudes.Length;

            var n = Math.Min(dims, isoCoords.Length);

            for (var i = 0; i < n; i++)
            {
                var startOffset = this.StarIsoLocations[0];
                var endOffset = this.EndIsoLocations[0];
                var startMag = this.StartMagnitudes[0];
                var endMag = this.EndMagnitudes[0];

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
    }
}