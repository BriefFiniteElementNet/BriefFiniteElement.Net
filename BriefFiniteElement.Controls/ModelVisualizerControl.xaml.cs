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


        public static void VisualizeInNewWindow(Model model, bool dialog = false, params IElementGraphMeshProvider[] prvdrs)
        {
            var wnd = new Window();

            wnd.Content = new ModelVisualizerControl() { ModelToVisualize = model, MeshProviders = prvdrs.ToList() };

            if (dialog)
                wnd.ShowDialog();
            else
                wnd.Show();
        }

        public List<IElementGraphMeshProvider> MeshProviders = new List<IElementGraphMeshProvider>();

        #region ModelToVisualize Property and Property Change Routed event

        public static readonly DependencyProperty ModelToVisualizeProperty
            = DependencyProperty.Register(
                "ModelToVisualize", typeof(BriefFiniteElementNet.Model), typeof(ModelVisualizerControl),
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
                typeof(RoutedPropertyChangedEventHandler<BriefFiniteElementNet.Model>),
                typeof(ModelVisualizerControl));

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
                "ElementVisualThickness", typeof(double), typeof(ModelVisualizerControl),
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
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ModelVisualizerControl));

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
                "DisableEditingProperties", typeof(bool), typeof(ModelVisualizerControl),
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
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

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
                "ShowNodes", typeof(bool), typeof(ModelVisualizerControl),
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
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

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
                "ShowElements", typeof(bool), typeof(ModelVisualizerControl),
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
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

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
                "ShowRigidElements", typeof(bool), typeof(ModelVisualizerControl),
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
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

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

        #region ShowNodalLoads Property and Property Change Routed event

        public static readonly DependencyProperty ShowNodalLoadsProperty
            = DependencyProperty.Register(
                "ShowNodalLoads", typeof(bool), typeof(ModelVisualizerControl),
                new PropertyMetadata(false, OnShowNodalLoadsChanged, ShowNodalLoadsCoerceValue));

        public bool ShowNodalLoads
        {
            get { return (bool) GetValue(ShowNodalLoadsProperty); }
            set { SetValue(ShowNodalLoadsProperty, value); }
        }

        public static readonly RoutedEvent ShowNodalLoadsChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowNodalLoadsChanged",
                RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

        private static object ShowNodalLoadsCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowNodalLoadsChanged
        {
            add { AddHandler(ShowNodalLoadsChangedEvent, value); }
            remove { RemoveHandler(ShowNodalLoadsChangedEvent, value); }
        }

        private static void OnShowNodalLoadsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowNodalLoadsChangedEvent;
            obj.RaiseEvent(args);

            obj.UpdateUi();
        }


        #endregion

        #region ShowNodalLoads_Force Property and Property Change Routed event

        public static readonly DependencyProperty ShowNodalLoads_ForceProperty
            = DependencyProperty.Register(
                "ShowNodalLoads_Force", typeof(bool), typeof(ModelVisualizerControl),
                new PropertyMetadata(false, OnShowNodalLoads_ForceChanged, ShowNodalLoads_ForceCoerceValue));

        public bool ShowNodalLoads_Force
        {
            get { return (bool) GetValue(ShowNodalLoads_ForceProperty); }
            set { SetValue(ShowNodalLoads_ForceProperty, value); }
        }

        public static readonly RoutedEvent ShowNodalLoads_ForceChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowNodalLoads_ForceChanged",
                RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

        private static object ShowNodalLoads_ForceCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowNodalLoads_ForceChanged
        {
            add { AddHandler(ShowNodalLoads_ForceChangedEvent, value); }
            remove { RemoveHandler(ShowNodalLoads_ForceChangedEvent, value); }
        }

        private static void OnShowNodalLoads_ForceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowNodalLoads_ForceChangedEvent;
            obj.RaiseEvent(args);

            obj.UpdateUi();
        }


        #endregion

        #region ShowNodalLoads_Moment Property and Property Change Routed event

        public static readonly DependencyProperty ShowNodalLoads_MomentProperty
            = DependencyProperty.Register(
                "ShowNodalLoads_Moment", typeof(bool), typeof(ModelVisualizerControl),
                new PropertyMetadata(false, OnShowNodalLoads_MomentChanged, ShowNodalLoads_MomentCoerceValue));

        public bool ShowNodalLoads_Moment
        {
            get { return (bool) GetValue(ShowNodalLoads_MomentProperty); }
            set { SetValue(ShowNodalLoads_MomentProperty, value); }
        }

        public static readonly RoutedEvent ShowNodalLoads_MomentChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowNodalLoads_MomentChanged",
                RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

        private static object ShowNodalLoads_MomentCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowNodalLoads_MomentChanged
        {
            add { AddHandler(ShowNodalLoads_MomentChangedEvent, value); }
            remove { RemoveHandler(ShowNodalLoads_MomentChangedEvent, value); }
        }

        private static void OnShowNodalLoads_MomentChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowNodalLoads_MomentChangedEvent;
            obj.RaiseEvent(args);
            
            obj.UpdateUi();
        }


        #endregion

        #region ShowConstraints Property and Property Change Routed event

        public static readonly DependencyProperty ShowConstraintsProperty
            = DependencyProperty.Register(
                "ShowConstraints", typeof(bool), typeof(ModelVisualizerControl),
                new PropertyMetadata(false, OnShowConstraintsChanged, ShowConstraintsCoerceValue));

        public bool ShowConstraints
        {
            get { return (bool) GetValue(ShowConstraintsProperty); }
            set { SetValue(ShowConstraintsProperty, value); }
        }

        public static readonly RoutedEvent ShowConstraintsChangedEvent
            = EventManager.RegisterRoutedEvent(
                "ShowConstraintsChanged",
                RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ModelVisualizerControl));

        private static object ShowConstraintsCoerceValue(DependencyObject d, object value)
        {
            var val = (bool) value;
            var obj = (ModelVisualizerControl) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<bool> ShowConstraintsChanged
        {
            add { AddHandler(ShowConstraintsChangedEvent, value); }
            remove { RemoveHandler(ShowConstraintsChangedEvent, value); }
        }

        private static void OnShowConstraintsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ModelVisualizerControl;
            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool) e.OldValue,
                (bool) e.NewValue);
            args.RoutedEvent = ModelVisualizerControl.ShowConstraintsChangedEvent;
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


            if (ShowElements)
                foreach (var elm in ModelToVisualize.Elements)
                {
                    var builder = new MeshBuilder(false, false);

                    if (elm is FrameElement2Node)
                        AddFrameElement(builder, elm as FrameElement2Node);
                    else if (elm is BarElement)
                        AddBarElement(builder, elm as BarElement);
                    else if (elm is ConcentratedMass)
                        AddMassElement(builder, elm as ConcentratedMass);
                    else if (elm is TrussElement2Node)
                        AddTrussElement(builder, elm as TrussElement2Node);
                    else if (elm is DktElement)
                        AddDktElement(builder, elm as DktElement);
                    else if (elm is Tetrahedral)
                        AddTetrahedronElement(builder, elm as Tetrahedral);
                    else if (elm is TriangleFlatShell)
                        AddFlatshellElement(builder, elm as TriangleFlatShell);
                    else if (elm is CstElement)
                        AddCstElement(builder, elm as CstElement);
                    else if (elm is Element2D)
                        AddElement2d(builder, elm as Element2D);
                    else if (elm is Element3D)
                        AddElement3D(builder, elm as Element3D);
                    else if (elm is Spring1D)
                        AddSpringElement(builder, elm as Spring1D);
                    else if (MeshProviders.Any(i => i.CanProvideMeshForElement(elm)))
                        AddCustomElementWithMeshProvider(builder, elm,
                            MeshProviders.First(i => i.CanProvideMeshForElement(elm)));


                    var gradient = new LinearGradientBrush();
                    //TODO: to be done like this: http://waldoscode.blogspot.de/2014/11/helix-3d-toolkit-well-viewer-part-2.html


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

            #region Adding mpc elements


            if (ShowRigidElements)
                foreach (var elm in ModelToVisualize.MpcElements)
                {
                    var builder = new MeshBuilder(false, false);

                    AddMpcElement(builder, elm);
                    
                    var gradient = new LinearGradientBrush();
                        //TODO: to be done like this: http://waldoscode.blogspot.de/2014/11/helix-3d-toolkit-well-viewer-part-2.html


                    gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
                    gradient.GradientStops.Add(new GradientStop(Colors.Cyan, 0.2));
                    gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.4));
                    gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
                    gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
                    gradient.GradientStops.Add(new GradientStop(Colors.White, 1));


                    var mesh = builder.ToMesh(true);

                    var material = MaterialHelper.CreateMaterial(gradient, null, null, 1, 0);

                    var mygeometry = new GeometryModel3D(mesh, material) {BackMaterial = material};

                    var modelElement = new ModelUIElement3D();
                    modelElement.Model = mygeometry;


                    BindMouseEvents(modelElement, elm);
                    // var myModelVisual3D = new ModelVisual3D();
                    //myModelVisual3D.Content = modelGroup;


                    MainViewport.Children.Add(modelElement);
                }

            #endregion

            #region Adding nodes

            if (ShowNodes)
                foreach (var nde in ModelToVisualize.Nodes)
                {
                    var builder = new MeshBuilder(false, false);

                    AddNode(builder, nde);

                    var gradient = new LinearGradientBrush();
                        //to be done like this: http://waldoscode.blogspot.de/2014/11/helix-3d-toolkit-well-viewer-part-2.html

                    gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
                    gradient.GradientStops.Add(new GradientStop(Colors.Cyan, 0.2));
                    gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.4));
                    gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
                    gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
                    gradient.GradientStops.Add(new GradientStop(Colors.White, 1));


                    var mesh = builder.ToMesh(true);

                    var material = MaterialHelper.CreateMaterial(Brushes.Crimson);

                    var mygeometry = new GeometryModel3D(mesh, material) {BackMaterial = material};

                    var modelElement = new ModelUIElement3D();
                    modelElement.Model = mygeometry;

                    BindMouseEvents(modelElement, nde);
                    // var myModelVisual3D = new ModelVisual3D();
                    //myModelVisual3D.Content = modelGroup;


                    MainViewport.Children.Add(modelElement);
                }

            #endregion

            #region Adding nodal nodes

            if (ShowNodalLoads_Force)
                foreach (var nde in ModelToVisualize.Nodes)
                {
                    var builder = new MeshBuilder(false, false);

                    foreach (var nodalLoad in nde.Loads)
                    {
                        AddNodalLoad_Force(builder, nde, nodalLoad);
                    }

                    var mesh = builder.ToMesh(true);

                    var material = MaterialHelper.CreateMaterial(Brushes.HotPink);

                    var mygeometry = new GeometryModel3D(mesh, material) { BackMaterial = material };

                    var modelElement = new ModelUIElement3D();
                    modelElement.Model = mygeometry;

                    BindMouseEvents(modelElement, nde);
                    // var myModelVisual3D = new ModelVisual3D();
                    //myModelVisual3D.Content = modelGroup;


                    MainViewport.Children.Add(modelElement);
                }

            if (ShowNodalLoads_Moment)
                foreach (var nde in ModelToVisualize.Nodes)
                {
                    var builder = new MeshBuilder(false, false);

                    foreach (var nodalLoad in nde.Loads)
                    {
                        AddNodalLoad_Moment(builder, nde, nodalLoad);
                    }

                    var mesh = builder.ToMesh(true);

                    var material = MaterialHelper.CreateMaterial(Brushes.HotPink);

                    var mygeometry = new GeometryModel3D(mesh, material) { BackMaterial = material };

                    var modelElement = new ModelUIElement3D();
                    modelElement.Model = mygeometry;

                    BindMouseEvents(modelElement, nde);
                    // var myModelVisual3D = new ModelVisual3D();
                    //myModelVisual3D.Content = modelGroup;


                    MainViewport.Children.Add(modelElement);
                }
            #endregion

            #region Adding node constraints

            if (ShowConstraints)
                foreach (var nde in ModelToVisualize.Nodes)
                {
                    var builder = new MeshBuilder(false, false);

                    AddNodeConstraint(builder, nde);

                    var mesh = builder.ToMesh(true);

                    var material = MaterialHelper.CreateMaterial(Brushes.GreenYellow, Brushes.Green, Brushes.Green, 1,
                        100);


                    var mygeometry = new GeometryModel3D(mesh, material) {BackMaterial = material};

                    var modelElement = new ModelUIElement3D();
                    modelElement.Model = mygeometry;

                    BindMouseEvents(modelElement, nde);
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
                PropertyHelper.BrowseObjectProperties(elm);

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
                    MainCanvas.Children.Add(tb = new Border() {Tag = tagGuid});

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
                args.Handled = true;

                PropertyHelper.BrowseObjectProperties(sender);
            };

            /**/
        }

        private void BindMouseEvents(ModelUIElement3D model, MpcElement elm)
        {
            var tagGuid = @"642E0B05-0BFE-4091-81E7-48687CEA310E";

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
                    tx.Text = "MPC Element";
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
                args.Handled = true;

                PropertyHelper.BrowseObjectProperties(elm);
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
                    MainCanvas.Children.Add(tb = new Border() {Tag = tagGuid});
                    tb.Child = new TextBlock();
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

                grd.ItemsSource = new[] {elm};

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

        private void AddSpringElement(MeshBuilder bldr, Spring1D elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;

            if (elm.StartNode.Location.Equals(elm.EndNode.Location))
            {
                bldr.AddSphere(new Point3D(elm.StartNode.Location.X, 
                    elm.StartNode.Location.Y, 
                    elm.StartNode.Location.Z), ElementVisualThickness * 3);
            }
            
        }

        private void AddCustomElementWithMeshProvider(MeshBuilder bldr, Element elm, IElementGraphMeshProvider prv)
        {
            foreach (var tuple in prv.ProvideTriangleMeshForElement(elm))
            {
                var p1 = elm.Nodes[tuple.Item1].Location;
                var p2 = elm.Nodes[tuple.Item2].Location;
                var p3 = elm.Nodes[tuple.Item3].Location;

                bldr.AddTriangle(p1, p3, p2);
            }
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

        private void AddBarElement(MeshBuilder bldr, BarElement elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            if (true)
            {
                section = new PolygonYz(
                    new PointYZ(-r, -r),
                    new PointYZ(-r, r),
                    new PointYZ(r, r),
                    new PointYZ(r, -r),
                    new PointYZ(-r, -r));
            }
         


            for (var i = 0; i < section.Count - 1; i++)
            {
                var trs = elm.GetTransformationManager();

                var v1 = new Vector(0, section[i].Y, section[i].Z);
                var v2 = new Vector(0, section[i + 1].Y, section[i + 1].Z);

                var p1 = elm.StartNode.Location + trs.TransformLocalToGlobal(v1);
                var p2 = elm.StartNode.Location + trs.TransformLocalToGlobal(v2);

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

            var r = ElementVisualThickness/2;

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

                    bldr.AddPipe(st, en, ElementVisualThickness/2, ElementVisualThickness/1.9, 4);
                }
            }
        }

        private void AddTrussElement(MeshBuilder bldr, TrussElement2Node elm)
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

        private void AddDktElement(MeshBuilder bldr, DktElement elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness/2;


            var p1 = elm.Nodes[0].Location;
            var p2 = elm.Nodes[1].Location;
            var p3 = elm.Nodes[2].Location;


            bldr.AddTriangle(p1, p3, p2);
        }

        private void AddTetrahedronElement(MeshBuilder bldr, Tetrahedral elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            var p1 = elm.Nodes[0].Location;
            var p2 = elm.Nodes[1].Location;
            var p3 = elm.Nodes[2].Location;
            var p4 = elm.Nodes[3].Location;


            bldr.AddTriangle(p1, p3, p4);
            bldr.AddTriangle(p3, p2, p4);
            bldr.AddTriangle(p1, p2, p4);
            bldr.AddTriangle(p1, p2, p3);
            
        }

        private void AddFlatshellElement(MeshBuilder bldr, TriangleFlatShell elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness / 2;


            var p1 = elm.Nodes[0].Location;
            var p2 = elm.Nodes[1].Location;
            var p3 = elm.Nodes[2].Location;


            bldr.AddTriangle(p1, p3, p2);
        }

        private void AddElement2d(MeshBuilder bldr, Element2D elm)
        {

            if (elm.Nodes.Length == 4)
            {
                var p1 = elm.Nodes[0].Location;
                var p2 = elm.Nodes[1].Location;
                var p3 = elm.Nodes[2].Location;
                var p4 = elm.Nodes[3].Location;


                bldr.AddTriangle(p1, p2, p3);

                bldr.AddTriangle(p1, p3, p4);
            }


            if (elm.Nodes.Length == 3)
            {
                var p1 = elm.Nodes[0].Location;
                var p2 = elm.Nodes[1].Location;
                var p3 = elm.Nodes[2].Location;


                bldr.AddTriangle(p1, p2, p3);
            }

        }

        private void AddCstElement(MeshBuilder bldr, CstElement elm)
        {
            PolygonYz section = null;

            var r = ElementVisualThickness/2;


            var p1 = elm.Nodes[0].Location;
            var p2 = elm.Nodes[1].Location;
            var p3 = elm.Nodes[2].Location;


            bldr.AddTriangle(p1, p3, p2);
        }

        private void AddElement3D(MeshBuilder bldr, Element3D elm)
        {
            if (elm.Nodes.Length == 8)
            {
                //brick

                var p = new Func<Node, Point>(i => i.Location);

                var sides = new int[][]
                {
                    new int[] {1, 2, 6}, new int[] {1, 5, 6},
                    new int[] {2, 6, 3}, new int[] {7, 6, 3},
                    new int[] {4, 3, 7}, new int[] {4, 8, 7},

                    new int[] {1, 4, 8}, new int[] {1, 5, 8},
                    new int[] {5, 6, 7}, new int[] {5, 8, 7},
                    new int[] {1, 2, 3}, new int[] {1, 4, 3},
                };



                foreach (var side in sides)
                {
                    bldr.AddTriangle(p(elm.Nodes[side[0]-1]), p(elm.Nodes[side[1]-1]), p(elm.Nodes[side[2]-1]));
                }
                
            }
        }

        private void AddNode(MeshBuilder bldr, Node nde)
        {
            bldr.AddSphere(new Point3D(nde.Location.X, nde.Location.Y, nde.Location.Z),
                ElementVisualThickness*2, 20, 20);
        }

        private void AddMpcElement(MeshBuilder bldr, MpcElement elm)
        {
            PolygonYz section = null;

            if (elm.Nodes.Count(i => !ReferenceEquals(i, null)) < 2)
                return;

            var r = ElementVisualThickness / 2;

            var cnt = (Node)null;

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

        private void AddNodalLoad_Force(MeshBuilder bldr, Node nde,NodalLoad load)
        {
            var nodeRadius = ElementVisualThickness;

            var arrowLength = nodeRadius*10;

            var arrowDia = arrowLength/5;

            var loc = nde.Location;

            if (load.Force.Fx != 0.0)
            {
                bldr.AddArrow(ToP3d(loc - Math.Sign(load.Force.Fx) * Vector.I*arrowLength), ToP3d(loc), arrowDia);
            }

            if (load.Force.Fy != 0.0)
            {
                bldr.AddArrow(ToP3d(loc - Math.Sign(load.Force.Fy) * Vector.J*arrowLength), ToP3d(loc), arrowDia);
            }

            if (load.Force.Fz != 0.0)
            {
                bldr.AddArrow(ToP3d(loc - Math.Sign(load.Force.Fz) * Vector.K*arrowLength), ToP3d(loc), arrowDia);
            }
        }

        private void AddNodalLoad_Moment(MeshBuilder bldr, Node nde, NodalLoad load)
        {
            var nodeRadius = ElementVisualThickness;

            var arrowLength = nodeRadius * 10;

            var arrowDia = arrowLength / 5;

            var loc = nde.Location;

            if (load.Force.Mx != 0.0)
            {
                bldr.AddArrow(ToP3d(loc - Math.Sign(load.Force.Mx)* Vector.I * arrowLength), ToP3d(loc), arrowDia);
            }

            if (load.Force.My != 0.0)
            {
                bldr.AddArrow(ToP3d(loc - Math.Sign(load.Force.My) * Vector.J * arrowLength), ToP3d(loc), arrowDia);
            }

            if (load.Force.Mz != 0.0)
            {
                bldr.AddArrow(ToP3d(loc - Math.Sign(load.Force.Mz) * Vector.K * arrowLength), ToP3d(loc), arrowDia);
            }
        }


        private void AddNodeConstraint(MeshBuilder bldr, Node nde)
        {
            var cns = nde.Constraints;

            if (cns == Constraint.Released)
                return;


            var nodeRadius = ElementVisualThickness;

            var arrowLength = nodeRadius * 10;

            var arrowDia = arrowLength / 5;

            var bh = nodeRadius * 4;
            var bw = bh*1.3;

            var loc = nde.Location;
            var below = loc - Vector.K*(2*nodeRadius);



            if (cns == Constraint.Fixed)
            {
                var boxCenter = ToP3d(below - Vector.K*bh/2);

                bldr.AddBox(boxCenter, bw, bw/20, bh);
                bldr.AddBox(boxCenter, bw/20, bw, bh);
                return;
            }

            else
            {
                var boxCenter = ToP3d(below - Vector.K * bh / 2);

                bldr.AddCylinder(ToP3d(below), boxCenter, bw, 30);
            }
        }

        private static Point3D ToP3d(Point pt)
        {
            return new Point3D(pt.X, pt.Y, pt.Z);

        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://helixtoolkit.codeplex.com/");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewport.ZoomExtents();
        }

        private void btnSolve_OnClick(object sender, RoutedEventArgs e)
        {
            ModelToVisualize.Solve();
        }


        private void btnInternalForce_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}