using CSparse.Double;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Interaction logic for MatrixVisualizerControl.xaml
    /// </summary>
    public partial class MatrixVisualizerControl : UserControl
    {
        public MatrixVisualizerControl()
        {
            InitializeComponent();
        }

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString(CultureInfo.CurrentCulture);
        }

        private void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
        {
            var src = e.Source as DataGridCell;

            //DataGrid.cells src.Column
            //e.Row.Header = (e.Row.GetIndex()).ToString();
        }


        public static void VisualizeInNewWindow(DenseMatrix matrix, string title = "")
        {
            var wnd = new Window() { Title = title };
            var mtxCtrl = new MatrixVisualizerControl();
            mtxCtrl.VisualizeMatrix(matrix);

            wnd.Content = mtxCtrl;

            wnd.Show();
        }

        public static void VisualizeInNewWindow(DenseMatrix matrix, string title ,bool showDialog)
        {
            var wnd = new Window() { Title = title };
            var mtxCtrl = new MatrixVisualizerControl();
            mtxCtrl.VisualizeMatrix(matrix);

            wnd.Content = mtxCtrl;

            if (showDialog)
                wnd.ShowDialog();
            else
                wnd.Show();
        }

        public void VisualizeMatrix(DenseMatrix mtx)
        {
            var tbl = new DataTable();
            tbl.Locale = CultureInfo.InvariantCulture;
            target = mtx;

            for (var j = 0; j < mtx.ColumnCount; j++)
            {
                tbl.Columns.Add(j.ToString(CultureInfo.CurrentCulture), typeof(double));
            }

            for (var i = 0; i < mtx.RowCount; i++)
            {
                tbl.Rows.Add(mtx.Row(i));
            }

            DataGrid.ItemsSource = tbl.DefaultView;
        }

        private DenseMatrix target;

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var sb = new StringBuilder();

                for (int i = 0; i < target.RowCount; i++)
                {
                    for (int j = 0; j < target.ColumnCount; j++)
                    {
                        sb.Append(target[i, j]);
                        sb.Append(",");
                    }

                    sb.Append(";");
                }


                Clipboard.SetText(sb.ToString().Replace(",;", ";"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
