using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using CSparse.Double;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents an element with rigid body with no relative deformation of <see cref="Nodes"/> (the MPC version)
    /// </summary>
    [Serializable]
    [Obsolete("use RigidElement instead. this is under development.")]
    public sealed class RigidElement_MPC : MpcElement
    {
        public override CompressedColumnStorage GetExtraEquations()
        {
            throw new NotImplementedException();
        }
    }
}