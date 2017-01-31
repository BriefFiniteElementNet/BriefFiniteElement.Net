using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BriefFiniteElementNet.Geometry;

namespace BriefFiniteElementNet.Elements
{
    public static class TriangleFlatShellExtensions
    {
        [Obsolete]
        /// <summary>
        /// Gets the N points, equally spaced and lying of intersection line between current triangular flat shell and defined <see cref="plane" />.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="plane">The plane.</param>
        /// <param name="n">The n.</param>
        /// <returns>
        /// point list
        /// </returns>
        /// <remarks>
        /// Assume a flat plane in 3D, if the flat plane intersects with this triangular flat shell, then a line segment is common part of the flat shell and plane.
        /// this will equally divide that line section into n parts, then return the coordination of them
        /// </remarks>
        public static Point[] GetIntersectionPoints(this TriangleFlatShell element, Plane plane, int n)
        {
            var buf = new List<Point>();

            //http://geomalgorithms.com/a06-_intersect-2.html section "Intersection of a Triangle with a Plane"

            //http://stackoverflow.com/questions/15688232/check-which-side-of-a-plane-points-are-on

            //if this intersects with plane, then points must not be in same side of plane

            var d = -Vector.Dot(plane.Normal, (Vector)plane.P);

            var ps = element.Nodes.Select(i => i.Location).ToArray();

            var signs =
                ps.Select(i => Vector.Dot((Vector)i, plane.Normal) + d).Select(i => i < 0.0 ? -1 : (i > 0 ? 1 : 0)).ToArray();


            if (signs.Any(i => i.Equals(0)))
                return buf.ToArray();


            if (signs.Distinct().Count() == 1)
                //all points on same side of plane
                return buf.ToArray();


            //find each pair that are on opposite side of plane
            for (var i = 0; i < 3; i++)
            {
                for (var j = i; j < 3; j++)
                {
                    if (i == j)
                        continue;

                    if (signs[i] != signs[j])
                    {
                        //points i & j are on opposite sides of plane

                        {
                            //eqs from here: hxxps://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
                            var l = ps[i] - ps[j];
                            var l0 = ps[j];

                            var N = plane.Normal;
                            var p0 = plane.P;

                            var D = (p0 - l0).Dot(N) / (l).Dot(N);

                            var p = l0 + D * l;//intersection point

                            buf.Add(p);
                        }
                    }
                }
            }


            var sp = buf[0];
            var ep = buf[1];

            var buff = new List<Point>();

            n--;

            for (var i = 0; i < n+1; i++)
            {
                var t = sp + (ep - sp)*(1.0*i/n);
                buff.Add(t);
            }


            return buff.ToArray();
        }

        [Obsolete]
        public static FlatShellStressTensor[] GeTensorsAtIntersection(this TriangleFlatShell element, Plane plane, int n)
        {
            var buf = new List<FlatShellStressTensor>();

            var globalPoints = GetIntersectionPoints(element, plane, n);

            if (!globalPoints.Any())
                return buf.ToArray();

            var t = element.GetTransformationMatrix().Transpose(); //transpose of t

            var locals = globalPoints.Select(i => (t*i.ToMatrix()).ToPoint()).ToArray();

            var lpts = element.GetLocalPoints();

            var tensors =
                locals.Select(i => element.GetInternalForce(i.X, i.Y, LoadCombination.DefaultLoadCombination)).ToArray();


            return tensors;
        }


        [Obsolete]
        public static Tuple<Force,Point> GetTotalForce(this TriangleFlatShell element, Plane plane, int n)
        {
            //step 1: find intersection points
            //step 2: find tensors at intersection points
            //step 3: find rotation amount for tensors
            //step 4: rotate tensors
            //step 5: integrate forces

            //step 1
            var globalPoints = GetIntersectionPoints(element, plane, n);

            if (!globalPoints.Any())
                return new Tuple<Force, Point>();

            var t = element.GetTransformationMatrix().Transpose(); //transpose of t
            var locals = globalPoints.Select(i => (t * i.ToMatrix()).ToPoint()).ToArray();

            //step 2
            var tensors = GeTensorsAtIntersection(element, plane, n);
            var memTensors = tensors.Select(i => i.MembraneTensor).ToList();
            var bendTensors = tensors.Select(i => i.BendingTensor).ToList();

            //step 3
            var nLocal = (Vector)(t * plane.Normal.ToMatrix()).ToPoint();//project of N to local element coord system

            var theta = Math.Atan2(nLocal.Y, nLocal.X) - Math.PI/2;

            //step 4
            var rTensors = tensors.Select(i => MembraneStressTensor.Rotate(i.MembraneTensor, theta)).ToList();

            var avg = rTensors.Aggregate((i, j) => i + j);

            avg = MembraneStressTensor.Multiply(avg, 1.0 / tensors.Length);

            var length = (globalPoints.First() - globalPoints.Last()).Length;

            var fShearAmount = avg.Txy * element.Thickness * length;//shear force
            var fCompAmount = avg.Sy * element.Thickness * length;//compressive force

            // shear dirction
            var shearLocalDirection = new Vector(Math.Cos(theta), Math.Sin(theta), 0);//direction of shear force in local element coord system, which is vector I rotated by amount theta
            var shearGlobalDirection = (Vector) (t.Transpose()*shearLocalDirection.ToMatrix()).ToPoint();

            //compressive direction
            var compLocalDirection = new Vector(-Math.Sin(theta), Math.Cos(theta), 0);//direction of shear force in local element coord system, which is vector I rotated by amount theta
            var compGlobalDirection = (Vector)(t.Transpose() * compLocalDirection.ToMatrix()).ToPoint();


            var shearForce = fShearAmount*shearGlobalDirection.GetUnit();
            var compForce = fCompAmount*compGlobalDirection.GetUnit();

            var totForce = shearForce + compForce;

            var bufFrc = new Force(totForce, Vector.Zero);
            var loc = globalPoints.Aggregate((i, j) => (Point)((Vector)i + (Vector)j));

            loc.X /= globalPoints.Length;
            loc.Y /= globalPoints.Length;
            loc.Z /= globalPoints.Length;

            return new Tuple<Force, Point>(bufFrc, loc);
            //return bufFrc;
        }


        /// <summary>
        /// Determines whether the defined plane does intersect with defined triangle shell element or not.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="plane">The plane.</param>
        /// <returns>true, if intersects, false otherwise</returns>
        public static bool DoesIntersects(this TriangleFlatShell element, Plane plane)
        {
            //http://geomalgorithms.com/a06-_intersect-2.html section "Intersection of a Triangle with a Plane"

            //http://stackoverflow.com/questions/15688232/check-which-side-of-a-plane-points-are-on

            //if this intersects with plane, then points must not be in same side of plane

            var d = -Vector.Dot(plane.Normal, (Vector)plane.P);

            var ps = element.Nodes.Select(i => i.Location).ToArray();

            var signs =
                ps.Select(i => Vector.Dot((Vector)i, plane.Normal) + d).Select(i => i < 0.0 ? -1 : (i > 0 ? 1 : 0)).ToArray();

            if (signs.Any(i => i.Equals(0)))
                return false;

            if (signs.Distinct().Count() == 1)
                //all points on same side of plane
                return false;

            return true;
        }

        /// <summary>
        /// Gets the intersection of triangle and plane as a line segment
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="plane">The plane.</param>
        /// <returns>intersection</returns>
        public static LineSegment GetIntersection(this TriangleFlatShell element, Plane plane)
        {
            if (!DoesIntersects(element, plane))
                throw new InvalidOperationException();

            var points = GetIntersectionPoints(element, plane, 5);

            var buf = new LineSegment();

            buf.P1 = points.First();
            buf.P2 = points.Last();

            return buf;
        }

        /// <summary>
        /// Integrates the shear force along intersection of element with defined plane.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="plane">The plane.</param>
        /// <returns>Integrated force</returns>
        public static Force IntegrateShearForce(this TriangleFlatShell element, Plane plane,LoadCombination cmb)
        {
            //step 1: does intersect?
            //step 2: get a tensor (no matter where, this is constant stress)
            //step 3: find rotation amount for tensors
            //step 4: rotate tensors
            //step 5: integrate forces

            if (!DoesIntersects(element, plane))
                return Force.Zero;

            var intersection = GetIntersection(element, plane);

            //step 2
            var tensor = element.GetInternalForce(0, 0, cmb);

            //step 3
            var t = element.GetTransformationMatrix().Transpose(); //transpose of t
            var nLocal = (Vector)(t * plane.Normal.ToMatrix()).ToPoint();//project of N to local element coord system
            var theta = Math.Atan2(nLocal.Y, nLocal.X) - Math.PI / 2;

            //step 4
            var rotatedTensor = MembraneStressTensor.Rotate(tensor.MembraneTensor, theta);

            // shear direction
            var shearLocalDirection = new Vector(Math.Cos(theta), Math.Sin(theta), 0);//direction of shear force in local element coord system, which is vector I rotated by amount theta
            var shearGlobalDirection = (Vector)(t.Transpose() * shearLocalDirection.ToMatrix()).ToPoint();

            var fShearAmount = rotatedTensor.Txy * element.Thickness * intersection.Length;//shear force

            var shearForce = fShearAmount * shearGlobalDirection.GetUnit();

            return new Force(shearForce, Vector.Zero);
        }

        /// <summary>
        /// Integrates the shear force along intersection of element with defined plane.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="plane">The plane.</param>
        /// <returns>Integrated force</returns>
        public static Force IntegrateNormalForce(this TriangleFlatShell element, Plane plane, LoadCombination cmb)
        {
            //step 1: does intersect?
            //step 2: get a tensor (no matter where, this is constant stress)
            //step 3: find rotation amount for tensors
            //step 4: rotate tensors
            //step 5: integrate forces

            if (!DoesIntersects(element, plane))
                return Force.Zero;

            var intersection = GetIntersection(element, plane);

            //step 2
            var tensor = element.GetInternalForce(0, 0, cmb);

            //step 3
            var t = element.GetTransformationMatrix().Transpose(); //transpose of t
            var nLocal = (Vector)(t * plane.Normal.ToMatrix()).ToPoint();//project of N to local element coord system
            var theta = Math.Atan2(nLocal.Y, nLocal.X) - Math.PI / 2;

            //step 4
            var rotatedTensor = MembraneStressTensor.Rotate(tensor.MembraneTensor, theta);

            // shear direction
            var shearLocalDirection = new Vector(Math.Cos(theta), Math.Sin(theta), 0);//direction of shear force in local element coord system, which is vector I rotated by amount theta
            var shearGlobalDirection = (Vector)(t.Transpose() * shearLocalDirection.ToMatrix()).ToPoint();

            var fShearAmount = rotatedTensor.Txy * element.Thickness * intersection.Length;//shear force

            var shearForce = fShearAmount * shearGlobalDirection.GetUnit();

            return new Force(shearForce, Vector.Zero);
        }

        [Obsolete]
        public static Force IntegrateForceOverIntersection(Model mdl, Plane pl)
        {
            var buf = new Force();

            foreach (var elm in mdl.Elements)
            {
                var e = elm as TriangleFlatShell;

                if (e == null)
                    continue;

                var frc = e.GetTotalForce(pl, 5);

                var movedF = frc.Item1.Move(frc.Item2, Point.Origins);

                buf += movedF;
            }

            return buf;
        }

        [Obsolete]
        public class Tuple<T1,T2>
        {
            public T1 Item1;
            public T2 Item2;

            public Tuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }

            public Tuple()
            {
            }
        }


        /// <summary>
        /// Rotates the define local stress tensor into direction of plane.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="localStressTensor">The local stress tensor.</param>
        /// <param name="targetPlane">The target plane.</param>
        /// <returns>
        /// Rotated tensor
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static MembraneStressTensor RotateTensor(this TriangleFlatShell shell, MembraneStressTensor localStressTensor, Plane targetPlane)
        {
            var t = shell.GetTransformationMatrix().Transpose();

            var plane = targetPlane;

            var nLocal = (Vector)(t * plane.Normal.ToMatrix()).ToPoint();//project of N to local element coord system

            var theta = Math.Atan2(nLocal.Y, nLocal.X) - Math.PI / 2;

            var buf = MembraneStressTensor.Rotate(localStressTensor, theta);

            return buf;
        }

    }
}
