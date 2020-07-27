using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// represents an interface for an element helper
    /// </summary>
    public interface IElementHelper
    {
        Element TargetElement { get; set; }

        /// <summary>
        /// Gets the B matrix at defined isometric coordinates (B is derivation of N regarding to local x or y or z, not regarding to ξ, η or γ -- it is ∂N/∂x or ..., it is not ∂N/∂ξ or ...).
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// 
        /// <returns></returns>
        Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords);

        /// <summary>
        /// Gets the compliance D matrix at defined isometric coordinates in local coordination system.
        /// </summary>
        /// <remarks>
        /// This will be used for creating Stiffness matrix
        /// </remarks>
        /// <param name="targetElement">The target element.</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// 
        /// <returns>1x1, 2x2 or 3x3 matrix</returns>
        Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords);

        /// <summary>
        /// Gets the Rho matrix (mass property of material) at defined isometric coordinates in local coordination system.
        /// </summary>
        /// <remarks>
        /// This will be used for creating Mass matrix
        /// </remarks>
        /// <param name="targetElement">The target element.</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// 
        /// <returns>1x1, 2x2 or 3x3 matrix</returns>
        Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords);

        /// <summary>
        /// Gets the μ matrix (damp property of material) at defined isometric coordinates in local coordination system.
        /// </summary>
        /// <remarks>
        /// This will be used for creating Mass matrix
        /// </remarks>
        /// <param name="targetElement">The target element.</param>
        /// <param name="isoCoords">The isometric coordinations.</param>
        /// 
        /// <returns>1x1, 2x2 or 3x3 matrix</returns>
        Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords);

        /// <summary>
        /// Gets the N matrix at defined isometric coordinates.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// 
        /// <returns></returns>
        Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords);

        /// <summary>
        /// Gets the Jacobian matrix at defined isometric coordinates.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// 
        /// <returns>
        /// 1x1, 2x2 or 3x3 Jacobian matrix at defined location
        /// </returns>
        Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords);

        /// <summary>
        /// Gets the stiffness [K] matrix in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// 
        /// <returns>Stiffness matrix</returns>
        Matrix CalcLocalStiffnessMatrix(Element targetElement);

        /// <summary>
        /// Gets the mass [M] matrix in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// 
        /// <returns>Mass matrix</returns>
        Matrix CalcLocalMassMatrix(Element targetElement);

        /// <summary>
        /// Gets the damp [C] matrix in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// 
        /// <returns>Damp matrix</returns>
        Matrix CalcLocalDampMatrix(Element targetElement);

        /// <summary>
        /// Gets the DoF order of returned values.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <returns></returns>
        ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement);

        /// <summary>
        /// Gets the maximum degree of shape function members.
        /// </summary>
        /// <remarks>
        /// Will use for determining Gaussian sampling count
        /// </remarks>
        /// <param name="targetElement">The target element.</param>
        /// <returns>Maximum order</returns>
        int[] GetNMaxOrder(Element targetElement);

        /// <summary>
        /// Gets the maximum degree of B matrix members.
        /// </summary>
        /// <remarks>
        /// Will use for determining Gaussian sampling count
        /// </remarks>
        /// <param name="targetElement">The target element.</param>
        /// <returns>Gaussian sampling point count</returns>
        int[] GetBMaxOrder(Element targetElement);

        /// <summary>
        /// Gets the degree of Det(J).
        /// </summary>
        /// <remarks>
        /// Will use for determining Gaussian sampling count
        /// </remarks>
        /// <param name="targetElement">The target element.</param>
        /// <returns>Gaussian sampling point count</returns>
        int[] GetDetJOrder(Element targetElement);


        /// <summary>
        /// Gets the internal force at defined location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="localDisplacements"></param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// <param name="globalDisplacements">The local displacements on nodes.</param>
        /// <returns>Internal force at defined iso coordination</returns>
        [Obsolete("use GetLocalInternalStressAt instead")]
        IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] localDisplacements, params double[] isoCoords);

        /// <summary>
        /// Gets the internal stress at defined location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="localDisplacements"></param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// <param name="globalDisplacements">The local displacements on nodes.</param>
        /// <returns>Internal force at defined iso coordination</returns>
        GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords);

        /*
        /// <summary>
        /// Gets the internal force at defined location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="localDisplacements"></param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// <param name="globalDisplacements">The local displacements on nodes.</param>
        /// <returns>Internal stress at defined iso coordination</returns>
        /// 
        GeneralStressTensor GetLocalStressAt(Element targetElement,
            Displacement[] localDisplacements, params double[] isoCoords);
        */

        /// <summary>
        /// Gets the strain at defined location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="load">The load.</param>
        /// <param name="isoLocation">The iso location.</param>
        /// <returns>load internal force at defined iso coordination</returns>
        /// <summary>
        /// Gets the internal force of element, only due to applying specified load.
        /// </summary>
        /// <remarks>
        /// This gives back internal force of element assuming no nodal displacements there are, and only the <see cref="load"/> is applied to it.
        /// </remarks>
        IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load, double[] isoLocation);

        /// <summary>
        /// Gets the strain at defined location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="load">The load.</param>
        /// <param name="isoLocation">The iso location.</param>
        /// <returns>load internal force at defined iso coordination</returns>
        /// <summary>
        /// Gets the internal force of element, only due to applying specified load.
        /// </summary>
        /// <remarks>
        /// This gives back internal force of element assuming no nodal displacements there are, and only the <see cref="load"/> is applied to it.
        /// </remarks>
        GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation);

        /// <summary>
        /// Gets the displacement at specified location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="localDisplacements">The displacements in local coordination system (order is same with targetElement.Nodes).</param>
        /// <param name="isoCoords">The isometric coordinations (xi, eta, nu).</param>
        /// 
        /// <returns>
        /// Displacement of element at defined <see cref="isoCoords" /> in element's local coordination system
        /// </returns>
        Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords);

        /// <summary>
        /// Gets the displacement of element, only due to applying specified load. where all nodes are assumed as fixed
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="load">The load.</param>
        /// <param name="isoLocation">The iso location.</param>
        /// 
        /// <returns></returns>
        /// <remarks>
        /// This gives back the displacement of element, if no nodal displacements there are, and only the <see cref="load"/> is applied to it.
        /// </remarks>
        Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation);



        /// <summary>
        /// Gets the equivalent nodal loads because of applying <see cref="load" /> on the <see cref="targetElement" /> in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="load">The load.</param>
        /// <returns>The equivaled nodal load in local coordination system</returns>
        Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load);


        /// <summary>
        /// Adds the stiffness components into the coordinated storage regarding node numbers
        /// </summary>
        /// <remarks>
        /// Before calling this method, Model.ReindexNodex
        /// </remarks>
        /// <param name="global">the global stiffness matrix without any reduction, size: 6n*6n where n is node count</param>
        //void AddStiffnessComponents(CSparse.Storage.CoordinateStorage<double> global);
    }
}