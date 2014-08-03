using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    [Serializable]
    public class InvalidLabelException : BriefFiniteElementNetException
    {
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
