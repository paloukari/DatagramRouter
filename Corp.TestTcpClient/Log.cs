using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Corp.TestTcpClient
{
    static class Log
    {
        private static RichTextBox _logsBox;
        private static MainWindow _window;
        internal static void Initialize(MainWindow mainWindow)
        {
            _window = mainWindow;
            _logsBox = mainWindow.rtbLogs;
        }

        internal static void WriteLine(string logLine, LogType type = LogType.Info)
        {
            if (_logsBox != null && _window.EnableLogs)
            {
                _logsBox.Dispatcher.BeginInvoke(DispatcherPriority.Render, 
                    new Action(delegate()
                    {
                        try
                        {                     
                            TextRange logText = new TextRange(_logsBox.Document.ContentEnd, _logsBox.Document.ContentEnd);
                            logText.Text = logLine + System.Environment.NewLine;
                            switch (type)
                            {

                                case LogType.Info:
                                    break;
                                case LogType.Warning:
                                    logText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkOrange);
                                    break;
                                case LogType.Error:
                                    logText.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                                    logText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                                    break;
                                default:
                                    break;
                            }

                            _logsBox.ScrollToEnd();
                        }
                        catch (Exception)
                        {
                        }

                    }));
            }
        }
    }
}
