using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
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

    public class ConcentratedLoad : Load
    {
        private double[] _forceIsoLocation;
       
        private Force _force;

        private CoordinationSystem _coordinationSystem;


        /// <summary>
        /// The isoparametric (xi eta gamma) location of force inside element's boddy
        /// </summary>
        public double[] ForceIsoLocation
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
        /// The force magnitude
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

        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            throw new NotImplementedException();
        }

        #region SetIsoLocation

        /// <summary>
        /// Sets the isoparametric location of concentrated force.
        /// </summary>
        /// <param name="isoCoords">The isoparametric coordiation components (xi, eta, gamma).</param>
        private void SetIsoLocation(params double[] isoCoords)
        {
            _forceIsoLocation = new double[3];

            for (var i = 0; i < isoCoords.Length; i++)
            {
                if (i >= _forceIsoLocation.Length)
                    break;

                _forceIsoLocation[i] = isoCoords[i];
            }
        }

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
        #endregion

        #region Deserialization Constructor

        protected ConcentratedLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _forceIsoLocation = (double[])info.GetValue("_forceIsoLocation", typeof(Vector));
            _force = (Force)info.GetValue("_force", typeof(double));
            _coordinationSystem = (CoordinationSystem)(int)info.GetValue("_coordinationSystem", typeof(double));

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
    }
}
