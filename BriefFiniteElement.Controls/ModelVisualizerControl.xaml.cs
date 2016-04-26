using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using BriefFiniteElementNet.Elements;
using DefaultLights = HelixToolkit.DefaultLights;
using MaterialHelper = HelixToolkit.MaterialHelper;
using MeshBuilder = HelixToolkit.MeshBuilder;

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

        public static void VisualizeInNewWindow(Model model)
        {
            var wnd = new Window();

            wnd.Content = new ModelVisualizerControl() {ModelToVisualize = model};

            wnd.ShowDialog();
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

        #region DisableEditingProperties Property and Property Change Routed event

        public static readonly DependencyProperty DisableEditingPropertiesProperty
            = DependencyProperty.Register(
                "DisableEditingProperties", typeof (bool), typeof (ModelVisualizerControl),
                new PropertyMetadata(false, OnDisableEditingPropertiesChanged, DisableEditingPropertiesCoerceValue));

        public bool DisableEditingProperties
        {
            get { return (bool) GetValue(DisableEditingPropertiesProperty); }
            set { SetValue(DisableEditingPropertiesProperty, value); }
        }

        public static readonly RoutedEvent DisableEditingPropertiesChangedEvent
            = EventManager.RegisterRoutedEvent(
                "DisableEditingPropertiesChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<bool>),
                typeof (ModelVisualizerControl));

        private static object DisableEditingPropertiesCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> DisableEditingPropertiesChanged
        {
            add { AddHandler(DisableEditingPropertiesChangedEvent, value); }
            remove { RemoveHandler(DisableEditingPropertiesChangedEvent, value); }
        }

        private static void OnDisableEditingPropertiesChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.DisableEditingPropertiesChangedEvent;
            obj.RaiseEvent(args);
            
        }


        #endregion


        #region ShowNodes Property and Property Change Routed event

        public static readonly DependencyProperty ShowNodesProperty
            = DependencyProperty.Register(
                "ShowNodes", typeof (bool), typeof (ModelVisualizerControl),
                new PropertyMetadata(true, OnShowNodesChanged, ShowNodesCoerceValue));

        public bool ShowNodes
        {
            get { return (bool) GetValue(ShowNodesProperty); }
            set { SetValue(ShowNodesProperty, value); }
        }

        public static readonly RoutedEvent ShowNodesChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowNodesChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<bool>),
                typeof (ModelVisualizerControl));

        private static object ShowNodesCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowNodesChanged
        {
            add { AddHandler(ShowNodesChangedEvent, value); }
            remove { RemoveHandler(ShowNodesChangedEvent, value); }
        }

        private static void OnShowNodesChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowNodesChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region ShowElements Property and Property Change Routed event

        public static readonly DependencyProperty ShowElementsProperty
            = DependencyProperty.Register(
                "ShowElements", typeof (bool), typeof (ModelVisualizerControl),
                new PropertyMetadata(true, OnShowElementsChanged, ShowElementsCoerceValue));

        public bool ShowElements
        {
            get { return (bool) GetValue(ShowElementsProperty); }
            set { SetValue(ShowElementsProperty, value); }
        }

        public static readonly RoutedEvent ShowElementsChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowElementsChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<bool>),
                typeof (ModelVisualizerControl));

        private static object ShowElementsCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowElementsChanged
        {
            add { AddHandler(ShowElementsChangedEvent, value); }
            remove { RemoveHandler(ShowElementsChangedEvent, value); }
        }

        private static void OnShowElementsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowElementsChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region ShowRigidElements Property and Property Change Routed event

        public static readonly DependencyProperty ShowRigidElementsProperty
            = DependencyProperty.Register(
                "ShowRigidElements", typeof (bool), typeof (ModelVisualizerControl),
                new PropertyMetadata(true, OnShowRigidElementsChanged, ShowRigidElementsCoerceValue));

        public bool ShowRigidElements
        {
            get { return (bool) GetValue(ShowRigidElementsProperty); }
            set { SetValue(ShowRigidElementsProperty, value); }
        }

        public static readonly RoutedEvent ShowRigidElementsChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowRigidElementsChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<bool>),
                typeof (ModelVisualizerControl));

        private static object ShowRigidElementsCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowRigidElementsChanged
        {
            add { AddHandler(ShowRigidElementsChangedEvent, value); }
            remove { RemoveHandler(ShowRigidElementsChangedEvent, value); }
        }

        private static void OnShowRigidElementsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowRigidElementsChangedEvent;
            obj.RaiseEvent(args);

            obj.UpdateUi();
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

            if (!ModelToVisualize.Nodes.Any())
                return;


            if (ElementVisualThickness == 0)
                ElementVisualThickness = GetSmartElementThichness();

            #region Adding elements

            var sb = new StringBuilder();

            
            if(ShowElements)
            foreach (var elm in ModelToVisualize.Elements)
            {
                var builder = new MeshBuilder(false, false);

                switch (elm.ElementType)
                {
                    case ElementType.Undefined:
                        sb.AppendLine("Undefined element type! ");
                        break;

                    case ElementType.FrameElement2Node:
                        AddFrameElement(builder, elm as FrameElement2Node);
                        break;

                    case ElementType.ConcentratedMass:
                        AddMassElement(builder, elm as ConcentratedMass);
                        break;

                    case ElementType.TrussElement2Noded:
                        AddTrussElement(builder, elm as TrussElement2Node);
                        break;

                    case ElementType.Dkt:
                        AddDktElement(builder, elm as Obsolete__DktElement);
                        break;

                    case ElementType.Cst:
                        AddCstElement(builder, elm as CstElement);
                        break;

                    default:
                        sb.AppendLine("Unknown element type for rendering: " + elm.ElementType);
                        break;
                }

                var gradient = new LinearGradientBrush();//to be done like this: http://waldoscode.blogspot.de/2014/11/helix-3d-toolkit-well-viewer-part-2.html

                gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
                gradient.GradientStops.Add(new GradientStop(Colors.Cyan, 0.2));
                gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.4));
                gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
                gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
                gradient.GradientStops.Add(new GradientStop(Colors.White, 1));


                var mesh = builder.ToMesh(true);
                
                var material = MaterialHelper.CreateMaterial(gradient, null, null, 1, 0);

                var mygeometry = new GeometryModel3D(mesh, material) { BackMaterial = material };

                var modelElement = new ModelUIElement3D();
                modelElement.Model = mygeometry;


                BindMouseEvents(modelElement, elm);
               // var myModelVisual3D = new ModelVisual3D();
                //myModelVisual3D.Content = modelGroup;


                MainViewport.Children.Add(modelElement);
                
            }

            #endregion


            #region Adding nodes

            if(ShowNodes)
            foreach (var nde in ModelToVisualize.Nodes)
            {
                var builder = new MeshBuilder(false, false);

                AddNode(builder, nde);

                var gradient = new LinearGradientBrush();//to be done like this: http://waldoscode.blogspot.de/2014/11/helix-3d-toolkit-well-viewer-part-2.html

                gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
                gradient.GradientStops.Add(new GradientStop(Colors.Cyan, 0.2));
                gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.4));
                gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
                gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
                gradient.GradientStops.Add(new GradientStop(Colors.White, 1));


                var mesh = builder.ToMesh(true);

                var material = MaterialHelper.CreateMaterial(Brushes.Crimson);

                var mygeometry = new GeometryModel3D(mesh, material) { BackMaterial = material };

                var modelElement = new ModelUIElement3D();
                modelElement.Model = mygeometry;

                BindMouseEvents(modelElement, nde);
                // var myModelVisual3D = new ModelVisual3D();
                //myModelVisual3D.Content = modelGroup;


                MainViewport.Children.Add(modelElement);
            }

            #endregion


            #region Adding rigid elements

            if(ShowRigidElements)
            foreach (var elm in ModelToVisualize.RigidElements)
            {
                var builder = new MeshBuilder(false, false);

                AddRigidElement(builder, elm);

                var gradient = new LinearGradientBrush();//to be done like this: http://waldoscode.blogspot.de/2014/11/helix-3d-toolkit-well-viewer-part-2.html

                gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
                gradient.GradientStops.Add(new GradientStop(Colors.Cyan, 0.2));
                gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.4));
                gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
                gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
                gradient.GradientStops.Add(new GradientStop(Colors.White, 1));


                var mesh = builder.ToMesh(true);

                var material = MaterialHelper.CreateMaterial(Brushes.GreenYellow, Brushes.Green, Brushes.Green,1,100);
                

                var mygeometry = new GeometryModel3D(mesh, material) { BackMaterial = material };

                var modelElement = new ModelUIElement3D();
                modelElement.Model = mygeometry;

                BindMouseEvents(modelElement, elm);
                // var myModelVisual3D = new ModelVisual3D();
                //myModelVisual3D.Content = modelGroup;


                MainViewport.Children.Add(modelElement);
            }

            #endregion

            if (sb.Length != 0)
                MessageBox.Show(sb.ToString());

            
            
            MainViewport.Children.Add(new DefaultLights());



        }

        private void BindMouseEvents(ModelUIElement3D model, Element elm)
        {
            var tagGuid = @"35DA1A20-AA6D-4436-890E-CBFA341F9E51";

            Material oldfrontm = null;
            Material oldbackm = null;


            #region MouseEnter

            model.MouseEnter += (sender, e) =>
            {
                var tb = MainCanvas.Children
                    .Cast<UIElement>()
                    .Where(i => i is Border)
                    .Cast<Border>()
                    .FirstOrDefault(i => tagGuid.Equals(i.Tag));

                if (tb == null)
                {
                    MainCanvas.Children.Add(tb = new Border() {Tag = tagGuid});
                    
                    tb.MouseMove += (o, args) => { args.Handled = false; };
                    tb.PreviewMouseMove += (o, args) => { args.Handled = false; };
                    tb.Child = new TextBlock();
                    StyleTexblock(tb);
                }


                var tx = tb.Child as TextBlock;

                if (tx != null)
                    tx.Text = "ELEMENT: " + elm.ElementType + " : " + elm.Label;

                var geo = model.Model as GeometryModel3D;

                if (geo != null)
                {
                    oldfrontm = geo.Material;
                    oldbackm = geo.BackMaterial;

                    geo.Material = geo.BackMaterial = MaterialHelper.CreateMaterial(Brushes.Aqua);

                }
            };

            #endregion

            #region MouseMove

            model.MouseMove += (sender, e) =>
            {
                Border tb = null;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Border)
                    {
                        if (tagGuid.Equals((child as Border).Tag))
                        {
                            tb = child as Border;
                            break;
                        }
                    }
                }

                if (tb == null)
                    return;

                tb.Visibility = Visibility.Visible;

                var mousePos = e.GetPosition(MainCanvas);

                Canvas.SetLeft(tb, mousePos.X - tb.ActualWidth - 10);
                Canvas.SetTop(tb, mousePos.Y - tb.ActualHeight - 10);

            };

            #endregion

            #region MouseLeave

            model.MouseLeave += (sender, args) =>
            {
                Border tb = null;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Border)
                    {
                        if (tagGuid.Equals((child as Border).Tag))
                        {
                            tb = child as Border;
                            break;
                        }
                    }
                }

                if (tb == null)
                    return;

                tb.Visibility = Visibility.Collapsed;

                var geo = model.Model as GeometryModel3D;

                if (geo != null)
                {
                    geo.Material = oldfrontm;
                    geo.BackMaterial = oldbackm;
                }
            };

            #endregion

            model.MouseDown += (sender, args) =>
            {
                var grd = new DataGrid();

                PropertyHelper.Populate(grd, elm);
                grd.ItemsSource = new[] {elm};

                if (DisableEditingProperties)
                    grd.IsReadOnly = true;

                var wnd = new Window();
                wnd.Content = grd;
                wnd.ShowDialog();
                args.Handled = true;
            };

        }

        private void BindMouseEvents(ModelUIElement3D model, RigidElement elm)
        {
            var tagGuid = @"35DA1A20-AA6D-4436-890E-CBFA341F9E51";

            Material oldfrontm = null;
            Material oldbackm = null;

            /**/
            
            #region MouseEnter

            model.MouseEnter += (sender, e) =>
            {
                var tb = MainCanvas.Children
                    .Cast<UIElement>()
                    .Where(i => i is Border)
                    .Cast<Border>()
                    .FirstOrDefault(i => tagGuid.Equals(i.Tag));

                if (tb == null)
                {
                    MainCanvas.Children.Add(tb = new Border() { Tag = tagGuid });

                    tb.MouseMove += (o, args) => { args.Handled = false; };
                    tb.PreviewMouseMove += (o, args) => { args.Handled = false; };
                    tb.Child = new TextBlock();
                    StyleTexblock(tb);
                }


                var tx = tb.Child as TextBlock;

                if (tx != null)
                    tx.Text = "RigidELEMENT";
                var geo = model.Model as GeometryModel3D;

                if (geo != null)
                {
                    oldfrontm = geo.Material;
                    oldbackm = geo.BackMaterial;

                    geo.Material = geo.BackMaterial = MaterialHelper.CreateMaterial(Brushes.Aqua);

                }
            };

            #endregion

            #region MouseMove

            model.MouseMove += (sender, e) =>
            {
                Border tb = null;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Border)
                    {
                        if (tagGuid.Equals((child as Border).Tag))
                        {
                            tb = child as Border;
                            break;
                        }
                    }
                }

                if (tb == null)
                    return;

                tb.Visibility = Visibility.Visible;

                var mousePos = e.GetPosition(MainCanvas);

                Canvas.SetLeft(tb, mousePos.X - tb.ActualWidth - 10);
                Canvas.SetTop(tb, mousePos.Y - tb.ActualHeight - 10);

            };

            #endregion

            #region MouseLeave

            model.MouseLeave += (sender, args) =>
            {
                Border tb = null;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Border)
                    {
                        if (tagGuid.Equals((child as Border).Tag))
                        {
                            tb = child as Border;
                            break;
                        }
                    }
                }

                if (tb == null)
                    return;

                tb.Visibility = Visibility.Collapsed;

                var geo = model.Model as GeometryModel3D;

                if (geo != null)
                {
                    geo.Material = oldfrontm;
                    geo.BackMaterial = oldbackm;
                }
            };

            #endregion

            model.MouseDown += (sender, args) =>
            {
                var grd = new DataGrid();

                PropertyHelper.Populate(grd, elm);
                grd.ItemsSource = new[] { elm };

                if (DisableEditingProperties)
                    grd.IsReadOnly = true;

                var wnd = new Window();
                wnd.Content = grd;
                wnd.ShowDialog();
                args.Handled = true;
            };

            /**/

        }

        private void StyleTexblock(Border txt)
        {
            txt.BorderThickness = new Thickness(5);
            txt.Padding = new Thickness(5);
            txt.Background = Brushes.White;
            txt.BorderBrush = Brushes.Gray;

            txt.CornerRadius = new CornerRadius(10);
        }

        private void BindMouseEvents(ModelUIElement3D model, Node elm)
        {
            var tagGuid = @"35DA1A20-AA6D-4436-890E-CBFA341F9E51";

            Material oldfrontm = null;
            Material oldbackm = null;


            #region MouseEnter

            model.MouseEnter += (sender, e) =>
            {
                var tb = MainCanvas.Children
                    .Cast<UIElement>()
                    .Where(i => i is Border)
                    .Cast<Border>()
                    .FirstOrDefault(i => tagGuid.Equals(i.Tag));

                if (tb == null)
                {
                    MainCanvas.Children.Add(tb = new Border() { Tag = tagGuid });
                    tb.Child=new TextBlock();
                    tb.MouseMove += (o, args) => { args.Handled = false; };
                    tb.PreviewMouseMove += (o, args) => { args.Handled = false; };
                    StyleTexblock(tb);
                }

                var tx = tb.Child as TextBlock;

                if (tx != null)
                    tx.Text = "NODE: " + elm.Label;

                

                var geo = model.Model as GeometryModel3D;

                if (geo != null)
                {
                    oldfrontm = geo.Material;
                    oldbackm = geo.BackMaterial;

                    geo.Material = geo.BackMaterial = MaterialHelper.CreateMaterial(Brushes.Aqua);

                }
            };

            #endregion

            #region MouseMove

            model.MouseMove += (sender, e) =>
            {
                Border tb = null;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Border)
                    {
                        if (tagGuid.Equals((child as Border).Tag))
                        {
                            tb = child as Border;
                            break;
                        }
                    }
                }

                if (tb == null)
                    return;

                tb.Visibility = Visibility.Visible;

                var mousePos = e.GetPosition(MainCanvas);

                Canvas.SetLeft(tb, mousePos.X - tb.ActualWidth - 10);
                Canvas.SetTop(tb, mousePos.Y - tb.ActualHeight - 10);

            };

            #endregion

            #region MouseLeave

            model.MouseLeave += (sender, args) =>
            {
                Border tb = null;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Border)
                    {
                        if (tagGuid.Equals((child as Border).Tag))
                        {
                            tb = child as Border;
                            break;
                        }
                    }
                }

                if (tb == null)
                    return;

                tb.Visibility = Visibility.Collapsed;

                var geo = model.Model as GeometryModel3D;

                if (geo != null)
                {
                    geo.Material = oldfrontm;
                    geo.BackMaterial = oldbackm;
                }
            };

            #endregion

            model.MouseDown += (sender, args) =>
            {
                var grd = new DataGrid();

                PropertyHelper.Populate(grd, elm);
                grd.ItemsSource = new[] { elm };

                if (DisableEditingProperties)
                    grd.IsReadOnly = true;

                var wnd = new Window();
                wnd.Content = grd;
                wnd.ShowDialog();
                args.Handled = true;

            };

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

            var dims = new double[] {maxX - minX, maxY - minY, maxZ - minZ};

            var epsi1on = dims.Max()*1e-6;


            var dim = new double[] {maxX - minX, maxY - minY, maxZ - minZ}.Where(i => Math.Abs(i) > epsi1on).Any()
                ? new double[] {maxX - minX, maxY - minY, maxZ - minZ}.Where(i => Math.Abs(i) > epsi1on).Min()
                : 1.0;

            var d1 = 0.01*dim;

            var d2 = 0.0;

            if (ModelToVisualize.Elements.Any())
                d2 = 0.05*
                     ModelToVisualize.Elements.Where(i => i.Nodes.Length > 1)
                         .Select(i => (i.Nodes[0].Location - i.Nodes.Last().Location).Length)
                         .Min();

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
                    new PointYZ(-r, -r),
                    new PointYZ(-r, r),
                    new PointYZ(r, r),
                    new PointYZ(r, -r),
                    new PointYZ(-r, -r));
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

        private void AddMassElement(MeshBuilder bldr, ConcentratedMass elm)
        {
            bldr.AddCone(elm.Nodes[0].Location.ToPoint3D(), new Vector3D(0, 0, 1), 0, ElementVisualThickness*2,
                10*ElementVisualThickness, true, true, 5);
        }

        private void AddRigidElement(MeshBuilder bldr, RigidElement elm)
        {
            PolygonYz section = null;

            if (elm.Nodes.Count(i => !ReferenceEquals(i, null)) < 2)
                return;

            var r = ElementVisualThickness / 2;

            var cnt = elm.CentralNode;

            if (cnt == null)
            {
                cnt = elm.Nodes.First(i => !ReferenceEquals(i, null));

                /*
                for (int i = 0; i < elm.Nodes.Count; i++)
                {
                    for (int j = 0; j < elm.Nodes.Count; j++)
                    {
                        if(i==j)
                            continue;

                        var st = elm.Nodes[i].Location.ToPoint3D();
                        var en = elm.Nodes[j].Location.ToPoint3D();

                        bldr.AddPipe(st, en, ElementVisualThickness / 2, ElementVisualThickness / 1.9, 4);
                    }
                }
                */
            }
            //else
            {
                for (var i = 0; i < elm.Nodes.Count; i++)
                {
                    if (ReferenceEquals(elm.Nodes[i], null))
                        continue;

                    if (ReferenceEquals(elm.Nodes[i], cnt))
                        continue;

                    var st = elm.Nodes[i].Location.ToPoint3D();
                    var en = cnt.Location.ToPoint3D();

                    bldr.AddPipe(st, en, ElementVisualThickness / 2, ElementVisualThickness / 1.9, 4);
                }
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

        private void AddDktElement(MeshBuilder bldr, Obsolete__DktElement elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            var p1 = elm.Nodes[0].Location;
            var p2 = elm.Nodes[1].Location;
            var p3 = elm.Nodes[2].Location;


            bldr.AddTriangle(p1, p3, p2);

        }

        private void AddCstElement(MeshBuilder bldr, CstElement elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            var p1 = elm.Nodes[0].Location;
            var p2 = elm.Nodes[1].Location;
            var p3 = elm.Nodes[2].Location;


            bldr.AddTriangle(p1, p3, p2);

        }

        private void AddNode(MeshBuilder bldr, Node nde)
        {
            bldr.AddSphere(new Point3D(nde.Location.X, nde.Location.Y, nde.Location.Z),
                ElementVisualThickness*2, 20, 20);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://helixtoolkit.codeplex.com/");
        }


        
    }
}
