using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BriefFiniteElementNet.SimpleUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this.Context = new MainWindowDataContext();
            Context.Model = new Model();

        }

        public MainWindowDataContext Context;

        private void OpenBin_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            var res = dlg.ShowDialog();

            if (res == null || !res.Value)
                return;


            try
            {
                var buf = Model.Load(dlg.FileName);
                Context.Model = buf;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }

        private void OpenXml_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            var res = dlg.ShowDialog();

            if (res == null || !res.Value)
                return;


            try
            {
                var str = new FileStream(dlg.FileName, FileMode.Open);
                var buf = XmlIO.ReadFromXml(str);
                str.Close();

                Context.Model = buf;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveFrame3dd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();

            var res = dlg.ShowDialog();

            if (res == null || !res.Value)
                return;

            try
            {
                var valdt = new Validation.Frame3DDValidator(null);
                valdt.Model = Context.Model;
                //var content = valdt.Createf3DDInputFile();
                //File.WriteAllText(dlg.FileName, content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void SaveXml_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CreateGrid_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new Window2();

            var res = wnd.ShowDialog();

            if (!res.HasValue || !res.Value)
                return;

            var grd = StructureGenerator.Generate3DFrameElementGrid(wnd.Context.XSpans + 1, wnd.Context.YSpans + 1,
                wnd.Context.ZSpans + 1);

            if (wnd.Context.RandomLoads)
                StructureGenerator.AddRandomiseLoading(grd, true, false);

            Context.Model = grd;
        }

        private void AddNodes_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddNodesWindow();
            wnd.Context.Target = this.Context.Model;

            wnd.ShowDialog();

            VisualizerControl.UpdateUi();
        }
    }

    public class MainWindowDataContext : INotifyPropertyChanged
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

        #region Model Property and field

        [Obfuscation(Exclude = true, ApplyToMembers = false)]
        public Model Model
        {
            get { return model; }
            set
            {
                if (AreEqualObjects(model, value))
                    return;

                var _fieldOldValue = model;

                model = value;

                MainWindowDataContext.OnModelChanged(this,
                    new PropertyValueChangedEventArgs<Model>(_fieldOldValue, value));

                this.OnPropertyChanged("Model");
            }
        }

        private Model model;

        public EventHandler<PropertyValueChangedEventArgs<Model>> ModelChanged;

        public static void OnModelChanged(object sender, PropertyValueChangedEventArgs<Model> e)
        {
            var obj = sender as MainWindowDataContext;

            if (obj.ModelChanged != null)
                obj.ModelChanged(obj, e);
        }

        #endregion
    }
}