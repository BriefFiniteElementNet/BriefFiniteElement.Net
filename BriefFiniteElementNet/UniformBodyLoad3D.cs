using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a uniformly distributed load on a whole of 3d <see cref="Element"/>'s body.
    /// </summary>
    [Serializable]
    public class UniformBodyLoad3D : Load3D
    {
        private double _vx, _vy, _vz;

        /// <summary>
        /// Gets or sets the Z component of this load.
        /// </summary>
        /// <value>
        /// The Z component of distributed body load in N/m^3 dimension.
        /// This is in global coordination system not element's local coordination system.
        /// </value>
        public double Vz
        {
            get { return _vz; }
            set { _vz = value; }
        }

        /// <summary>
        /// Gets or sets the Y component of this load.
        /// </summary>
        /// <value>
        /// The Y component of distributed body load in N/m^3 dimension.
        /// This is in global coordination system not element's local coordination system.
        /// </value>
        public double Vy
        {
            get { return _vy; }
            set { _vy = value; }
        }

        /// <summary>
        /// Gets or sets the X component of this load.
        /// </summary>
        /// <value>
        /// The X component of distributed body load in N/m^3 dimension.
        /// This is in global coordination system not element's local coordination system.
        /// </value>
        public double Vx
        {
            get { return _vx; }
            set { _vx = value; }
        }

        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            switch (element.ElementType)
            {
                case ElementType.Undefined:
                    break;
                case ElementType.FrameElement2Node:
                    break;
                case ElementType.TrussElement2Noded:
                    break;
                case ElementType.TetrahedralIso:
                    return (element as Tetrahedral).GetEquivalentNodalLoads(this);
                    break;
                case ElementType.Dkt:
                    return GetGlobalEquivalentNodalLoads(element as DktElement);
                default:
                    element.GetEquivalentNodalLoads(this);
                    throw new ArgumentOutOfRangeException();
            }

            return element.GetEquivalentNodalLoads(this);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the global equivalent nodal loads for <see cref="Tetrahedral"/> element.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <returns>global equivalent nodal loads for <see cref="Tetrahedral"/> element</returns>
        private Force[] GetGlobalEquivalentNodalLoads(Tetrahedral elm)
        {
            //p.263 of Structural Analysis with the Finite Element Method Linear Statics, ISBN: 978-1-4020-8733-2
            //formula (8.37c) : the total body force is distributed in equal parts between the four nodes, as expected!

            elm.UpdateGeoMatrix();

            var v = elm.det/6;

            var f = new Force();
            
            f.Fx = v / 4 * _vx;
            f.Fy = v / 4 * _vy;
            f.Fz = v / 4 * _vz;

            return new[] {f, f, f, f};
        }

        private Force[] GetGlobalEquivalentNodalLoads(DktElement elm)
        {
            //using lumped method.

            var area = elm.GetArea();

            var totv = area*elm.Thickness;

            var totfx = _vx*totv;
            var totfy = _vy*totv;
            var totfz = _vz*totv;

            var f = new Force(totfx/3, totfy/3, totfz/3, 0, 0, 0);

            return new Force[] {f, f, f};
        }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_vx", _vx);
            info.AddValue("_vy", _vy);
            info.AddValue("_vz", _vz);

            base.GetObjectData(info, context);
        }

        /// <inheritdoc/>
        protected UniformBodyLoad3D(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _vx = info.GetDouble("_vx");
            _vy = info.GetDouble("_vy");
            _vz = info.GetDouble("_vz");
        }

        public UniformBodyLoad3D()
        {
            
        }
    }
}
