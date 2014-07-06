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
    internal static class ExceptionHelper
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
    }
}
