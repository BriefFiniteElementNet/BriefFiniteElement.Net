using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace BriefFiniteElementNet.Controls.ElementRenderers
{
    public interface IElementRenderer<T> where T : BriefFiniteElementNet.Elements.Element
    {
        /// <summary>
        /// Renders the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        ModelUIElement3D Render(T element, RenderSettings settings);
    }

    /// <summary>
    /// Represents a class for settings of render
    /// </summary>
    public class RenderSettings
    {
        /// <summary>
        /// The showing load combination, used for displacements and internal forces
        /// </summary>
        public LoadCombination ShowingLoadCombination;

        /// <summary>
        /// The scale used for showing the deformation 
        /// </summary>
        public double DeformationScale;

        /// <summary>
        /// The value to show deformation or not
        /// </summary>
        public bool ShowDeformation;

        /// <summary>
        /// The value to show internal force or not
        /// </summary>
        public bool ShowInternalForce;

    }
}
