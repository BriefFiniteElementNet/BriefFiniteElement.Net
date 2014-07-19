using System;
using System.Reflection;
using System.Windows;
using BriefFiniteElementNet;
using BriefFiniteElementNet.DebuggerVisualizersVS13;
using BriefFiniteElementNet.DebuggerVisualizersVS2013;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(MatrixVisualizer), typeof(VisualizerObjectSource), Target = typeof(Matrix), Description = "Epsi1on Matrix Visualizer!")]
namespace BriefFiniteElementNet.DebuggerVisualizersVS2013
{
    public class MatrixVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            // TODO: Get the object to display a visualizer for.
            //       Cast the result of objectProvider.GetObject() 
            //       to the type of the object being visualized.
            object data = (object) objectProvider.GetObject();

            // TODO: Display your view of the object.
            //       Replace displayForm with your own custom Form or Control.
            TestShowVisualizer(data);
        }

        // TODO: Add the following to your testing code to test the visualizer:
        // 
        //    MatrixVisualizer.TestShowVisualizer(new SomeType());
        // 
        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            //VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(MatrixVisualizer));
            //visualizerHost.ShowVisualizer();

            var ctrl = new MatrixVisualizerControl();

            var rows = (int)objectToVisualize.GetType().GetField("rowCount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(objectToVisualize);
            var cols = (int)objectToVisualize.GetType().GetField("columnCount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(objectToVisualize);
            var arr = (double[])objectToVisualize.GetType().GetProperty("CoreArray", BindingFlags.Public | BindingFlags.Instance).GetValue(objectToVisualize, null);


            var mtx = new Matrix((int)rows, (int)cols);
            Array.Copy(arr as double[], mtx.CoreArray, mtx.CoreArray.Length);

            ctrl.VisualizeMatrix(mtx);
            new Window() { Content = ctrl, Title = "epsi1on Matrix Visualizer!", Width = cols * 150, Height = rows * 50 }
                .ShowDialog();

        }
    }

}
