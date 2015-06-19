using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace BriefFiniteElementNet.Controls
{
    public class PropertyHelper
    {
        public static void Populate(DataGrid dg, object obj)
        {
            var props = obj.GetType().GetProperties();
            dg.AutoGenerateColumns = false;
            foreach (var inf in props)
            {
                DataGridTextColumn cl = null;
                if (!inf.CanRead)
                    continue;

                dg.Columns.Add(cl = new DataGridTextColumn() {Header = inf.Name, Binding = new Binding(inf.Name)});

                if (!inf.CanWrite)
                    cl.IsReadOnly = true;
            }

        }
    }
}
