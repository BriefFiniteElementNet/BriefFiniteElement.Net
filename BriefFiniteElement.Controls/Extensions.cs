using System;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts <see cref="BriefFiniteElementNet.Point"/> to <see cref="System.Windows.Media.Media3D.Point3D"/>
        /// </summary>
        /// <param name="pt">input <see cref="BriefFiniteElementNet.Point"/>.</param>
        /// <returns><see cref="System.Windows.Media.Media3D.Point3D"/> equivalent to <see cref="pt"/></returns>
        public static Point3D ToPoint3D(this Point pt)
        {
            return new Point3D(pt.X, pt.Y, pt.Z);
        }



        /// <summary>
        /// Adds the triangle.
        /// </summary>
        /// <param name="bulder">The bulder.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        public static void AddTriangle(this MeshBuilder bulder, Point p1, Point p2, Point p3)
        {
            bulder.AddTriangle(p1.ToPoint3D(), p2.ToPoint3D(), p3.ToPoint3D());
        }



        /// <summary>
        /// Visualizes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public static void Show(this Model model)
        {
            var wnd = new Window();
            var ctrl = new ModelVisualizerControl();
            ctrl.ModelToVisualize = model;

            wnd.Content = ctrl;
            wnd.ShowDialog();

        }
    }
}
