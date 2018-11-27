using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.Controls
{
    /// <summary>
    /// Interaction logic for WPFTraceListener.xaml
    /// </summary>
    public partial class WpfTraceListener : Window,ITraceListener
    {
        public WpfTraceListener()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Writes the specified record into window.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(TraceRecord record)
        {
            this.Dispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    this.TraceRecords.Add(record);
                    ProcessUITasks();
                }));

            
        }

        public void ProcessUITasks()
        {
            var frame = new DispatcherFrame();

            this.Dispatcher.Invoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<TraceRecord> Records
        {
            get { return new ReadOnlyCollection<TraceRecord>(this.TraceRecords); }
        }

        #region TraceRecords Property and Property Change Routed event

        public static readonly DependencyProperty TraceRecordsProperty
            = DependencyProperty.Register(
                "TraceRecords", typeof (ObservableCollection<TraceRecord>), typeof (WpfTraceListener),
                new PropertyMetadata(new ObservableCollection<TraceRecord>(), OnTraceRecordsChanged, TraceRecordsCoerceValue));

        public ObservableCollection<TraceRecord> TraceRecords
        {
            get { return (ObservableCollection<TraceRecord>) GetValue(TraceRecordsProperty); }
            set { SetValue(TraceRecordsProperty, value); }
        }

        public static readonly RoutedEvent TraceRecordsChangedEvent
            = EventManager.RegisterRoutedEvent(
                "TraceRecordsChanged",
                RoutingStrategy.Direct,
                typeof (RoutedPropertyChangedEventHandler<ObservableCollection<TraceRecord>>),
                typeof (WpfTraceListener));

        private static object TraceRecordsCoerceValue(DependencyObject d, object value)
        {
            var val = (ObservableCollection<TraceRecord>) value;
            var obj = (WpfTraceListener) d;


            return value;
        }

        public event RoutedPropertyChangedEventHandler<ObservableCollection<TraceRecord>> TraceRecordsChanged
        {
            add { AddHandler(TraceRecordsChangedEvent, value); }
            remove { RemoveHandler(TraceRecordsChangedEvent, value); }
        }

        private static void OnTraceRecordsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var obj = d as WpfTraceListener;
            var args = new RoutedPropertyChangedEventArgs<ObservableCollection<TraceRecord>>(
                (ObservableCollection<TraceRecord>) e.OldValue,
                (ObservableCollection<TraceRecord>) e.NewValue);
            args.RoutedEvent = WpfTraceListener.TraceRecordsChangedEvent;
            obj.RaiseEvent(args);
            
        }


        #endregion


        public static WpfTraceListener CreateModelTrace(Model model)
        {
            var buf = new WpfTraceListener();
            model.Trace.Listeners.Add(buf);
            return buf;
        }

        public static void ShowModelTrace(Model model)
        {
            try
            {
                showModelTrace(model);
                return;
                var thr = new Thread(showModelTrace);
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start(model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Synced!
        /// </summary>
        /// <param name="model">The model.</param>
        private static void showModelTrace(object model)
        {
            var mdl = model as Model;
            var wnd = new WpfTraceListener();
            mdl.Trace.Listeners.Add(wnd);
            wnd.Show();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
