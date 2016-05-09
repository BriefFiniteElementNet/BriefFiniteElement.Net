using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class CustomMeshBuilder
    {
        public int WCount;
        public double Width;

        public Point P0;
        public Vector Normal;

        public List<Point> points = new List<Point>();
        private bool Inited = false;


        public void AddSegment(Vector leader, double length, int rows)
        {
            var deltaW = Width / WCount;


            if (!Inited)
            {
                for (int j = 0; j < WCount; j++)
                {
                    var pt = P0 + (-Normal).GetUnit() * deltaW*j;

                    points.Add(pt);
                }

                Inited = true;
            }

            var nprim = Normal.Cross(leader);
            var leadPrim = nprim.Cross(Normal);
           


            for (int i = 1; i <= rows; i++)
            {
                var r = (length * i)/(rows);

                var p0i = P0 + leadPrim.GetUnit()*r;

                for (int j = 0; j < WCount; j++)
                {
                    var pt = p0i + (-Normal).GetUnit()*deltaW*j;

                    points.Add(pt);
                }
            }

            P0 += leadPrim.GetUnit()*length;

        }
    }
}
