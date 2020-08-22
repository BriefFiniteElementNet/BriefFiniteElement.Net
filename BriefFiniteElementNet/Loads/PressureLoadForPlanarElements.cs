using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Loads
{
    /// <summary>
    /// Represents a pressure load for shells - always perpendicular to the surface of the shell
    /// </summary>
    [Serializable]
    public class PressureLoadForPlanarElements : ElementalLoad
    {
        #region ctor + properties
        public PressureLoadForPlanarElements()
        {

        }

        private double _magnitude;
        /// <summary>
        /// Gets or sets the magnitude of the pressure load (always perpendicular to the face).
        /// </summary>
        /// <value>
        /// The magnitude of the pressure
        /// </value>
        public double Magnitude
        {
            get { return _magnitude; }
            set { _magnitude = value; }
        }

        public PressureLoadForPlanarElements(double magnitude, LoadCase lc)
        : base(lc)
        {
            _magnitude = magnitude;
        }

        public PressureLoadForPlanarElements(double magnitude)
            : base(LoadCase.DefaultLoadCase)
        {
            _magnitude = magnitude;
        }
        #endregion

        #region Serialization
        protected PressureLoadForPlanarElements(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _magnitude = info.GetDouble("_magnitude");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_magnitude", _magnitude);

            base.GetObjectData(info, context);
        }
        #endregion

        /// <inheritdoc />
        public Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            return element.GetGlobalEquivalentNodalLoads(this);
        }

        public override IsoPoint[] GetInternalForceDiscretationPoints()
        {
            //no discrete points
            return new IsoPoint[0];
        }
    }
}
