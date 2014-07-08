using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    public class UniformLoad1D : Load1D
    {
        #region Members

        private double magnitude;

        /// <summary>
        /// Gets or sets the magnitude.
        /// </summary>
        /// <remarks>
        /// Value is magnitude of distributed load, the unit is [Force/Length]
        /// </remarks>
        /// <value>
        /// The magnitude of distributed load along the member.
        /// </value>
        public double Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }

        #endregion

        public override Force[] GetEquivalentNodalLoads(Element element)
        {

            
            if (element is FrameElement2Node)
            {
                var frElm = element as FrameElement2Node;

                var l = (frElm.EndNode.Location - frElm.StartNode.Location).Length;

                var w = GetLocalDistributedLoad(element as Element1D);

                var localEndForces = new Force[2];

                if (frElm.HingedAtEnd & frElm.HingedAtStart)
                {
                    localEndForces[0] = new Force(w.X*l/2, w.Y*l/2, w.Z*l/2, 0, 0, 0);
                    localEndForces[1] = new Force(w.X*l/2, w.Y*l/2, w.Z*l/2, 0, 0, 0);
                }
                else if (!frElm.HingedAtEnd & frElm.HingedAtStart)
                {

                }
                else if (frElm.HingedAtEnd & !frElm.HingedAtStart)
                {

                }
                else if (!frElm.HingedAtEnd & !frElm.HingedAtStart)
                {
                    localEndForces[0] = new Force(w.X * l / 2, w.Y * l / 2, w.Z * l / 2, 0, -w.Z * l * l / 12.0, w.Y * l * l / 12.0);
                    localEndForces[1] = new Force(w.X * l / 2, w.Y * l / 2, w.Z * l / 2, 0, w.Z * l * l / 12.0, -w.Y * l * l / 12.0);
                }


                for (var i = 0; i < element.Nodes.Length; i++)
                {
                    var frc = localEndForces[i];
                    localEndForces[i] = new Force(frElm.TransformLocalToGlobal(frc.Forces), frElm.TransformLocalToGlobal(frc.Moments));
                }

                return localEndForces;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the local distributed load.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <returns></returns>
        /// <remarks>
        /// Gets a vector that its components shows Wx, Wy and Wz in local coordination system of <see cref="elm"/>
        /// </remarks>
        private Vector GetLocalDistributedLoad(Element1D elm)
        {
            if (coordinationSystem == CoordinationSystem.Local)
            {
                return new Vector(
                        direction == Direction.X ? this.magnitude : 0,
                        direction == Direction.Y ? this.magnitude : 0,
                        direction == Direction.Z ? this.magnitude : 0);
            }

            if (elm is FrameElement2Node)
            {
                var frElm = elm as FrameElement2Node;

                var w = new Vector();


                var globalVc = new Vector(
                    direction == Direction.X ? this.magnitude : 0,
                    direction == Direction.Y ? this.magnitude : 0,
                    direction == Direction.Z ? this.magnitude : 0);

                w = frElm.TransformGlobalToLocal(globalVc);

                return w;
            }

            throw new NotImplementedException();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            if (elm is FrameElement2Node)
            {
                var frElm = elm as FrameElement2Node;

                var l = (frElm.EndNode.Location - frElm.StartNode.Location).Length;
                var w = GetLocalDistributedLoad(elm);

                var f1 = -GetEquivalentNodalLoads(elm)[0];
                var f2 = new Force(new Vector(w.X * x, w.Y * x, w.Z * x), new Vector());

                var buf = f1.Move(new Point(0, 0, 0), new Point(x, 0, 0)) +
                              f2.Move(new Point(x/2, 0, 0), new Point(x, 0, 0));

                return -buf;
            }

            throw new NotImplementedException();
        }


        internal UniformLoad1D(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }


        public UniformLoad1D(double magnitude,Direction direction,CoordinationSystem sys)
        {
            this.magnitude = magnitude;
            this.coordinationSystem = sys;
            this.direction = direction;
        }

        public UniformLoad1D()
        {
        }
    }
}
