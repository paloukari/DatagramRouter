using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Corp.TestTcpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker _worker;
        string _port = "";
        string _IP = "";
        int _Messages = 1;
        MessageType _messageType ;
        int _concurrentTests = 0;
        string _dataToSend = null;
        bool _waitForServerResponse = false;
        int _concurrentTestsCompleted = 0;
        int _maxDegreeOfParallelism = 1;
        internal bool EnableLogs = false;
        int _threadParallelism = 0;

        public MainWindow()
        {
            InitializeComponent();

            Log.Initialize(this);
        }

        #region UI
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            InitTest();

            _dataToSend = null;

            RunTest(new object[] { btnConnect, rctConnectivity, progressBar1, progressBar2 });
        }

        private void btnSendData_Click(object sender, RoutedEventArgs e)
        {
            InitTest();

            RunTest(new object[] { btnSendData, rctSendData, progressBar1, progressBar2 });
        }

        private void btnSendDataWithResponse_Click(object sender, RoutedEventArgs e)
        {
            InitTest();

            _waitForServerResponse = true;

            RunTest(new object[] { btnSendDataWithResponse, rctSendWithResponse, progressBar1, progressBar2, tbServerResponse });
        }

        private void chbEnableLogs_Checked(object sender, RoutedEventArgs e)
        {
            baseGrid.RowDefinitions[1].Height = new GridLength(7.0f, GridUnitType.Star);
            App.Current.MainWindow.Height *= 1.7f;
        }

        private void chbEnableLogs_Unchecked(object sender, RoutedEventArgs e)
        {
            baseGrid.RowDefinitions[1].Height = new GridLength(0.0f, GridUnitType.Star);
            App.Current.MainWindow.Height /= 1.7f;
        }
        #endregion

        private void RunTest(object[] args)
        {
            ((Button)args[0]).IsEnabled = false;

            ((Rectangle)args[1]).Fill = Brushes.LightGray;

            _worker = new BackgroundWorker() { WorkerReportsProgress = true };

            _worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {

                Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
                {
                    ((Rectangle)args[1]).Fill = Brushes.Red;
                }));
                Parallel.For(0, _concurrentTests, new ParallelOptions() { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, (i) =>
                    {
                        Interlocked.Increment(ref _threadParallelism);
                        AsynchronousClient.SimpleRouterServiceTest(_port, _IP, _dataToSend, _messageType, _Messages, _waitForServerResponse, i, (res) =>
                        {
                            if (Interlocked.Increment(ref _concurrentTestsCompleted) == _concurrentTests)
                                Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
                                {
                                    ((Rectangle)args[1]).Fill = Brushes.Green;
                                    //if (_waitForServerResponse)
                                    //    ((TextBox)args[4]).Text = res.ServerResponse;
                                }));
                        });
                        Interlocked.Decrement(ref _threadParallelism);
                        _worker.ReportProgress(100 * _concurrentTestsCompleted / _concurrentTests);

                    });
            };

            _worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs r)
            {
                ((Button)args[0]).IsEnabled = true;
            };

            _worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                ((ProgressBar)args[2]).Value = e.ProgressPercentage;
                ((ProgressBar)args[3]).Value = _threadParallelism * 100 / _maxDegreeOfParallelism;
            };

            _worker.RunWorkerAsync();
        }

        private void InitTest()
        {
            _port = tbPort.Text;
            _IP = tbIP.Text;
            _messageType = (MessageType)cbMessageType.SelectedValue;
            _Messages = Int32.Parse(tbMessages.Text);
            _concurrentTests = GetConcurrentTests();
            progressBar1.Maximum = 100;
            _dataToSend = tbDataToSend.Text;
            _waitForServerResponse = false;
            _maxDegreeOfParallelism = Int32.Parse(tbMaxDegreeOfParallelism.Text);
            EnableLogs = chbEnableLogs.IsChecked != null ? (bool)chbEnableLogs.IsChecked : false;
        }

        private int GetConcurrentTests()
        {
            int tests = 1;
            if (chbCuncurrentTests.IsChecked != null && (bool)chbCuncurrentTests.IsChecked)
                tests = Int32.Parse(tbConcurrentTests.Text);

            _concurrentTests = tests;
            _concurrentTestsCompleted = 0;
            return tests;
        }

    }
}
