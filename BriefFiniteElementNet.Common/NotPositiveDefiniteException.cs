using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a exception for cases that stiffness matrix (Kff) is not positive definite
    /// </summary>
    [Serializable]
    public class NotPositiveDefiniteException:BriefFiniteElementNetException
    {
        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotPositiveDefiniteException"/> class.
        /// </summary>
        public NotPositiveDefiniteException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotPositiveDefiniteException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotPositiveDefiniteException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotPositiveDefiniteException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public NotPositiveDefiniteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotPositiveDefiniteException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected NotPositiveDefiniteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
