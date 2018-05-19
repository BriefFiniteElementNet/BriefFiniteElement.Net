using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BriefFiniteElementNet.TestUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var types = this.GetType().Assembly.GetTypes().Where(i => typeof(Window).IsAssignableFrom(i)).Where(i => i != this.GetType()).ToArray();

            foreach (var tp in types)
            {
                var btn = new Button() { Content = tp.Name };
                stkMain.Children.Add(btn);

                btn.Click += (a, b) =>
                {
                    var wnd = Activator.CreateInstance(tp) as Window;

                    if (wnd != null)
                        wnd.ShowDialog();
                };
            }

        }
    }
}
