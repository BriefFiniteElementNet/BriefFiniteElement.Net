using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class ValidationResult
    {
        public string HtmlSpanElement;

        public HtmlTags.HtmlTag Span = new HtmlTags.HtmlTag("span");

        public string Title;

        public bool? ValidationFailed;
    }
}
