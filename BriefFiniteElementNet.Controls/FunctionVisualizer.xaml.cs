﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Interaction logic for FunctionVisualizer.xaml
    /// </summary>
    public partial class FunctionVisualizer : UserControl
    {
            public FunctionVisualizer()
        {
            InitializeComponent();
        }

        #region VerticalAxisLabel Property and Property Change Routed event

        public static readonly DependencyProperty VerticalAxisLabelProperty
            = DependencyProperty.Register(
                "VerticalAxisLabel", typeof (string), typeof (FunctionVisualizer),
                new PropertyMetadata(null, OnVerticalAxisLabelChanged, VerticalAxisLabelCoerceValue));

        public string VerticalAxisLabel
        {
            get { return (string) GetValue(VerticalAxisLabelProperty); }
            set { SetValue(VerticalAxisLabelProperty, value); }
        }

        public static readonly RoutedEvent VerticalAxisLabelChangedEvent
            = EventManager.RegisterRoutedEvent(
                "VerticalAxisLabelChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<string>),
                typeof (FunctionVisualizer));

        private static object VerticalAxisLabelCoerceValue(DependencyObject d, object value)
        {
            var val = (string) value;
            var obj = (FunctionVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<string> VerticalAxisLabelChanged
        {
            add { AddHandler(VerticalAxisLabelChangedEvent, value); }
            remove { RemoveHandler(VerticalAxisLabelChangedEvent, value); }
        }

        private static void OnVerticalAxisLabelChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<string>(
                (string) e.OldValue,
                (string) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.VerticalAxisLabelChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region HorizontalAxisLabel Property and Property Change Routed event

        public static readonly DependencyProperty HorizontalAxisLabelProperty
            = DependencyProperty.Register(
                "HorizontalAxisLabel", typeof (string), typeof (FunctionVisualizer),
                new PropertyMetadata(null, OnHorizontalAxisLabelChanged, HorizontalAxisLabelCoerceValue));

        public string HorizontalAxisLabel
        {
            get { return (string) GetValue(HorizontalAxisLabelProperty); }
            set { SetValue(HorizontalAxisLabelProperty, value); }
        }

        public static readonly RoutedEvent HorizontalAxisLabelChangedEvent
            = EventManager.RegisterRoutedEvent(
                "HorizontalAxisLabelChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<string>),
                typeof (FunctionVisualizer));

        private static object HorizontalAxisLabelCoerceValue(DependencyObject d, object value)
        {
            var val = (string) value;
            var obj = (FunctionVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<string> HorizontalAxisLabelChanged
        {
            add { AddHandler(HorizontalAxisLabelChangedEvent, value); }
            remove { RemoveHandler(HorizontalAxisLabelChangedEvent, value); }
        }

        private static void OnHorizontalAxisLabelChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<string>(
                (string) e.OldValue,
                (string) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.HorizontalAxisLabelChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();

        }


        #endregion

        #region Min Property and Property Change Routed event

        public static readonly DependencyProperty MinProperty
            = DependencyProperty.Register(
                "Min", typeof (double), typeof (FunctionVisualizer),
                new PropertyMetadata(0.0, OnMinChanged, MinCoerceValue));

        public double Min
        {
            get { return (double) GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly RoutedEvent MinChangedEvent
            = EventManager.RegisterRoutedEvent(
                "MinChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<double>),
                typeof (FunctionVisualizer));

        private static object MinCoerceValue(DependencyObject d, object value)
        {
            var val = (double) value;
            var obj = (FunctionVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<double> MinChanged
        {
            add { AddHandler(MinChangedEvent, value); }
            remove { RemoveHandler(MinChangedEvent, value); }
        }

        private static void OnMinChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<double>(
                (double) e.OldValue,
                (double) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.MinChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region Max Property and Property Change Routed event

        public static readonly DependencyProperty MaxProperty
            = DependencyProperty.Register(
                "Max", typeof (double), typeof (FunctionVisualizer),
                new PropertyMetadata(0.0, OnMaxChanged, MaxCoerceValue));

        public double Max
        {
            get { return (double) GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly RoutedEvent MaxChangedEvent
            = EventManager.RegisterRoutedEvent(
                "MaxChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<double>),
                typeof (FunctionVisualizer));

        private static object MaxCoerceValue(DependencyObject d, object value)
        {
            var val = (double) value;
            var obj = (FunctionVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<double> MaxChanged
        {
            add { AddHandler(MaxChangedEvent, value); }
            remove { RemoveHandler(MaxChangedEvent, value); }
        }

        private static void OnMaxChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<double>(
                (double) e.OldValue,
                (double) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.MaxChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region SamplingCount Property and Property Change Routed event

        public static readonly DependencyProperty SamplingCountProperty
            = DependencyProperty.Register(
                "SamplingCount", typeof (int), typeof (FunctionVisualizer),
                new PropertyMetadata(10, OnSamplingCountChanged, SamplingCountCoerceValue));

        /// <summary>
        /// Gets or sets the sampling count.
        /// </summary>
        /// <value>
        /// The sampling count.
        /// </value>
        /// <remarks>
        /// Two samples are at <see cref="Min"/> and <see cref="Max"/>.
        /// </remarks>
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
                typeof (FunctionVisualizer));

        private static object SamplingCountCoerceValue(DependencyObject d, object value)
        {
            var val = (int) value;
            var obj = (FunctionVisualizer) d;


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
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<int>(
                (int) e.OldValue,
                (int) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.SamplingCountChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region TargetFunction Property and Property Change Routed event

        public static readonly DependencyProperty TargetFunctionProperty
            = DependencyProperty.Register(
                "TargetFunction", typeof (Func<double,double>), typeof (FunctionVisualizer),
                new PropertyMetadata(null, OnTargetFunctionChanged, TargetFunctionCoerceValue));

        public Func<double,double> TargetFunction
        {
            get { return (Func<double,double>) GetValue(TargetFunctionProperty); }
            set { SetValue(TargetFunctionProperty, value); }
        }

        public static readonly RoutedEvent TargetFunctionChangedEvent
            = EventManager.RegisterRoutedEvent(
                "TargetFunctionChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<Func<double,double>>),
                typeof (FunctionVisualizer));

        private static object TargetFunctionCoerceValue(DependencyObject d, object value)
        {
            var val = (Func<double,double>) value;
            var obj = (FunctionVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<Func<double,double>> TargetFunctionChanged
        {
            add { AddHandler(TargetFunctionChangedEvent, value); }
            remove { RemoveHandler(TargetFunctionChangedEvent, value); }
        }

        private static void OnTargetFunctionChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<Func<double,double>>(
                (Func<double,double>) e.OldValue,
                (Func<double,double>) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.TargetFunctionChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        #region GraphColor Property and Property Change Routed event

        public static readonly DependencyProperty GraphColorProperty
            = DependencyProperty.Register(
                "GraphColor", typeof (Color), typeof (FunctionVisualizer),
                new PropertyMetadata(Colors.Black, OnGraphColorChanged, GraphColorCoerceValue));

        public Color GraphColor
        {
            get { return (Color) GetValue(GraphColorProperty); }
            set { SetValue(GraphColorProperty, value); }
        }

        public static readonly RoutedEvent GraphColorChangedEvent
            = EventManager.RegisterRoutedEvent(
                "GraphColorChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<Color>),
                typeof (FunctionVisualizer));

        private static object GraphColorCoerceValue(DependencyObject d, object value)
        {
            var val = (Color) value;
            var obj = (FunctionVisualizer) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<Color> GraphColorChanged
        {
            add { AddHandler(GraphColorChangedEvent, value); }
            remove { RemoveHandler(GraphColorChangedEvent, value); }
        }

        private static void OnGraphColorChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FunctionVisualizer;
            var args = new RoutedPropertyChangedEventArgs<Color>(
                (Color) e.OldValue,
                (Color) e.NewValue);
            args.RoutedEvent = FunctionVisualizer.GraphColorChangedEvent;
            obj.RaiseEvent(args);
            obj.UpdateUi();
        }


        #endregion

        private bool suspend = false;

        public void UpdateUi()
        {
            if (suspend)
                return;

            if (Min >= Max)
                return;

            if (SamplingCount < 2)
                return;

            if (TargetFunction == null)
                return;

            var fnc = TargetFunction;

            double delta = (Max - Min)/(SamplingCount - 1);

            var data = new List<DataPoint>();

            for (int i = 0; i < SamplingCount; i++)
            {
                double t = Min + i * delta;

                if (t > Max) t = Max;// floating point
                if (t < Min) t = Min;// floating point

                data.Add(new DataPoint(t, fnc(t)));
            }

            var model = new PlotModel();

            var series = new LineSeries()
            {
                Title = "Series 1",
                MarkerType = MarkerType.Circle,
                Color = OxyColor.FromArgb(GraphColor.A, GraphColor.R, GraphColor.G, GraphColor.B),
                StrokeThickness = 2.0
            };

            series.Points.AddRange(data);

            if (!string.IsNullOrEmpty(VerticalAxisLabel))
            {
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = VerticalAxisLabel });
            }

            if (!string.IsNullOrEmpty(HorizontalAxisLabel))
            {
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = HorizontalAxisLabel });
            }

            model.Series.Add(series);

            plotter.Model = model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fnc"></param>
        /// <param name="x0">determines the minimum horizontal axis value</param>
        /// <param name="x1">determines the maximum horizontal axis value</param>
        public static void VisualizeInNewWindow(Func<double, double> fnc, double x0, double x1,int samplingCount=10)
        {
            var ctrl = new FunctionVisualizer();
            ctrl.suspend = true;

            ctrl.Min = x0;
            ctrl.Max = x1;
            ctrl.SamplingCount = samplingCount;
            ctrl.TargetFunction = fnc;

            var wnd = new Window();

            wnd.Content = ctrl;
            ctrl.suspend = false;
            ctrl.UpdateUi();

            wnd.ShowDialog();
        }
    }
}
