using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a general load that can apply to a node (include 3 force and 3 moments)
    /// </summary>
    [Serializable]
    public class NodalLoad : ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodalLoad"/> class.
        /// </summary>
        public NodalLoad()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodalLoad"/> struct with <see cref="LoadCase.DefaultLoadCase"/> as <see cref="NodalLoad.Case"/>.
        /// </summary>
        /// <param name="force">The force.</param>
        public NodalLoad(Force force) 
        {
            this.force = force;
            this._case = LoadCase.DefaultLoadCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodalLoad"/> struct.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="case">The Load case.</param>
        public NodalLoad(Force force, LoadCase @case)
        {
            this.force = force;
            _case = @case;
        }

        private Force force;

        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        /// <value>
        /// The magnitude of <see cref="NodalLoad"/>.
        /// </value>
        public Force Force
        {
            get { return force; }
            set { force = value; }
        }

        /// <summary>
        /// Gets or sets the case.
        /// </summary>
        /// <value>
        /// The Load case of <see cref="NodalLoad"/>.
        /// </value>
        public LoadCase Case
        {
            get { return _case; }
            set { _case = value; }
        }

        private LoadCase _case;

        #region Serialization stuff


        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("force", force);
            info.AddValue("_case", _case);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodalLoad" /> class. satisfies the constrictor for <see cref="ISerializable" /> interface.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected NodalLoad(SerializationInfo info, StreamingContext context)
        {
            force = (Force) info.GetValue("force", typeof (Force));
            _case = (LoadCase) info.GetValue("_case", typeof (LoadCase));
        }

        #endregion

    }
}
