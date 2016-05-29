using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using BriefFiniteElementNet.Elements;
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

        public static void AddQuad(this MeshBuilder bulder, Point p1, Point p2, Point p3,Point p4)
        {
            bulder.AddQuad(p1.ToPoint3D(), p2.ToPoint3D(), p3.ToPoint3D(), p4.ToPoint3D());
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



        public static void ShowInternalForce(this Model model)
        {
            ModelInternalForceVisualizer.VisualizeInNewWindow(model);
        }


        /// <summary>
        /// Visualizes the specified <see cref="function"/> in speifid interval and <see cref="samplingCount"/>.
        /// </summary>
        /// <param name="function">The function to be visualized.</param>
        /// <param name="min">The minimum of interval that function should be visualized.</param>
        /// <param name="max">The maximum of interval that function should be visualized.</param>
        /// <param name="samplingCount">The sampling count in defined inerval.</param>
        /// <param name="verticalValueLabel">The vertical value label.</param>
        public static void Show(this Func<double, double> function, double min, double max, int samplingCount = 10,
            string verticalValueLabel = null)
        {

            var ctrl = new FunctionVisualizer();

            ctrl.TargetFunction = function;
            ctrl.Min = min;
            ctrl.Max = max;
            ctrl.SamplingCount = samplingCount;
            ctrl.VerticalAxisLabel = verticalValueLabel;

            var wnd = new Window();
            wnd.Title = string.Format("Visualizing {0}", verticalValueLabel);
            wnd.Content = ctrl;
            wnd.ShowDialog();
        }


        public static double GetElementLength(this Element elm)
        {
            if (elm.Nodes.Length == 2)
                return (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            return 0.0;
        }
    }
}
