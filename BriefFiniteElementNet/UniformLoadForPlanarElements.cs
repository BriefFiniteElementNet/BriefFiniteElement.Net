using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Security.Permissions;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a uniform load for shells
    /// </summary>
    [Serializable]
    public class UniformLoadForPlanarElements : Load
    {
        public UniformLoadForPlanarElements()
        {
        }

        public UniformLoadForPlanarElements(double ux, double uy, double uz, LoadCase lc)
            : base(lc)
        {
            _ux = ux;
            _uy = uy;
            _uz = uz;
        }

        public UniformLoadForPlanarElements(double ux, double uy, double uz)
            : base(LoadCase.DefaultLoadCase)
        {
            _ux = ux;
            _uy = uy;
            _uz = uz;
        }

        protected UniformLoadForPlanarElements(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _ux = info.GetDouble("_ux");
            _uy = info.GetDouble("_uy");
            _uz = info.GetDouble("_uz");
            _coordinationSystem = (CoordinationSystem) info.GetInt32("_coordinationSystem");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_ux", _ux);
            info.AddValue("_uy", _uy);
            info.AddValue("_uz", _uz);
            info.AddValue("_coordinationSystem", (int) _coordinationSystem);

            base.GetObjectData(info, context);
        }

        private double _ux;
        private double _uy;
        private double _uz;
        private CoordinationSystem _coordinationSystem;

        /// <inheritdoc />
        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            //var buf = new Force[3];

            return element.GetEquivalentNodalLoads(this);

            //return buf;
        }

        public Force[] GetGlobalEquivalentNodalLoads(DktElement element)
        {
            var a = (element as DktElement).GetArea();

            Vector local = Vector.Zero;


            if (CoordinationSystem == CoordinationSystem.Global)
            {
                var global = new Vector(_ux, _uy, _uz);

                local = element.TranformGlobalToLocal(global);
            }
            else
            {
                local = new Vector(_ux, _uy, _uz);
            }

            local.X = local.Y = 0;//dkt only local Uz

            var glob = element.TranformLocalToGlobal(local);

            var buf = new Force[]
            {new Force(glob*a/3, Vector.Zero), new Force(glob*a/3, Vector.Zero), new Force(glob*a/3, Vector.Zero)};

            return buf;
        }


        public Force[] GetGlobalEquivalentNodalLoads(CstElement element)
        {
            var a = (element as CstElement).GetArea();

            Vector local = Vector.Zero;

            if (CoordinationSystem == CoordinationSystem.Global)
            {
                var global = new Vector(_ux, _uy, _uz);

                local = element.TranformGlobalToLocal(global);
            }
            else
            {
                local = new Vector(_ux, _uy, _uz);
            }

            local.Z = 0;//CST only local Ux and Uy

            var glob = element.TranformLocalToGlobal(local);

            var buf = new Force[] { new Force(glob * a / 3, Vector.Zero), new Force(glob * a / 3, Vector.Zero), new Force(glob * a / 3, Vector.Zero) };

            return buf;
        }
        /// <summary>
        /// Gets or sets the uniform area load in X direction.
        /// </summary>
        /// <value>
        /// The uniform area load in X direction.
        /// </value>
        public double Ux
        {
            get { return _ux; }
            set { _ux = value; }
        }

        /// <summary>
        /// Gets or sets the uniform area load in X direction.
        /// </summary>
        /// <value>
        /// The uniform area load in X direction.
        /// </value>
        public double Uy
        {
            get { return _uy; }
            set { _uy = value; }
        }

        /// <summary>
        /// Gets or sets the uniform area load in X direction.
        /// </summary>
        /// <value>
        /// The uniform area load in X direction.
        /// </value>
        public double Uz
        {
            get { return _uz; }
            set { _uz = value; }
        }

        /// <summary>
        /// Gets or sets the coordination system which this load targets.
        /// </summary>
        /// <value>
        /// The coordination system.
        /// </value>
        /// <remarks>Take a look at local coordination system of DKT element</remarks>
        public CoordinationSystem CoordinationSystem
        {
            get { return _coordinationSystem; }
            set { _coordinationSystem = value; }
        }
    }
}
