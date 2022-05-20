using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    /// <summary>
    /// Represents an validation result
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// A Span element which is inside of an HTML
        /// </summary>
        public HtmlTags.HtmlTag Span = new HtmlTags.HtmlTag("span");

        /// <summary>
        /// 
        /// </summary>
        public string Title;

        /// <summary>
        /// 
        /// </summary>
        public bool? ValidationFailed;
    }
}
