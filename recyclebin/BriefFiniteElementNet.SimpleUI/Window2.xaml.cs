using System;
using System.Collections.Generic;
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

namespace BriefFiniteElementNet.SimpleUI
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            this.DataContext = Context = new Window2DataContext();
        }

        public Window2DataContext Context;

        public class Window2DataContext : INotifyPropertyChanged
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

            #region XSpans Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public int XSpans
            {
                get { return xSpans; }
                set
                {
                    if (AreEqualObjects(xSpans, value))
                        return;

                    var _fieldOldValue = xSpans;

                    xSpans = value;

                    Window2DataContext.OnXSpansChanged(this,
                        new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

                    this.OnPropertyChanged("XSpans");
                }
            }

            private int xSpans = 2;

            public EventHandler<PropertyValueChangedEventArgs<int>> XSpansChanged;

            public static void OnXSpansChanged(object sender, PropertyValueChangedEventArgs<int> e)
            {
                var obj = sender as Window2DataContext;

                if (obj.XSpansChanged != null)
                    obj.XSpansChanged(obj, e);

                if (obj.XSpans < 1)
                    obj.XSpans = 1;
            }

            #endregion

            #region YSpans Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public int YSpans
            {
                get { return ySpans; }
                set
                {
                    if (AreEqualObjects(ySpans, value))
                        return;

                    var _fieldOldValue = ySpans;

                    ySpans = value;

                    Window2DataContext.OnYSpansChanged(this,
                        new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

                    this.OnPropertyChanged("YSpans");
                }
            }

            private int ySpans = 3;

            public EventHandler<PropertyValueChangedEventArgs<int>> YSpansChanged;

            public static void OnYSpansChanged(object sender, PropertyValueChangedEventArgs<int> e)
            {
                var obj = sender as Window2DataContext;

                if (obj.YSpansChanged != null)
                    obj.YSpansChanged(obj, e);

                if (obj.YSpans < 1)
                    obj.YSpans = 1;
            }

            #endregion

            #region ZSpans Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public int ZSpans
            {
                get { return zSpans; }
                set
                {
                    if (AreEqualObjects(zSpans, value))
                        return;

                    var _fieldOldValue = zSpans;

                    zSpans = value;

                    Window2DataContext.OnZSpansChanged(this,
                        new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

                    this.OnPropertyChanged("ZSpans");
                }
            }

            private int zSpans = 4;

            public EventHandler<PropertyValueChangedEventArgs<int>> ZSpansChanged;

            public static void OnZSpansChanged(object sender, PropertyValueChangedEventArgs<int> e)
            {
                var obj = sender as Window2DataContext;

                if (obj.ZSpansChanged != null)
                    obj.ZSpansChanged(obj, e);

                if (obj.ZSpans < 1)
                    obj.ZSpans = 1;
            }

            #endregion

            #region RandomLoads Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public bool RandomLoads
            {
                get { return randomLoads; }
                set
                {
                    if (AreEqualObjects(randomLoads, value))
                        return;

                    var _fieldOldValue = randomLoads;

                    randomLoads = value;

                    Window2DataContext.OnRandomLoadsChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

                    this.OnPropertyChanged("RandomLoads");
                }
            }

            private bool randomLoads;

            public EventHandler<PropertyValueChangedEventArgs<bool>> RandomLoadsChanged;

            public static void OnRandomLoadsChanged(object sender, PropertyValueChangedEventArgs<bool> e)
            {
                var obj = sender as Window2DataContext;

                if (obj.RandomLoadsChanged != null)
                    obj.RandomLoadsChanged(obj, e);


            }

            #endregion
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}