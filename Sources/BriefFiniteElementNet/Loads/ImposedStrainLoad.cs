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
    /// Represents an uniform imposed strain load applied to element
    /// </summary>
    /// <remarks>
    /// Can be used for thermal expansion situation or ...
    /// </remarks>
    [Serializable]
    public class ImposedStrainLoad: ElementalLoad
    {

        /// <inheritdoc/>
        public ImposedStrainLoad() : base()
        {

        }

        private double _imposedStrainMagnitude;

        /// <summary>
        /// Gets or sets the magnitude of imposed strain to the element. strain is kind of unitless.
        /// </summary>
        public double ImposedStrainMagnitude
        {
            get { return _imposedStrainMagnitude; }
            set
            {
                _imposedStrainMagnitude = value;
            }
        }

        /// <inheritdoc/>
        public override IsoPoint[] GetInternalForceDiscretationPoints()
        {
            return new IsoPoint[0];
        }

        #region Deserialization Constructor

        /// <inheritdoc/>
        protected ImposedStrainLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _imposedStrainMagnitude = (double)info.GetValue("_imposedStrainMagnitude", typeof(double));
        }

        /// <inheritdoc/>
        public ImposedStrainLoad(double imposedStrainMagnitude)
        {
            _imposedStrainMagnitude = imposedStrainMagnitude;
        }

        #endregion

        #region ISerialization Implementation
        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_imposedStrainMagnitude", _imposedStrainMagnitude);
            base.GetObjectData(info, context);
        }

        #endregion

    }
}
