using System;
using Corp.RouterService.Configuration;


namespace Corp.Antigonis.RouterServiceService
{
  /// <summary>
  /// Class ConsoleMain, to run as a console application
  /// </summary>
  class ConsoleMain
  {
    // log4net logger
    private static readonly LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [MTAThread]
    static void Main(string[] args)
    {
      RunService();
    }


    /// <summary>
    /// The main code to start/stop the service
    /// </summary>
    internal static void RunService()
    {
      Log4NetConfiguration.Configure();

      Corp.ServerHost.ServerHost.InitializeServer();

      bool exitLoop = false;
      bool isStarted = false;
      bool startImmediately = false;
      while (exitLoop == false)
      {
        Console.WriteLine("\nEXIT/START/STOP");
        Console.Write("$ ");

        string userCommand;
        if (startImmediately)
        {
            userCommand = "START";
            startImmediately = false;
            goto skipCommand;
            
        }
        userCommand = Console.ReadLine().ToUpper();
      skipCommand:
        if (userCommand == "START")
        {
          if (isStarted == false)
          {
            Console.WriteLine("Service is starting...");

            if (log.IsInfoEnabled)
            {
              log.Info("Calling StartServer()...");
            }
            Corp.ServerHost.ServerHost.StartServer();
            isStarted = true;
            if (log.IsInfoEnabled)
            {
              log.Info("Called StartServer().");
            }

            Console.WriteLine("Service is started");
          }
        }
        else if (userCommand == "STOP")
        {
          if (isStarted == true)
          {
            Console.WriteLine("Service is stopping...");

            if (log.IsInfoEnabled)
            {
              log.Info("Calling StopServer()...");
            }
            Corp.ServerHost.ServerHost.StopServer();
            isStarted = false;
            if (log.IsInfoEnabled)
            {
              log.Info("Called StopServer().");
            }

            Console.WriteLine("Service is stopped");
          }
        }
        else if (userCommand == "EXIT")
        {
          Console.WriteLine("Exiting...");

          if (isStarted == true)
          {
            if (log.IsInfoEnabled)
            {
              log.Info("Calling StopServer()...");
            }
            Corp.ServerHost.ServerHost.StopServer();
            isStarted = false;
            if (log.IsInfoEnabled)
            {
              log.Info("Called StopServer().");
            }
          }

          exitLoop = true;
        }
      }
    }

    internal static void InstallPerformanceCounters()
    {
      Log4NetConfiguration.Configure();

      Corp.ServerHost.ServerHost.InstallPerformanceCounters();
    }

    internal static void UninstallPerformanceCounters()
    {
      Log4NetConfiguration.Configure();

      Corp.ServerHost.ServerHost.UninstallPerformanceCounters();
    }
  }
}
