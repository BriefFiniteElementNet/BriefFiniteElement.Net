using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Loads
{
    [Obsolete("still in development")]
    public class UniformLoad:Load
    {
        private Vector _direction;
        private double _magnitude;

        /// <summary>
        /// Sets or gets the direction of load
        /// </summary>
        public Vector Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Sets or gets the magnitude of load
        /// </summary>
        public double Magnitude
        {
            get { return _magnitude; }
            set { _magnitude = value; }
        }


        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            throw new NotImplementedException();
        }
    }
}
