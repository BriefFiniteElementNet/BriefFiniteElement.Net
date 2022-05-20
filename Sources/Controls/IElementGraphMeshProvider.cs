using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Represents an interface for showing an element into the UI by triangles
    /// </summary>
    public interface IElementGraphMeshProvider
    {
        bool CanProvideMeshForElement(Element elm);

        List<Tuple<int, int, int>> ProvideTriangleMeshForElement(Element elm);
    }
}
