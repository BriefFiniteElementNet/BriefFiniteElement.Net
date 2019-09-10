using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.DebuggerVisualizers;
using BriefFiniteElementNet.Mathh;

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(PolynomialVisualizer), typeof(VisualizerObjectSource), Target = typeof(Polynomial), Description = "Epsi1on polynomial Visualizer!")]
namespace BriefFiniteElementNet.DebuggerVisualizers
{

    public class PolynomialVisualizer : DialogDebuggerVisualizer
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
            object data = (object)objectProvider.GetObject();

            // TODO: Display your view of the object.
            //       Replace displayForm with your own custom Form or Control.
            TestShowVisualizer(data);
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            //VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(MatrixVisualizer));
            //visualizerHost.ShowVisualizer();

            var pl = objectToVisualize as Polynomial;

            if (pl == null)
                return;

            var ctrl = new FunctionVisualizer();

            ctrl.GraphColor = Colors.Black;
            //ctrl.HorizontalAxisLabel = "X";
            //ctrl.VerticalAxisLabel = "Y";
            ctrl.Min = -1;
            ctrl.Max = 1;
            ctrl.SamplingCount = 100;
            ctrl.TargetFunction = new Func<double, double>(i => pl.Evaluate(i));

            ctrl.UpdateUi();

            new Window() { Content = ctrl, Title = "polynomial Visualizer!", Width = 500, Height = 300 }
                .ShowDialog();

        }
    }
}
