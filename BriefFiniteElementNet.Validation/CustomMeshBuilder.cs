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

        public List<List<Point>> Grid = new List<List<Point>>();

        public List<int> Elbows = new List<int>();


        private bool Inited = false;


        public void AddSegment(Vector leader, double length, int rows)
        {
            var deltaW = Width / (WCount-1);


            if (!Inited)
            {
                var lst = new List<Point>();
                Grid.Add(lst);

                for (var j = 0; j < WCount; j++)
                {
                    var pt = P0 + (-Normal).GetUnit() * deltaW*j;

                    points.Add(pt);
                    lst.Add(pt);
                }

                Inited = true;
            }

            var nprim = Normal.Cross(leader);
            var leadPrim = nprim.Cross(Normal);
           


            for (var i = 1; i <= rows; i++)
            {
                var lst = new List<Point>();
                Grid.Add(lst);

                var r = (length * i)/(rows);

                var p0i = P0 + leadPrim.GetUnit()*r;

                for (int j = 0; j < WCount; j++)
                {
                    var pt = p0i + (-Normal).GetUnit()*deltaW*j;

                    points.Add(pt);
                    lst.Add(pt);
                }
            }

            P0 += leadPrim.GetUnit()*length;

            Elbows.Add(Grid.Count);
        }

        public void TriangularMesh(Action<Point, Point, Point> act)
        {
            for (var i = 0; i < Grid.Count - 1; i++)
            {
                var l1 = Grid[i];
                var l2 = Grid[i+1];

                for (int j = 0; j < l1.Count - 1; j++)
                {
                    var p1 = l1[j];
                    var p2 = l1[j+1];
                    var p3 = l2[j];
                    var p4 = l2[j+1];

                    act(p1, p2, p3);
                    act(p2, p3, p4);
                }
            }
        }


        public void Q4Mesh(Action<Point, Point, Point,Point> act)
        {
            for (var i = 0; i < Grid.Count - 1; i++)
            {
                var l1 = Grid[i];
                var l2 = Grid[i + 1];

                for (int j = 0; j < l1.Count - 1; j++)
                {
                    var p1 = l1[j];
                    var p2 = l1[j + 1];
                    var p3 = l2[j];
                    var p4 = l2[j + 1];

                    //act(p4, p3, p2, p1);
                    act(p1, p2, p3, p4);
                }
            }
        }
        public void FrameMeshEdges(Action<Point, Point> act)
        {
            for (var i = 0; i < Grid.Count - 1; i++)
            {
                var l1 = Grid[i];
                var l2 = Grid[i + 1];

                act(l1.First(), l2.First());
                act(l1.Last(), l2.Last());
            }
        }

        public void FrameMeshElbow(Action<Point, Point> act)
        {
            for (int i = 0; i < Elbows.Count-1; i++)
            {
                var elbow = Elbows[i];
                var lst = Grid[elbow-1];

                for (int j = 0; j < lst.Count - 1; j++)
                {
                    act(lst[j], lst[j + 1]);
                }
            }
        }


    }
}
