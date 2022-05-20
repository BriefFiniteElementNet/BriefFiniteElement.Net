using System;

namespace BriefFiniteElementNet.Geometry
{
    /// <summary>
    /// Represents a line segment in 3d space
    /// </summary>
    public struct LineSegment
    {
       

        public LineSegment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

       
        private Point p1;

       
        private Point p2;


        /// <summary>
        /// Determines whether specified plane does lie on, or parallel to this line segment or not.
        /// </summary>
        /// <param name="plane">The plane.</param>
        public bool IsParallel(Plane plane)
        {
            var v = P2 - P1;

            var nv = Vector.Dot(v.GetUnit(), plane.Normal.GetUnit());

            var thre = 1e-6;

            return nv < thre;
        }

        public double Length
        {
            get
            {
                return (P2 - P1).Length;
            }
        }

        /// <summary>
        /// The p1, starting point
        /// </summary>
        public Point P1
        {
            get { return p1; }
            set
            {
                p1 = value;
            }
        }

        /// <summary>
        /// The p2, ending point
        /// </summary>
        public Point P2
        {
            get { return p2; }
            set
            {
                p2 = value;
            }
        }
    }
}
