using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the base class for customized Exceptions in BriefFiniteElementNet
    /// </summary>
    [Serializable]
    public class BriefFiniteElementNetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BriefFiniteElementNetException"/> class.
        /// </summary>
        public BriefFiniteElementNetException()
        {
            this.HelpLink =
                "https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list&referringTitle=Documentation";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BriefFiniteElementNetException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BriefFiniteElementNetException(string message) : base(message)
        {
            this.HelpLink =
                "https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list&referringTitle=Documentation";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BriefFiniteElementNetException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public BriefFiniteElementNetException(string message, Exception innerException) : base(message, innerException)
        {
            this.HelpLink =
                "https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list&referringTitle=Documentation";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BriefFiniteElementNetException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BriefFiniteElementNetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.HelpLink =
                "https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list&referringTitle=Documentation";
            IssueId = (string)info.GetValue("IssueId", typeof(string));
        }

        public string IssueId;

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IssueId", IssueId);
        }

        #endregion
    }


}
