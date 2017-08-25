using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for exceptions in <see cref="Matrix"/>
    /// </summary>
    [Serializable]
    public class MatrixException : BriefFiniteElementNetException
    {
        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
        }

        public MatrixException()
        {
        }

        public MatrixException(string message)
            : base(message)
        {
        }

        public MatrixException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MatrixException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [DebuggerHidden]
        public static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new MatrixException(message);
        }
    }
}
