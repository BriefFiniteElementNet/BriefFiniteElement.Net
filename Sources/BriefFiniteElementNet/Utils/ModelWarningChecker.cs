using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using System;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represent a tool for checking the model warnings that probably leads to error in solve time
    /// </summary>
    public class ModelWarningChecker
    {
        /// <summary>
        /// Checks the model for warnings.
        /// </summary>
        /// <param name="model">The model.</param>
        public void CheckModel(Model model)
        {
            for (var i = 0; i < model.Elements.Count; i++)
            {
                var elementIdentifier = string.Format("{0}'th element in Model.Elements",i);

            }
        }


       
    }
}
