using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a static class for fast generating of sections
    /// </summary>
    public static class SectionGenerator
    {

        /// <summary>
        /// Gets a rectangular section.
        /// </summary>
        /// <param name="h">The height of section.</param>
        /// <param name="w">The width of section.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static PolygonYz GetRectangularSection(double h, double w)
        {
            var buf = new PolygonYz(
                new PointYZ(-w/2, -h/2),
                new PointYZ(-w/2, h/2),
                new PointYZ(w/2, h/2),
                new PointYZ(w/2, -h/2),
                new PointYZ(-w/2, -h/2));

            return buf;
        }

        /// <summary>
        /// Gets an I section with defined parameters.
        /// </summary>
        /// <param name="w">The overall width of section.</param>
        /// <param name="h">The overall height of section.</param>
        /// <param name="tf">The thickness of flanges of section.</param>
        /// <param name="tw">The thickness of web of section.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static PolygonYz GetISetion(double w, double h, double tf, double tw)
        {
            var buf = new PolygonYz(
                new PointYZ(-w/2, -h/2),
                new PointYZ(-w/2, -h/2 + tf),
                new PointYZ(-tw/2, -h/2 + tf),
                new PointYZ(-tw/2, h/2 - tf),
                new PointYZ(-w/2, h/2 - tf),
                new PointYZ(-w/2, h/2),

                new PointYZ(w/2, h/2),
                new PointYZ(w/2, h/2 - tf),
                new PointYZ(tw/2, h/2 - tf), 
                new PointYZ(tw/2, -h/2 + tf),
                new PointYZ(w/2, -h/2 + tf),
                new PointYZ(w/2, -h/2),

                new PointYZ(-w/2, -h/2)
                );


            return buf;
        }
    }
}
