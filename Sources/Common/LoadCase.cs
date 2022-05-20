using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a load case.
    /// </summary>
    /// <remarks>
    /// In comparing two different load case, white space characters and 
    /// </remarks>
    [Serializable]
    [DebuggerDisplay("{CaseName} (Type : {LoadType})")]
    public struct LoadCase : IEquatable<LoadCase>, ISerializable
    {

        /// <summary>
        /// Gets the default load case.
        /// </summary>
        /// <value>
        /// The default load case.
        /// </value>
        /// <remarks>
        /// Gets a LoadCase with <see cref="LoadType"/> of <see cref="BriefFiniteElementNet.LoadType.Default"/> and empty <see cref="CaseName"/></remarks>
        public static LoadCase DefaultLoadCase
        {
            get { return new LoadCase(); }
        }

        private string caseName;
        private LoadType loadType;

        /// <summary>
        /// Gets or sets the type of the load.
        /// </summary>
        /// <value>
        /// The type of the load.
        /// </value>
        public LoadType LoadType
        {
            get { return loadType; }
            set { loadType = value; }
        }

        /// <summary>
        /// Gets or sets the CaseName.
        /// </summary>
        /// <value>
        /// The unique name of load case.
        /// </value>
        public string CaseName
        {
            get { return caseName; }
            set { caseName = value; }
        }

        #region Equality Compairer

        public bool Equals(LoadCase other)
        {
            if (this.loadType != other.loadType)
                return false;

            return new FemNetStringCompairer().Equals(caseName, other.caseName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LoadCase && Equals((LoadCase) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((caseName != null ? new FemNetStringCompairer().GetHashCode(caseName) : 4321)*397) ^ (int) loadType;
            }
        }

        public static bool operator ==(LoadCase left, LoadCase right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LoadCase left, LoadCase right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Serialization Stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("caseName", caseName);
            info.AddValue("loadType", (int) loadType);

            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCase"/> struct. for ISerialable.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private LoadCase(SerializationInfo info, StreamingContext context)
        {
            caseName = info.GetString("caseName");
            loadType = (LoadType) info.GetInt32("loadType");
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCase"/> struct.
        /// </summary>
        /// <param name="caseName">Name of the <see cref="LoadCase"/>.</param>
        /// <param name="loadType">Type of the <see cref="LoadCase"/>.</param>
        public LoadCase(string caseName, LoadType loadType)
        {
            this.caseName = caseName;
            this.loadType = loadType;
        }


    }
}
