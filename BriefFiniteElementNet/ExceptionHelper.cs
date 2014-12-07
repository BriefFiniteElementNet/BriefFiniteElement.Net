using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Throws the member with same label exists exception.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <exception cref="InvalidLabelException"></exception>
        [DebuggerHidden]
        public static void ThrowMemberWithSameLabelExistsException(string label)
        {
            throw new InvalidLabelException(string.Format("Member with same label ({0}) exists in the Model", label));
        }


        /// <summary>
        /// Throws an exception with specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="issueId">The issue identifier.</param>
        [DebuggerHidden]
        public static void Throw(string message,string issueId)
        {
            var ex = new BriefFiniteElementNetException(message);
            ex.IssueId = issueId;

            throw ex;
        }



        /// <summary>
        /// Throws the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="issueId">The issue identifier.</param>
        public static void Throw(string issueId)
        {
            var msg = string.Format("An error of number {0} is occurred.", issueId);

            var ex = new BriefFiniteElementNetException(msg);
            ex.IssueId = issueId;

            throw ex;
        }
    }
}
