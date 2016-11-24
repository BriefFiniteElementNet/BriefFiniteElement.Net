using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BriefFiniteElementNet.Controls
{
    public class PropertyHelper
    {
        public static void Populate(DataGrid dg, object obj)
        {
            PropertyInfo[] infs = new PropertyInfo[0];

            


            if (obj is IEnumerable)
            {
                var first = (obj as IEnumerable).Cast<object>().FirstOrDefault();
                if (first != null)

                    infs = first.GetType().GetProperties();
            }

            else
            {
                infs = obj.GetType().GetProperties();

            }


            {
                var props = infs;
                dg.AutoGenerateColumns = false;

                foreach (var inf in props)
                {
                    DataGridTextColumn cl = null;
                    if (!inf.CanRead)
                        continue;

                    dg.Columns.Add(
                        cl =
                            new DataGridTextColumn()
                            {
                                Header = inf.Name,
                                Binding = new Binding(inf.Name) {Converter = new CustomTostring()}
                            });

                    if (!inf.CanWrite)
                        cl.IsReadOnly = true;
                }

                dg.MouseDoubleClick += (sender, args) =>
                {
                    var cl = (sender as DataGrid).CurrentCell;

                    if (cl == null)
                        return;

                    var t = cl.Item;


                    if (t == null)
                        return;

                    var d = (cl.Column as DataGridTextColumn);

                    if (d == null)
                        return;


                    var f = d.Binding as Binding;

                    if (f == null)
                        return;

                    var g = f.Path;


                    var prp = t.GetType().GetProperty(g.Path);

                    if (prp == null)
                        return;

                    var val = prp.GetValue(t, null);

                    //(cl.Column as DataGridTextColumn).Binding.
                    if (t != null)
                        BrowseObjectProperties(val);
                };
            }

           
        }


        public class CustomTostring : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (ReferenceEquals(value, null))
                    return value;

                if (value is IEnumerable)
                {
                    return string.Format("Count = {0}", (value as IEnumerable).Cast<object>().Count());
                }

                return value;

                throw new NotImplementedException();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        public static void BrowseObjectProperties(object obj)
        {
            var grd = new DataGrid() {SelectionMode = DataGridSelectionMode.Single};

            PropertyHelper.Populate(grd, obj);

            if (obj is IEnumerable)
                grd.ItemsSource = obj as IEnumerable;
            else
                grd.ItemsSource = new[] {obj};




            var wnd = new Window();
            wnd.Content = grd;
            wnd.Title = obj.GetType().FullName;
            wnd.ShowDialog();

        }


    }
}
