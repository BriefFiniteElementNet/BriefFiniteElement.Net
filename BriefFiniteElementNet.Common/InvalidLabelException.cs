using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace BriefFiniteElementNet
{
    [Serializable]
    public class InvalidLabelException : BriefFiniteElementNetException
    {
        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
        }

        public InvalidLabelException()
        {
        }

        public InvalidLabelException(string message) : base(message)
        {
        }

        public InvalidLabelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLabelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
