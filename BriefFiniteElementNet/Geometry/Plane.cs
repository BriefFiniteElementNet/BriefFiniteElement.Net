namespace BriefFiniteElementNet.Geometry
{
    /// <summary>
    /// Represents a flat plane in 3D space
    /// </summary>
    public struct Plane
    {
        /// <summary>
        /// The normal vector of plane
        /// </summary>
        public Vector Normal;

        /// <summary>
        /// A points which exists on the surface of plane
        /// </summary>
        public Point P;

        /// <summary>
        /// Moves the Plane with specified amount.
        /// </summary>
        /// <param name="d">The amount of movement.</param>
        /// <returns>Moved plane</returns>
        public Plane Move(Vector d)
        {
            var buf = new Plane {Normal = this.Normal, P = this.P + d};

            return buf;
        }

        /// <summary>
        /// Creates a plane from a point and normal vector.
        /// </summary>
        /// <param name="normal">The normal vector.</param>
        /// <param name="point">The point.</param>
        /// <returns>New plane</returns>
        public static Plane FromPointAndNormal(Vector normal, Point point)
        {
            return new Plane() {Normal = normal, P = point};
        }

        /// <summary>
        /// Creates a plane from 3 points.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns>New plane</returns>
        public static Plane FromThreePoints(Point p1, Point p2, Point p3)
        {
            var v1 = p3 - p1;
            var v2 = p3 - p2;
            var n = Vector.Cross(v1, v2);
            return FromPointAndNormal(n, p1);
        }


        /// <summary>
        /// The XY plane with Z = 0
        /// </summary>
        public static Plane XYPlane = FromPointAndNormal(Vector.K, Point.Origins);

        /// <summary>
        /// The XZ plane with Y = 0
        /// </summary>
        public static Plane XZPlane = FromPointAndNormal(Vector.J, Point.Origins);

        /// <summary>
        /// The YZ plane with X = 0
        /// </summary>
        public static Plane YZPlane = FromPointAndNormal(Vector.I, Point.Origins);

    }
}
