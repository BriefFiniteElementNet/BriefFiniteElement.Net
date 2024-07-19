namespace BriefFiniteElementNet
{
    internal static class GeometryUtils
    {


        public static double GetTriangleArea(Point p0, Point p1, Point p2)
        {
            var v1 = p1 - p0;
            var v2 = p2 - p0;

            var cross = Vector.Cross(v1, v2);
            return cross.Length / 2;
        }
    }
}