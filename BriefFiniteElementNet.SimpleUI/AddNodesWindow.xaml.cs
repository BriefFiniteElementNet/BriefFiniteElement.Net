using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BriefFiniteElementNet.Validation;

namespace BriefFiniteElementNet.SimpleUI
{
    /// <summary>
    /// Interaction logic for AddNodesWindow.xaml
    /// </summary>
    public partial class AddNodesWindow : Window
    {
        public AddNodesWindow()
        {
            InitializeComponent();

            this.DataContext =
                this.Context =
                    new AddNodesWindowDataContext()
                    {
                        Infos = new ObservableCollection<AddNodesWindowDataContext.NodeAddInfo>()
                    };
        }

        public AddNodesWindowDataContext Context;

        public class AddNodesWindowDataContext : INotifyPropertyChanged
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

            #region Target Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public Model Target
            {
                get { return target; }
                set
                {
                    if (AreEqualObjects(target, value))
                        return;

                    var _fieldOldValue = target;

                    target = value;

                    AddNodesWindowDataContext.OnTargetChanged(this, new PropertyValueChangedEventArgs<Model>(_fieldOldValue, value));

                    this.OnPropertyChanged("Target");
                }
            }

            private Model target;

            public EventHandler<PropertyValueChangedEventArgs<Model>> TargetChanged;

            public static void OnTargetChanged(object sender, PropertyValueChangedEventArgs<Model> e)
            {
                var obj = sender as AddNodesWindowDataContext;

                if (obj.TargetChanged != null)
                    obj.TargetChanged(obj, e);


            }

            #endregion

            #region Infos Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ObservableCollection<NodeAddInfo> Infos
            {
                get { return infos; }
                set
                {
                    if (AreEqualObjects(infos, value))
                        return;

                    var _fieldOldValue = infos;

                    infos = value;

                    AddNodesWindowDataContext.OnInfosChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<NodeAddInfo>>(_fieldOldValue, value));

                    this.OnPropertyChanged("Infos");
                }
            }

            private ObservableCollection<NodeAddInfo> infos;

            public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<NodeAddInfo>>> InfosChanged;

            public static void OnInfosChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<NodeAddInfo>> e)
            {
                var obj = sender as AddNodesWindowDataContext;

                if (obj.InfosChanged != null)
                    obj.InfosChanged(obj, e);


            }

            #endregion

            public class NodeAddInfo : INotifyPropertyChanged
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

                #region X Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public double X
                {
                    get { return x; }
                    set
                    {
                        if (AreEqualObjects(x, value))
                            return;

                        var _fieldOldValue = x;

                        x = value;

                        NodeAddInfo.OnXChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                        this.OnPropertyChanged("X");
                    }
                }

                private double x;

                public EventHandler<PropertyValueChangedEventArgs<double>> XChanged;

                public static void OnXChanged(object sender, PropertyValueChangedEventArgs<double> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.XChanged != null)
                        obj.XChanged(obj, e);


                }

                #endregion

                #region Y Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public double Y
                {
                    get { return y; }
                    set
                    {
                        if (AreEqualObjects(y, value))
                            return;

                        var _fieldOldValue = y;

                        y = value;

                        NodeAddInfo.OnYChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                        this.OnPropertyChanged("Y");
                    }
                }

                private double y;

                public EventHandler<PropertyValueChangedEventArgs<double>> YChanged;

                public static void OnYChanged(object sender, PropertyValueChangedEventArgs<double> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.YChanged != null)
                        obj.YChanged(obj, e);


                }

                #endregion

                #region Z Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public double Z
                {
                    get { return z; }
                    set
                    {
                        if (AreEqualObjects(z, value))
                            return;

                        var _fieldOldValue = z;

                        z = value;

                        NodeAddInfo.OnZChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                        this.OnPropertyChanged("Z");
                    }
                }

                private double z;

                public EventHandler<PropertyValueChangedEventArgs<double>> ZChanged;

                public static void OnZChanged(object sender, PropertyValueChangedEventArgs<double> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.ZChanged != null)
                        obj.ZChanged(obj, e);


                }

                #endregion

                
                #region Dx Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public DofConstraint Dx
                {
                    get { return dx; }
                    set
                    {
                        if (AreEqualObjects(dx, value))
                            return;

                        var _fieldOldValue = dx;

                        dx = value;

                        NodeAddInfo.OnDxChanged(this, new PropertyValueChangedEventArgs<DofConstraint>(_fieldOldValue, value));

                        this.OnPropertyChanged("Dx");
                    }
                }

                private DofConstraint dx;

                public EventHandler<PropertyValueChangedEventArgs<DofConstraint>> DxChanged;

                public static void OnDxChanged(object sender, PropertyValueChangedEventArgs<DofConstraint> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.DxChanged != null)
                        obj.DxChanged(obj, e);


                }

                #endregion

                #region Dy Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public DofConstraint Dy
                {
                    get { return dy; }
                    set
                    {
                        if (AreEqualObjects(dy, value))
                            return;

                        var _fieldOldValue = dy;

                        dy = value;

                        NodeAddInfo.OnDyChanged(this, new PropertyValueChangedEventArgs<DofConstraint>(_fieldOldValue, value));

                        this.OnPropertyChanged("Dy");
                    }
                }

                private DofConstraint dy;

                public EventHandler<PropertyValueChangedEventArgs<DofConstraint>> DyChanged;

                public static void OnDyChanged(object sender, PropertyValueChangedEventArgs<DofConstraint> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.DyChanged != null)
                        obj.DyChanged(obj, e);


                }

                #endregion

                #region Dz Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public DofConstraint Dz
                {
                    get { return dz; }
                    set
                    {
                        if (AreEqualObjects(dz, value))
                            return;

                        var _fieldOldValue = dz;

                        dz = value;

                        NodeAddInfo.OnDzChanged(this, new PropertyValueChangedEventArgs<DofConstraint>(_fieldOldValue, value));

                        this.OnPropertyChanged("Dz");
                    }
                }

                private DofConstraint dz;

                public EventHandler<PropertyValueChangedEventArgs<DofConstraint>> DzChanged;

                public static void OnDzChanged(object sender, PropertyValueChangedEventArgs<DofConstraint> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.DzChanged != null)
                        obj.DzChanged(obj, e);


                }

                #endregion

                #region Rx Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public DofConstraint Rx
                {
                    get { return rx; }
                    set
                    {
                        if (AreEqualObjects(rx, value))
                            return;

                        var _fieldOldValue = rx;

                        rx = value;

                        NodeAddInfo.OnRxChanged(this, new PropertyValueChangedEventArgs<DofConstraint>(_fieldOldValue, value));

                        this.OnPropertyChanged("Rx");
                    }
                }

                private DofConstraint rx;

                public EventHandler<PropertyValueChangedEventArgs<DofConstraint>> RxChanged;

                public static void OnRxChanged(object sender, PropertyValueChangedEventArgs<DofConstraint> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.RxChanged != null)
                        obj.RxChanged(obj, e);


                }

                #endregion

                #region Ry Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public DofConstraint Ry
                {
                    get { return ry; }
                    set
                    {
                        if (AreEqualObjects(ry, value))
                            return;

                        var _fieldOldValue = ry;

                        ry = value;

                        NodeAddInfo.OnRyChanged(this, new PropertyValueChangedEventArgs<DofConstraint>(_fieldOldValue, value));

                        this.OnPropertyChanged("Ry");
                    }
                }

                private DofConstraint ry;

                public EventHandler<PropertyValueChangedEventArgs<DofConstraint>> RyChanged;

                public static void OnRyChanged(object sender, PropertyValueChangedEventArgs<DofConstraint> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.RyChanged != null)
                        obj.RyChanged(obj, e);


                }

                #endregion

                #region Rz Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public DofConstraint Rz
                {
                    get { return rz; }
                    set
                    {
                        if (AreEqualObjects(rz, value))
                            return;

                        var _fieldOldValue = rz;

                        rz = value;

                        NodeAddInfo.OnRzChanged(this, new PropertyValueChangedEventArgs<DofConstraint>(_fieldOldValue, value));

                        this.OnPropertyChanged("Rz");
                    }
                }

                private DofConstraint rz;

                public EventHandler<PropertyValueChangedEventArgs<DofConstraint>> RzChanged;

                public static void OnRzChanged(object sender, PropertyValueChangedEventArgs<DofConstraint> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.RzChanged != null)
                        obj.RzChanged(obj, e);


                }

                #endregion

                #region Label Property and field

                [Obfuscation(Exclude = true, ApplyToMembers = false)]
                public string Label
                {
                    get { return label; }
                    set
                    {
                        if (AreEqualObjects(label, value))
                            return;

                        var _fieldOldValue = label;

                        label = value;

                        NodeAddInfo.OnLabelChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                        this.OnPropertyChanged("Label");
                    }
                }

                private string label;

                public EventHandler<PropertyValueChangedEventArgs<string>> LabelChanged;

                public static void OnLabelChanged(object sender, PropertyValueChangedEventArgs<string> e)
                {
                    var obj = sender as NodeAddInfo;

                    if (obj.LabelChanged != null)
                        obj.LabelChanged(obj, e);


                }

                #endregion


            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Context.Infos.Add(new AddNodesWindowDataContext.NodeAddInfo());
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

            var locs = Context.Infos.Select(i => new Point(i.X, i.Y, i.Z));

            if (locs.Distinct().Count() != locs.Count())
            {
                var res = MessageBox.Show("duplicated nodes with same location found!");
                return;
            }

            foreach (var nfo in Context.Infos)
            {
                var nde = new Node();

                nde.Constraints = new Constraint(nfo.Dx, nfo.Dy, nfo.Dz, nfo.Rx, nfo.Ry, nfo.Rz);
                nde.Location = new Point(nfo.X, nfo.Y, nfo.Z);
                nde.Label = nfo.Label;

                Context.Target.Nodes.Add(nde);
            }

            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            var txt = Clipboard.GetText();

            var lines =
                txt.Split('\r')
                    .SelectMany(i => i.Split('\n'))
                    .Select(i => i.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

            var toConstraint = new Func<string, DofConstraint>(i =>
            {
                if (i == "0")
                    return DofConstraint.Released;

                if (i == "1")
                    return DofConstraint.Fixed;

                if (i.ToLower() == "r")
                    return DofConstraint.Released;

                if (i.ToLower() == "f")
                    return DofConstraint.Fixed;


                if (i.ToLower() == "released")
                    return DofConstraint.Released;

                if (i.ToLower() == "fixed")
                    return DofConstraint.Fixed;

                throw new Exception(string.Format("Unknown fixity: {0}", i));

            });

            foreach (var line in lines)
            {
                var sp =
                    line.Split(',').SelectMany(i => i.Split('\t'))
                        .Select(i => i.Trim())
                        .ToArray();


                if (sp.Length < 3)
                    throw new Exception();


                var nfo = new AddNodesWindowDataContext.NodeAddInfo();

                nfo.X = sp[0].ToDouble();
                nfo.Y = sp[1].ToDouble();
                nfo.Z = sp[2].ToDouble();

                if (sp.Length != 3 && sp.Length != 9 && sp.Length != 10)
                {
                    throw new Exception("Invalid number of values");
                }

                if (sp.Length > 3)
                {
                    nfo.Dx = toConstraint(sp[3]);
                    nfo.Dy = toConstraint(sp[4]);
                    nfo.Dz = toConstraint(sp[5]);

                    nfo.Rx = toConstraint(sp[6]);
                    nfo.Ry = toConstraint(sp[7]);
                    nfo.Rz = toConstraint(sp[8]);
                    
                }

                if (sp.Length == 10)
                    nfo.Label = sp[9];

                Context.Infos.Add(nfo);
            }
        }
    }
}
