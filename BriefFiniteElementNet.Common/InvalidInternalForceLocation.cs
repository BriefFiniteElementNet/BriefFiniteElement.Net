using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Used when 
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidInternalForceLocationException : Exception
    {
        public InvalidInternalForceLocationException()
        {
        }

        public InvalidInternalForceLocationException(string message) : base(message)
        {
        }

        public InvalidInternalForceLocationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidInternalForceLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
