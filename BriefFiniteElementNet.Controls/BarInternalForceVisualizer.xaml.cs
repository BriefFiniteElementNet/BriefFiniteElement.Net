using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BriefFiniteElementNet.Elements;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Interaction logic for BarInternalForce.xaml
    /// </summary>
    public partial class BarInternalForceVisualizer : UserControl
    {
        public BarInternalForceVisualizer()
        {
            InitializeComponent();
            this.DataContext = this.Context = new BarInternalForceContext();

            Context.ComponentChanged += (a, b) => UpdateUi();
            Context.TypeChanged += (a, b) => UpdateUi();
            Context.TargetElementChanged += (a, b) =>
            {
                UpdateUi();
                Context.ReloadLoadCases();
            };

            Context.SelectedLoadCaseChanged += (a, b) => UpdateUi();
            Context.init();
        }

        public void UpdateUi()
        {
            var bar = Context.TargetElement;

            for (var i = plotter.Children.Count - 1; i >= 0; i--)
                if (plotter.Children[i] is LineGraph)
                    plotter.Children.RemoveAt(i);

            if (bar == null)
                return;

            var elementDiscPonits = Context.TargetElement.GetInternalForceDiscretationPoints(Context.SelectedLoadCase).Select(i => i.Xi).ToArray();

            var samples = 100;

            var lst = new List<double>(elementDiscPonits);

            //remove discrete points and add a little before and after

            var eps = 1e-10;

            foreach (var discPonit in elementDiscPonits)
            {
                lst.Add(discPonit - eps);
                lst.Add(discPonit + eps);
            }

            for (int i = 0; i <= samples; i++)
            {
                var c = i * 1.0 / samples;

                var xi = c*2 - 1;

                lst.Add(xi);
            }

            var l2 = lst.Where(i => i >= -1 && i <= 1).Where(i => !elementDiscPonits.Contains(i)).Distinct().ToArray();

            Array.Sort(l2);

            var pts = new List<Point>();

            var lc = Context.SelectedLoadCase;

            for (var i = 0; i < l2.Length; i++)
            {
                var xi = l2[i];
                var x = Context.TargetElement.IsoCoordsToLocalCoords(xi)[0];

                Force frc;

                if (Context.Type == BarInternalForceContext.ForceType.Approximate)
                    frc = bar.GetInternalForceAt(xi, lc);
                else
                    frc = bar.GetExactInternalForceAt(xi, lc);

                var y = 0.0;

                switch (Context.Component)
                {
                    case BarInternalForceContext.ForceComponent.Fx:
                        y = frc.Fx;
                        break;
                    case BarInternalForceContext.ForceComponent.Fy:
                        y = frc.Fy;
                        break;
                    case BarInternalForceContext.ForceComponent.Fz:
                        y = frc.Fz;
                        break;
                    case BarInternalForceContext.ForceComponent.Mx:
                        y = frc.Mx;
                        break;
                    case BarInternalForceContext.ForceComponent.My:
                        y = frc.My;
                        break;
                    case BarInternalForceContext.ForceComponent.Mz:
                        y = frc.Mz;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                pts.Add(new Point(x, y, 0));
            }

            var src = new ObservableDataSource<Point>(pts);

            src.SetXMapping(i => i.X);
            src.SetYMapping(i => i.Y);


            var gr = new LineGraph(src);
            
            gr.Stroke = Brushes.Black;
            gr.StrokeThickness = 2;
            gr.Name = Context.Component.ToString();

            plotter.Children.Add(gr);
            //plotter.AddLineGraph(src, Colors.Black, 2, Context.Component.ToString());
            Microsoft.Research.DynamicDataDisplay.Charts.Legend.SetDescription(gr, Context.Component.ToString());
        }

        private BarInternalForceContext Context;

        public class BarInternalForceContext : INotifyPropertyChanged
        {
            #region INotifyPropertyChanged members and helpers

            public event PropertyChangedEventHandler PropertyChanged;

            private static bool AreEqualObjects(object obj1, object obj2)
            {
                var obj1Null = ReferenceEquals(obj1, null);
                var obj2Null = ReferenceEquals(obj2, null);

                if (obj1Null && obj2Null)
                    return true;

                if (obj1Null || obj2Null)
                    return false;

                if (obj1.GetType() != obj2.GetType())
                    return false;

                if (ReferenceEquals(obj1, obj2))
                    return true;

                return obj1.Equals(obj2);
            }

            private void OnPropertyChanged(params string[] propertyNames)
            {
                if (propertyNames == null)
                    return;

                if (this.PropertyChanged != null)
                    foreach (var propertyName in propertyNames)
                        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region Component Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ForceComponent Component
            {
                get { return component; }
                set
                {
                    if (AreEqualObjects(component, value))
                        return;

                    var _fieldOldValue = component;

                    component = value;

                    BarInternalForceContext.OnComponentChanged(this, new PropertyValueChangedEventArgs<ForceComponent>(_fieldOldValue, value));

                    this.OnPropertyChanged("Component");
                }
            }

            private ForceComponent component;

            public EventHandler<PropertyValueChangedEventArgs<ForceComponent>> ComponentChanged;

            private static void OnComponentChanged(object sender, PropertyValueChangedEventArgs<ForceComponent> e)
            {
                var obj = sender as BarInternalForceContext;

                if (obj.ComponentChanged != null)
                    obj.ComponentChanged(obj, e);
            }

            #endregion

            #region Type Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ForceType Type
            {
                get { return type; }
                set
                {
                    if (AreEqualObjects(type, value))
                        return;

                    var _fieldOldValue = type;

                    type = value;

                    BarInternalForceContext.OnTypeChanged(this, new PropertyValueChangedEventArgs<ForceType>(_fieldOldValue, value));

                    this.OnPropertyChanged("Type");
                }
            }

            private ForceType type;

            public EventHandler<PropertyValueChangedEventArgs<ForceType>> TypeChanged;

            private static void OnTypeChanged(object sender, PropertyValueChangedEventArgs<ForceType> e)
            {
                var obj = sender as BarInternalForceContext;

                if (obj.TypeChanged != null)
                    obj.TypeChanged(obj, e);
            }

            #endregion

            #region TargetElement Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public BarElement TargetElement
            {
                get { return targetElement; }
                set
                {
                    if (AreEqualObjects(targetElement, value))
                        return;

                    var _fieldOldValue = targetElement;

                    targetElement = value;

                    BarInternalForceContext.OnTargetElementChanged(this, new PropertyValueChangedEventArgs<BarElement>(_fieldOldValue, value));

                    this.OnPropertyChanged("TargetElement");
                }
            }

            private BarElement targetElement;

            public EventHandler<PropertyValueChangedEventArgs<BarElement>> TargetElementChanged;

            private static void OnTargetElementChanged(object sender, PropertyValueChangedEventArgs<BarElement> e)
            {
                var obj = sender as BarInternalForceContext;

                if (obj.TargetElementChanged != null)
                    obj.TargetElementChanged(obj, e);
            }

            #endregion

            #region AllLoadCases Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ObservableCollection<LoadCase> AllLoadCases
            {
                get { return _AllLoadCases; }
                set
                {
                    if (AreEqualObjects(_AllLoadCases, value))
                        return;

                    var _fieldOldValue = _AllLoadCases;

                    _AllLoadCases = value;

                    BarInternalForceContext.OnAllLoadCasesChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<LoadCase>>(_fieldOldValue, value));

                    this.OnPropertyChanged("AllLoadCases");
                }
            }

            private ObservableCollection<LoadCase> _AllLoadCases;

            public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<LoadCase>>> AllLoadCasesChanged;

            public static void OnAllLoadCasesChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<LoadCase>> e)
            {
                var obj = sender as BarInternalForceContext;

                if (obj.AllLoadCasesChanged != null)
                    obj.AllLoadCasesChanged(obj, e);
            }

            #endregion

            #region SelectedLoadCase Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public LoadCase SelectedLoadCase
            {
                get { return _SelectedLoadCase; }
                set
                {
                    if (AreEqualObjects(_SelectedLoadCase, value))
                        return;

                    var _fieldOldValue = _SelectedLoadCase;

                    _SelectedLoadCase = value;

                    BarInternalForceContext.OnSelectedLoadCaseChanged(this, new PropertyValueChangedEventArgs<LoadCase>(_fieldOldValue, value));

                    this.OnPropertyChanged("SelectedLoadCase");
                }
            }

            private LoadCase _SelectedLoadCase;

            public EventHandler<PropertyValueChangedEventArgs<LoadCase>> SelectedLoadCaseChanged;

            public static void OnSelectedLoadCaseChanged(object sender, PropertyValueChangedEventArgs<LoadCase> e)
            {
                var obj = sender as BarInternalForceContext;

                if (obj.SelectedLoadCaseChanged != null)
                    obj.SelectedLoadCaseChanged(obj, e);
            }

            #endregion


            public enum ForceComponent
            {
                Fx,
                Fy,
                Fz,
                Mx,
                My,
                Mz
            }

            public enum ForceType
            {
                Approximate,
                Exact
            }


            public void init()
            {
                ReloadLoadCases();
            }

            public void ReloadLoadCases()
            {
                if (AllLoadCases == null)
                    AllLoadCases = new ObservableCollection<LoadCase>();
                else
                    AllLoadCases.Clear();


                AllLoadCases.Add(LoadCase.DefaultLoadCase);

                if(targetElement!=null)
                {
                    foreach (var l in targetElement.Loads)
                        if (!AllLoadCases.Contains(l.Case))
                            AllLoadCases.Add(l.Case);
                }

                SelectedLoadCase = LoadCase.DefaultLoadCase;
            }
        }

        public static void VisualizeInNewWindow(BarElement elm)
        {
            var wnd = new Window();

            var ctrl = new BarInternalForceVisualizer();
            wnd.Content = ctrl;
            ctrl.Context.TargetElement = elm;

            wnd.ShowDialog();
        }
    }

    public class PropertyValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }

        public PropertyValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public PropertyValueChangedEventArgs()
        {
        }
    }
}
