using System;
using System.IO;
using System.Linq;
using System.Windows;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.DebuggerVisualizersVS2013;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ModelVisualizer), typeof(VisualizerObjectSource), Target = typeof(Model), Description = "Epsi1on Model Visualizer!")]
namespace BriefFiniteElementNet.DebuggerVisualizersVS2013
{
    
    public class ModelVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            object data = (object) objectProvider.GetObject();

            TestShowVisualizer(data);
        }

        public static void TestShowVisualizer(object model)
        {
            var mtd = model.GetType()
               .GetMethods()
               .Where(i => i.Name == "Save")
               .Where(i => i.GetParameters().Any())
               .First(i => i.GetParameters().Any(j => j.ParameterType == typeof(Stream)));

            var str = new MemoryStream();

            mtd.Invoke(model, new object[] { str, model });

            str.Position = 0;

            
            var model2 = Model.LoadWithBinder(str);

            var ctrl = new ModelVisualizerControl();

            ctrl.ModelToVisualize = model2;

            var wnd = new Window() {Content = ctrl, Title = "BriefFiniteElement.NET Model Visualizer!"};

            wnd.ShowDialog();
        }
    }
}
