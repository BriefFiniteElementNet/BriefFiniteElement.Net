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
    /// Represents an concentrated load which applies in the element's body.
    /// </summary>
    [Serializable]
    [Obsolete("still in development, has bugs")]

    public class ConcentratedLoad : ElementalLoad
    {


        public ConcentratedLoad():base()
        {
        }

        private IsoPoint _forceIsoLocation;

        private Force _force;

        private CoordinationSystem _coordinationSystem;


        /// <summary>
        /// The isoparametric (xi eta gamma) location of force inside element's body
        /// </summary>
        public IsoPoint ForceIsoLocation
        {
            get
            {
                return _forceIsoLocation;
            }

            set
            {
                _forceIsoLocation = value;
            }
        }

        /// <summary>
        /// The concentrated force amount
        /// </summary>
        public Force Force
        {
            get
            {
                return _force;
            }

            set
            {
                _force = value;
            }
        }

        /// <summary>
        /// Gets or sets the coordination system of force.
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


        #region SetIsoLocation

        /// <summary>
        /// Sets the isoparametric location of concentrated force.
        /// </summary>
        /// <param name="isoCoords">The isoparametric coordiation components (xi, eta, gamma).</param>
        private void SetIsoLocation(params double[] isoCoords)
        {
            _forceIsoLocation = new IsoPoint(isoCoords[0], isoCoords[1], isoCoords[2]);

            /*
            for (var i = 0; i < isoCoords.Length; i++)
            {
                if (i >= _forceIsoLocation.Length)
                    break;

                _forceIsoLocation[i] = isoCoords[i];
            }*/
        }

        /*
        private void SetIsoLocation(double xi)
        {
            SetIsoLocation(new double[] { xi, 0, 0 });
        }

        private void SetIsoLocation(double xi, double eta)
        {
            SetIsoLocation(new double[] { xi, eta, 0 });
        }

        private void SetIsoLocation(double xi, double eta, double gamma)
        {
            SetIsoLocation(new double[] { xi, eta, gamma });
        }
        */
        #endregion

        #region Deserialization Constructor

        protected ConcentratedLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _forceIsoLocation = (IsoPoint)info.GetValue("_forceIsoLocation", typeof(IsoPoint));
            _force = (Force)info.GetValue("_force", typeof(Force));
            _coordinationSystem = (CoordinationSystem)(int)info.GetValue("_coordinationSystem", typeof(int));

        }

        public ConcentratedLoad(Force force, IsoPoint forceIsoLocation, CoordinationSystem coordinationSystem)
        {
            Force = force;
            ForceIsoLocation = forceIsoLocation;
            CoordinationSystem = coordinationSystem;
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_forceIsoLocation", _forceIsoLocation);
            info.AddValue("_force", _force);
            info.AddValue("_coordinationSystem", (int)_coordinationSystem);
        }

        #endregion

       
        public override IsoPoint[] GetInternalForceDiscretationPoints()
        {
            var buf = new List<IsoPoint>();

            buf.Add(_forceIsoLocation);

            return buf.ToArray();
        }
    }
}
