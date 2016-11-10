using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// represents an interface for an element helper
    /// </summary>
    public interface IElementHelper
    {
        /// <summary>
        /// Gets the B matrix at defined isometric coordinates (B is derivation of N regarding to local x or y or z, not regarding to ξ, η or γ -- it is ∂N/∂x or ..., it is not ∂N/∂ξ or ...).
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="transformMatrix">The transform matrix (local-global coordinates).</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// <returns></returns>
        Matrix GetBMatrixAt(Element targetElement,Matrix transformMatrix, params double[] isoCoords);

        /// <summary>
        /// Gets the D matrix at defined isometric coordinates.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="transformMatrix">The transform matrix (local-global coordinates).</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// <returns>1x1, 2x2 or 3x3 matrix</returns>
        Matrix GetDMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords);

        /// <summary>
        /// Gets the N matrix at defined isometric coordinates.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="transformMatrix">The transform matrix (local-global coordinates).</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// <returns></returns>
        Matrix GetNMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords);

        /// <summary>
        /// Gets the Jacobian matrix at defined isometric coordinates.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="transformMatrix">The transform matrix (local-global coordinates).</param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// <returns>
        /// 1x1, 2x2 or 3x3 Jacobian matrix at defined location
        /// </returns>
        Matrix GetJMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords);

        /// <summary>
        /// Gets the stiffness [K] matrix in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <returns></returns>
        Matrix GetKMatrix(Element targetElement, Matrix transformMatrix);

        /// <summary>
        /// Gets the DoF order of returned values.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <returns></returns>
        FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement);
    }
}