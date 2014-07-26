using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Interaction logic for ModelVisualizer.xaml
    /// </summary>
    public partial class ModelVisualizerControl : UserControl
    {
        public ModelVisualizerControl()
        {
            InitializeComponent();

            txtHelp.ToolTip = new HelixHelp();

            this.Loaded += (sender, args) => UpdateUi();
        }

        #region ModelToVisualize Property and Property Change Routed event

        public static readonly DependencyProperty ModelToVisualizeProperty
            = DependencyProperty.Register(
                "ModelToVisualize", typeof (BriefFiniteElementNet.Model), typeof (ModelVisualizerControl),
                new PropertyMetadata(null, OnModelToVisualizeChanged, ModelToVisualizeCoerceValue));

        public BriefFiniteElementNet.Model ModelToVisualize
        {
            get { return (BriefFiniteElementNet.Model) GetValue(ModelToVisualizeProperty); }
            set { SetValue(ModelToVisualizeProperty, value); }
        }

        public static readonly RoutedEvent ModelToVisualizeChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ModelToVisualizeChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<BriefFiniteElementNet.Model>),
                typeof (ModelVisualizerControl));

        private static object ModelToVisualizeCoerceValue(DependencyObject d, object value)
        {
            var val = (BriefFiniteElementNet.Model) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<BriefFiniteElementNet.Model> ModelToVisualizeChanged
        {
            add { AddHandler(ModelToVisualizeChangedEvent, value); }
            remove { RemoveHandler(ModelToVisualizeChangedEvent, value); }
        }

        private static void OnModelToVisualizeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<BriefFiniteElementNet.Model>(
                (BriefFiniteElementNet.Model) e.OldValue,
                (BriefFiniteElementNet.Model) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ModelToVisualizeChangedEvent;
            obj.RaiseEvent(args);

            obj.UpdateUi();
        }


        #endregion

        #region ElementVisualThickness Property and Property Change Routed event

        public static readonly DependencyProperty ElementVisualThicknessProperty
            = DependencyProperty.Register(
                "ElementVisualThickness", typeof (double), typeof (ModelVisualizerControl),
                new PropertyMetadata(0.0, OnElementVisualThicknessChanged, ElementVisualThicknessCoerceValue));

        public double ElementVisualThickness
        {
            get { return (double) GetValue(ElementVisualThicknessProperty); }
            set { SetValue(ElementVisualThicknessProperty, value); }
        }

        public static readonly RoutedEvent ElementVisualThicknessChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ElementVisualThicknessChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<double>),
                typeof (ModelVisualizerControl));

        private static object ElementVisualThicknessCoerceValue(DependencyObject d, object value)
        {
            var val = (double) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<double> ElementVisualThicknessChanged
        {
            add { AddHandler(ElementVisualThicknessChangedEvent, value); }
            remove { RemoveHandler(ElementVisualThicknessChangedEvent, value); }
        }

        private static void OnElementVisualThicknessChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<double>(
                (double) e.OldValue,
                (double) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ElementVisualThicknessChangedEvent;
            obj.RaiseEvent(args);
        }


        #endregion


        /// <summary>
        /// Updates the UI.
        /// </summary>
        public void UpdateUi()
        {
            MainViewport.Children.Clear();

            if (!this.IsLoaded)
                return;

            if (ModelToVisualize == null)
                return;


            if (ElementVisualThickness == 0)
                ElementVisualThickness = GetSmartElementThichness();

            


            var builder = new MeshBuilder(false, false);

            #region Adding elements

            foreach (var elm in ModelToVisualize.Elements)
            {
                switch (elm.ElementType)
                {
                    case ElementType.Undefined:
                        throw new Exception();
                        break;
                    case ElementType.FrameElement2Node:
                        AddFrameElement(builder, elm as FrameElement2Node);
                        break;
                    case ElementType.TrussElement2Noded:
                        AddTrussElement(builder, elm as TrussElement2Node);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            #endregion



            var gradient = new LinearGradientBrush();

            gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
            gradient.GradientStops.Add(new GradientStop(Colors.Cyan, 0.2));
            gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.4));
            gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
            gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
            gradient.GradientStops.Add(new GradientStop(Colors.White, 1));


            var mesh = builder.ToMesh(true);
            var modelGroup = new Model3DGroup();
            var material = MaterialHelper.CreateMaterial(gradient, null, null, 1, 0);

            var mygeometry = new GeometryModel3D(mesh, material) {BackMaterial = material};
            modelGroup.Children.Add(mygeometry);
            var myModelVisual3D = new ModelVisual3D();
            myModelVisual3D.Content = modelGroup;
            MainViewport.Children.Add(myModelVisual3D);
            MainViewport.Children.Add(new DefaultLights());

        }




        private double GetSmartElementThichness()
        {
            var mdl = ModelToVisualize;
            var points = mdl.Nodes.Select(i => i.Location).ToArray();

            var minX = points.Select(i => i.X).Min();
            var maxX = points.Select(i => i.X).Max();

            var minY = points.Select(i => i.Y).Min();
            var maxY = points.Select(i => i.Y).Max();

            var minZ = points.Select(i => i.Z).Min();
            var maxZ = points.Select(i => i.Z).Max();


            var dim = new double[] {maxX - minX, maxY - minY, maxZ - minZ}.Where(i => i != 0).Min();

            var d1 = 0.01*dim;

            var d2 = 0.05*
                     ModelToVisualize.Elements.Select(i => (i.Nodes[0].Location - i.Nodes.Last().Location).Length).Min();

            var res = new double[] {d1, d2}.Max();

            return res;
        }


        private void AddFrameElement(MeshBuilder bldr, FrameElement2Node elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness/2;


            if (elm.UseOverridedProperties)
            {
                section = new PolygonYz(
                    new PointYz(-r, -r),
                    new PointYz(-r, r),
                    new PointYz(r, r),
                    new PointYz(r, -r),
                    new PointYz(-r, -r));
            }
            else
                section = elm.Geometry;

            
            for (var i = 0; i < section.Count-1; i++)
            {
                var v1 = new Vector(0, section[i].Y, section[i].Z);
                var v2 = new Vector(0, section[i + 1].Y, section[i + 1].Z);

                var p1 = elm.StartNode.Location + elm.TransformLocalToGlobal(v1);
                var p2 = elm.StartNode.Location + elm.TransformLocalToGlobal(v2);

                var v = elm.EndNode.Location - elm.StartNode.Location;

                if (Math.Abs(v.Z) < 0.01)
                    Guid.NewGuid();

                var p3 = p1 + v;
                var p4 = p2 + v;

                bldr.AddTriangle(p1, p3, p2);
                bldr.AddTriangle(p4, p2, p3);
            }

        }

        private void AddTrussElement(MeshBuilder bldr, TrussElement2Node elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            if (elm.UseOverridedProperties)
            {
                section = new PolygonYz(
                    new PointYz(-r, -r),
                    new PointYz(-r, r),
                    new PointYz(r, r),
                    new PointYz(r, -r),
                    new PointYz(-r, -r));
            }
            else
                section = elm.Geometry;


            for (var i = 0; i < section.Count - 1; i++)
            {
                var v1 = new Vector(0, section[i].Y, section[i].Z);
                var v2 = new Vector(0, section[i + 1].Y, section[i + 1].Z);

                var p1 = elm.StartNode.Location + elm.TransformLocalToGlobal(v1);
                var p2 = elm.StartNode.Location + elm.TransformLocalToGlobal(v2);

                var v = elm.EndNode.Location - elm.StartNode.Location;

                if (Math.Abs(v.Z) < 0.01)
                    Guid.NewGuid();

                var p3 = p1 + v;
                var p4 = p2 + v;

                bldr.AddTriangle(p1, p3, p2);
                bldr.AddTriangle(p4, p2, p3);
            }

        }

        private void AddNode(MeshBuilder bldr, Node nde)
        {
            throw new NotImplementedException();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://helixtoolkit.codeplex.com/");
        }


        
    }
}
