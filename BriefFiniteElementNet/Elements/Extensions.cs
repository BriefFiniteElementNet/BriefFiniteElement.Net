using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Some extension methods
    /// </summary>
    public static class Extensions
    {
        #region peace add methods
        /// <summary>
        /// get the global displacement at xi
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="loadCase"></param>
        /// <returns></returns>
        public static Displacement GetGlobalDisplacementAt(this BarElement element, double xi, LoadCase loadCase)
        {
            var localDisp = element.GetInternalDisplacementAt(xi, loadCase);
            var trMgr = element.GetTransformationManager();
            return trMgr.TransformLocalToGlobal(localDisp);
        }
        /// <summary>
        /// get the global displacement at xi for DefaultLoadCase
        /// </summary>
        /// <param name="xi"></param>
        /// <returns></returns>
        public static Displacement GetGlobalDisplacementAt(this BarElement element, double xi)
        {
            return element.GetGlobalDisplacementAt(xi, LoadCase.DefaultLoadCase);
        }
        
        #endregion
    }
}
