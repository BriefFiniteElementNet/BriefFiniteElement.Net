using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the base class for customised Exceptions in BriefFiniteElementNet
    /// </summary>
    public class BriefFiniteElementNetException : Exception
    {
        public BriefFiniteElementNetException()
        {
        }

        public BriefFiniteElementNetException(string message) : base(message)
        {
        }

        public BriefFiniteElementNetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BriefFiniteElementNetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


}
