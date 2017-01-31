using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BriefFiniteElementNet.Elements;
using HelixToolkit;


namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Interaction logic for ModelInternalForceVisualizer.xaml
    /// </summary>
    public partial class ModelInternalForceVisualizer : UserControl
    {

        #region ModelToVisualize Property and Property Change Routed event

        public static readonly DependencyProperty ModelToVisualizeProperty =
            ModelVisualizerControl.ModelToVisualizeProperty.AddOwner(typeof (ModelInternalForceVisualizer),
                new PropertyMetadata(null, OnModelToVisualizeChanged, ModelToVisualizeCoerceValue));

        /*
        public static readonly DependencyProperty ModelToVisualizeProperty
            = DependencyProperty.Register(
                "ModelToVisualize", typeof (Model), typeof (ModelInternalForceVisualizer),
                new PropertyMetadata(null, OnModelToVisualizeChanged, ModelToVisualizeCoerceValue));
        */
        public Model ModelToVisualize
        {
            get { return (Model) GetValue(ModelToVisualizeProperty); }
            set { SetValue(ModelToVisualizeProperty, value); }
        }

        public static readonly RoutedEvent ModelToVisualizeChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ModelToVisualizeChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<Model>),
                typeof (ModelInternalForceVisualizer));

        private static object ModelToVisualizeCoerceValue(DependencyObject d, object value)
        {
            var val = (Model) value;
            var obj = (ModelInternalForceVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<Model> ModelToVisualizeChanged
        {
            add { AddHandler(ModelToVisualizeChangedEvent, value); }
            remove { RemoveHandler(ModelToVisualizeChangedEvent, value); }
        }

        private static void OnModelToVisualizeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelInternalForceVisualizer;
            var args = new RoutedPropertyChangedEventArgs<Model>(
                (Model) e.OldValue,
                (Model) e.NewValue);
            args.RoutedEvent = ModelInternalForceVisualizer.ModelToVisualizeChangedEvent;
            obj.RaiseEvent(args);
            
        }


        #endregion

        #region ElementVisualThickness Property and Property Change Routed event

        public static readonly DependencyProperty ElementVisualThicknessProperty =
            ModelVisualizerControl.ElementVisualThicknessProperty.AddOwner(typeof (ModelInternalForceVisualizer),
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
                typeof (ModelInternalForceVisualizer));

        private static object ElementVisualThicknessCoerceValue(DependencyObject d, object value)
        {
            var val = (double) value;
            var obj = (ModelInternalForceVisualizer) d;


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
            var obj = d as ModelInternalForceVisualizer;
            var args = new RoutedPropertyChangedEventArgs<double>(
                (double) e.OldValue,
                (double) e.NewValue);
            args.RoutedEvent = ModelInternalForceVisualizer.ElementVisualThicknessChangedEvent;
            obj.RaiseEvent(args);
            
        }


        #endregion

        #region MaxDiagramHeight Property and Property Change Routed event

        public static readonly DependencyProperty MaxDiagramHeightProperty
            = DependencyProperty.Register(
                "MaxDiagramHeight", typeof (double), typeof (ModelInternalForceVisualizer),
                new PropertyMetadata(0.0, OnMaxDiagramHeightChanged, MaxDiagramHeightCoerceValue));

        public double MaxDiagramHeight
        {
            get { return (double) GetValue(MaxDiagramHeightProperty); }
            set { SetValue(MaxDiagramHeightProperty, value); }
        }

        public static readonly RoutedEvent MaxDiagramHeightChangedEvent
            = EventManager.RegisterRoutedEvent(
                "MaxDiagramHeightChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<double>),
                typeof (ModelInternalForceVisualizer));

        private static object MaxDiagramHeightCoerceValue(DependencyObject d, object value)
        {
            var val = (double) value;
            var obj = (ModelInternalForceVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<double> MaxDiagramHeightChanged
        {
            add { AddHandler(MaxDiagramHeightChangedEvent, value); }
            remove { RemoveHandler(MaxDiagramHeightChangedEvent, value); }
        }

        private static void OnMaxDiagramHeightChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelInternalForceVisualizer;
            var args = new RoutedPropertyChangedEventArgs<double>(
                (double) e.OldValue,
                (double) e.NewValue);
            args.RoutedEvent = ModelInternalForceVisualizer.MaxDiagramHeightChangedEvent;
            obj.RaiseEvent(args);
            
        }


        #endregion

        #region CurrentInternalForceType Property and Property Change Routed event

        public static readonly DependencyProperty CurrentInternalForceTypeProperty
            = DependencyProperty.Register(
                "CurrentInternalForceType", typeof (InternalForceType), typeof (ModelInternalForceVisualizer),
                new PropertyMetadata((InternalForceType) 0, OnCurrentInternalForceTypeChanged,
                    CurrentInternalForceTypeCoerceValue));

        public InternalForceType CurrentInternalForceType
        {
            get { return (InternalForceType) GetValue(CurrentInternalForceTypeProperty); }
            set { SetValue(CurrentInternalForceTypeProperty, value); }
        }

        public static readonly RoutedEvent CurrentInternalForceTypeChangedEvent
            = EventManager.RegisterRoutedEvent(
                "CurrentInternalForceTypeChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<InternalForceType>),
                typeof (ModelInternalForceVisualizer));

        private static object CurrentInternalForceTypeCoerceValue(DependencyObject d, object value)
        {
            var val = (InternalForceType) value;
            var obj = (ModelInternalForceVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<InternalForceType> CurrentInternalForceTypeChanged
        {
            add { AddHandler(CurrentInternalForceTypeChangedEvent, value); }
            remove { RemoveHandler(CurrentInternalForceTypeChangedEvent, value); }
        }

        private static void OnCurrentInternalForceTypeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelInternalForceVisualizer;
            var args = new RoutedPropertyChangedEventArgs<InternalForceType>(
                (InternalForceType) e.OldValue,
                (InternalForceType) e.NewValue);
            args.RoutedEvent = ModelInternalForceVisualizer.CurrentInternalForceTypeChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region SamplingCount Property and Property Change Routed event

        public static readonly DependencyProperty SamplingCountProperty
            = DependencyProperty.Register(
                "SamplingCount", typeof (int), typeof (ModelInternalForceVisualizer),
                new PropertyMetadata(10, OnSamplingCountChanged, SamplingCountCoerceValue));

        public int SamplingCount
        {
            get { return (int) GetValue(SamplingCountProperty); }
            set { SetValue(SamplingCountProperty, value); }
        }

        public static readonly RoutedEvent SamplingCountChangedEvent
            = EventManager.RegisterRoutedEvent(
                "SamplingCountChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<int>),
                typeof (ModelInternalForceVisualizer));

        private static object SamplingCountCoerceValue(DependencyObject d, object value)
        {
            var val = (int) value;
            var obj = (ModelInternalForceVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<int> SamplingCountChanged
        {
            add { AddHandler(SamplingCountChangedEvent, value); }
            remove { RemoveHandler(SamplingCountChangedEvent, value); }
        }

        private static void OnSamplingCountChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelInternalForceVisualizer;
            var args = new RoutedPropertyChangedEventArgs<int>(
                (int) e.OldValue,
                (int) e.NewValue);
            args.RoutedEvent = ModelInternalForceVisualizer.SamplingCountChangedEvent;
            obj.RaiseEvent(args);
            
        }


        #endregion


        public static void VisualizeInNewWindow(Model model)
        {
            var wnd = new Window();

            var ctrl = new ModelInternalForceVisualizer()
            {
                ModelToVisualize = model,
                SamplingCount = 10,
                TargetCombination = LoadCombination.DefaultLoadCombination,
                CurrentInternalForceType = InternalForceType.My
            };

            wnd.Content = ctrl;

            ctrl.UpdateUi();

            wnd.ShowDialog();
        }

        internal LoadCombination TargetCombination;


        public ModelInternalForceVisualizer()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://helixtoolkit.codeplex.com/");
        }

        public void UpdateUi()
        {
            MainViewport.Children.Clear();

            //if (!this.IsLoaded)
            //    return;

            if (ModelToVisualize == null)
                return;


            if (ElementVisualThickness == 0)
                ElementVisualThickness = GetSmartElementThichness();

            if (TargetCombination == null)
                return;


            var elementsBuilder = new MeshBuilder(false, false);
            var forcesBuilder = new MeshBuilder(false, false);

            var internalForces = GetLocalInternalForces();

            #region determining scale

            var max = 0.0;

            foreach (var internalForce in internalForces)
            {
                foreach (var point in internalForce)
                {
                    max = Math.Max(max, Math.Abs(point.Y));
                    max = Math.Max(max, Math.Abs(point.Z));
                }
            }

            var minElementLength = ModelToVisualize.Elements.Select(i => i.GetElementLength()).Where(i => i > 0).Min();

            var sc = 0.33*minElementLength/max;

            foreach (var internalForce in internalForces)
            {
                for (var i = 0; i < internalForce.Length; i++)
                {
                    internalForce[i] = new Point(internalForce[i].X, internalForce[i].Y*sc, internalForce[i].Z*sc);
                }
            }

            
            #endregion

            #region Adding elements

            var c = ModelToVisualize.Elements.Count;

            for (var i = 0; i < c; i++)
            {
                var elm = ModelToVisualize.Elements[i];
            
                switch (elm.ElementType)
                {
                    case ElementType.Undefined:
                        //throw new Exception();
                        break;
                    case ElementType.FrameElement2Node:
                        AddFrameElement(elementsBuilder, elm as FrameElement2Node);
                        AddFrameElementLoad(forcesBuilder, elm as FrameElement2Node, internalForces[i]);
                        break;
                    case ElementType.TrussElement2Noded:
                        AddTrussElement(elementsBuilder, elm as TrussElement2Node);
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


            var elmMsh = elementsBuilder.ToMesh(true);
            var frcMsh = forcesBuilder.ToMesh(true);

            var modelGroup = new Model3DGroup();

            var elmMat = MaterialHelper.CreateMaterial(gradient, null, null, 1, 0);
            var frcMat = MaterialHelper.CreateMaterial(gradient, null, null, 0.5, 0);

            var elmGeometry = new GeometryModel3D(elmMsh, elmMat) { BackMaterial = elmMat };
            var frcGeometry = new GeometryModel3D(frcMsh, frcMat) { BackMaterial = elmMat };




            modelGroup.Children.Add(elmGeometry);
            modelGroup.Children.Add(frcGeometry);

            var myModelVisual3D = new ModelVisual3D();
            myModelVisual3D.Content = modelGroup;
            MainViewport.Children.Add(myModelVisual3D);
            MainViewport.Children.Add(new DefaultLights());

            MainViewport.ZoomExtents();
        }


        /// <summary>
        /// Gets the every element's internal force in its local coordination system.
        /// will be used for normalization of digrams to digrams look better 
        /// </summary>
        /// <returns></returns>
        public List<Point[]> GetLocalInternalForces()
        {
            var n = SamplingCount;

            var buf = new List<Point[]>();

            foreach (var element in ModelToVisualize.Elements)
            {
                var elm = element as FrameElement2Node;
                
                if (elm == null)
                    continue;

                var diagramPoints = new Point[n];//Y=Fx or Fy or etc
                //var baseDiagramPoints = new Point[n];//Y=0

                var delta = (elm.EndNode.Location - elm.StartNode.Location).Length / (SamplingCount - 1);

                //var st = elm.StartNode.Location;

                for (var i = 0; i < n; i++)
                {
                    var x = delta * i;
                    var force = elm.GetInternalForceAt(x, TargetCombination);

                    //force = elm.TransformLocalToGlobal(force);

                    var y = 0.0;
                    var z = 0.0;

                    switch (CurrentInternalForceType)
                    {
                        case InternalForceType.Fx:
                            z = force.Fx;
                            break;
                        case InternalForceType.Fy:
                            y = force.Fy;
                            break;
                        case InternalForceType.Fz:
                            z = force.Fz;
                            break;
                        case InternalForceType.Mx:
                            z = force.Mx;
                            break;
                        case InternalForceType.My:
                            z = force.My;
                            break;
                        case InternalForceType.Mz:
                            y = force.Mz;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("CurrentInternalForceType");
                    }

                    var localPoint = new Vector(x, y, z);

                    diagramPoints[i] = (Point)localPoint;
                }

                buf.Add(diagramPoints);
            }

            return buf;
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


            var dim = new double[] { maxX - minX, maxY - minY, maxZ - minZ }.Where(i => i != 0).Min();

            var d1 = 0.01 * dim;

            var d2 = 0.05 *
                     ModelToVisualize.Elements.Select(i => (i.Nodes[0].Location - i.Nodes.Last().Location).Length).Min();

            var res = new double[] { d1, d2 }.Max();

            return res;
        }


        private void AddFrameElement(MeshBuilder bldr, FrameElement2Node elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            if (elm.UseOverridedProperties)
            {
                section = new PolygonYz(
                    new PointYZ(-r, -r),
                    new PointYZ(-r, r),
                    new PointYZ(r, r),
                    new PointYZ(r, -r),
                    new PointYZ(-r, -r));
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

        private void AddTrussElement(MeshBuilder bldr, TrussElement2Node elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            if (elm.UseOverridedProperties)
            {
                section = new PolygonYz(
                    new PointYZ(-r, -r),
                    new PointYZ(-r, r),
                    new PointYZ(r, r),
                    new PointYZ(r, -r),
                    new PointYZ(-r, -r));
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

        private void AddFrameElementLoad(MeshBuilder bldr, FrameElement2Node elm,Point[] localPoints)
        {
            var n = SamplingCount;

            var diagramPoints = new Point[n];//Y=Fx or Fy or etc
            var baseDiagramPoints = new Point[n];//Y=0

            

            #region calculating the graph nodes

            var delta = (elm.EndNode.Location - elm.StartNode.Location).Length/(SamplingCount - 1);

            var xs = Enumerable.Range(0, SamplingCount).Select(i=>i*delta).ToList();

            var st = elm.StartNode.Location;


            for (var i = 0; i < n; i++)
            {
                var x = delta * i;
                var localPoint = localPoints[i];//new Vector(x, y*scale, z*scale);
                var globalPoint = st + elm.TransformLocalToGlobal((Vector)localPoint);

                var globalBase = st+elm.TransformLocalToGlobal(new Vector(x, 0, 0));

                diagramPoints[i] = globalPoint;
                baseDiagramPoints[i] = globalBase;
            }


            #endregion

            for (var i = 0; i < n-1; i++)
            {
                bldr.AddTriangle(baseDiagramPoints[i], baseDiagramPoints[i + 1], diagramPoints[i]);
                bldr.AddTriangle(diagramPoints[i], baseDiagramPoints[i + 1], diagramPoints[i + 1]);
            }
        }


       
    }

    public enum InternalForceType
    {
        Undefined = 0,
        Fx,
        Fy,
        Fz,
        Mx,
        My,
        Mz
    }
}
