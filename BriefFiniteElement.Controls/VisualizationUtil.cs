using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BriefFiniteElementNet.Controls
{
    public static class VisualizationUtil
    {
        public static void VisualizeInNewWindow(DataTable tbl,bool dialog=false)
        {
            var wnd = new Window();
            var grd = new DataGrid();
            grd.ItemsSource = tbl.AsDataView();

            wnd.Content = grd;

            if (dialog)
                wnd.ShowDialog();
            else
                wnd.Show();

        }
    }
}
