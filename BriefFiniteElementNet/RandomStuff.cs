using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class that provides random stuff like random force rtc.
    /// </summary>
    public static class RandomStuff
    {

        private static Random NumRnd = new Random(1);

        /// <summary>
        /// Gets a random number more than <see cref="min"/> and less than <see cref="max"/>.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static double GetRandomNumber(double min=0, double max=1)
        {
            if (min > max)
                throw new InvalidOperationException();

            return (max - min)*NumRnd.NextDouble() + min;

        }





        private static Random ForceRnd = new Random(1);

        /// <summary>
        /// Gets a random <see cref="Force"/>, every component is more than <see cref="min"/> and less than <see cref="max"/>.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static Force GetRandomForce(double min, double max)
        {
            if (min > max)
                throw new InvalidOperationException();

            var buf = new Force(
                (max - min)*ForceRnd.NextDouble() + min,
                (max - min)*ForceRnd.NextDouble() + min,
                (max - min)*ForceRnd.NextDouble() + min,
                (max - min)*ForceRnd.NextDouble() + min,
                (max - min)*ForceRnd.NextDouble() + min,
                (max - min)*ForceRnd.NextDouble() + min);


            return buf;
        }



        public static Random DispsRnd = new Random();

        /// <summary>
        /// Gets a random <see cref="Displacement"/>, every component is more than <see cref="min"/> and less than <see cref="max"/>.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static Displacement GetRandomDisplacement(double min, double max)
        {
            if (min > max)
                throw new InvalidOperationException();

            var buf = new Displacement(
                (max - min) * DispsRnd.NextDouble() + min,
                (max - min) * DispsRnd.NextDouble() + min,
                (max - min) * DispsRnd.NextDouble() + min,
                (max - min) * DispsRnd.NextDouble() + min,
                (max - min) * DispsRnd.NextDouble() + min,
                (max - min) * DispsRnd.NextDouble() + min);


            return buf;
        }


        public static Random ConstraintRnd = new Random();

        public static Constraint GetRandomConstraint()
        {

            var bits = new char[]
            {
                ConstraintRnd.Next().GetHashCode()%2 == 1 ? '1' : '0', 
                ConstraintRnd.Next().GetHashCode()%2 == 1 ? '1' : '0',
                ConstraintRnd.Next().GetHashCode()%2 == 1 ? '1' : '0',

                ConstraintRnd.Next().GetHashCode()%2 == 1 ? '1' : '0',
                ConstraintRnd.Next().GetHashCode()%2 == 1 ? '1' : '0',
                ConstraintRnd.Next().GetHashCode()%2 == 1 ? '1' : '0'
            };

            var id = Guid.NewGuid();


            var buf = new Constraint(
                bits[0] == '0' ? DofConstraint.Released : DofConstraint.Fixed,
                bits[1] == '0' ? DofConstraint.Released : DofConstraint.Fixed,
                bits[2] == '0' ? DofConstraint.Released : DofConstraint.Fixed,
                bits[3] == '0' ? DofConstraint.Released : DofConstraint.Fixed,
                bits[4] == '0' ? DofConstraint.Released : DofConstraint.Fixed,
                bits[5] == '0' ? DofConstraint.Released : DofConstraint.Fixed);

            return buf;
        }


        public static Model GetRandomFrameModel(int n = 50)
        {
            var buf = new Model();

            var nodes = new Node[n];

            for (var i = 0; i < n; i++)
            {
                nodes[i] = new Node(GetRandomNumber(0, 10), GetRandomNumber(0, 10), GetRandomNumber(0, 10));

                nodes[i].Constraints = GetRandomConstraint();
            }

            buf.Nodes.Add(nodes);

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (i == j)
                        continue;

                    var elm = new FrameElement2Node(nodes[i], nodes[j]);
                    elm.A = GetRandomNumber();
                    elm.Iy = GetRandomNumber();
                    elm.Iz = GetRandomNumber();
                    elm.J = GetRandomNumber();

                    elm.E = GetRandomNumber();
                    elm.G = GetRandomNumber();

                    var ld = new UniformLoad1D(GetRandomNumber(100, 1000), LoadDirection.Z, CoordinationSystem.Global);

                    elm.Loads.Add(ld);

                    buf.Elements.Add(elm);
                }
            }

            for (int i = 0; i < n; i++)
            {
                nodes[i].Loads.Add(new NodalLoad(GetRandomForce(1000, 10000)));
            }

            return buf;
        }
    }
}
