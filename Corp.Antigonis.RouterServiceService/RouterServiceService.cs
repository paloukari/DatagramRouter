using System;


namespace Corp.Antigonis.RouterServiceService
{
  public class RouterServiceService : System.ServiceProcess.ServiceBase
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    // log4net logger
    private static readonly LoggingLibrary.Log4Net.ILog log = null;

    static string _serviceName = RouterServiceServiceInstaller._ServiceName;


    static RouterServiceService()
    {
            log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public RouterServiceService()
    {
      // This call is required by the Windows.Forms Component Designer.
      InitializeComponent();
    }


    /// <summary>
    /// The main entry point
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {     
      SetCurrentDirectoryToAssemblyDirectory();
      try
      {
        if (args.Length > 0)
        {
          ProcessCommandLineArguments(args);
        }
        else
        {
          log.InfoFormat("Running Service {0}", _serviceName);

          System.ServiceProcess.ServiceBase[] ServicesToRun;
          ServicesToRun = new System.ServiceProcess.ServiceBase[] { new RouterServiceService() };
          System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }
      }
      catch (Exception ex)
      {
        log.Error("UNHANDLED EXCEPTION AT MAIN", ex);
        throw;
      }
    }


    /// <summary>
    /// Process the command line arguments to install or uninstall the service
    /// </summary>
    /// <param name="args"></param>
    private static void ProcessCommandLineArguments(string[] args)
    {
      bool result;
      string message;

      if (args[0] == "-i")
      {
        result = RouterServiceInstaller.InstallService();
        if (result == true)
        {
          message = "SUCCESS - [{0}] service installed";
        }
        else
        {
          message = "FAILURE - [{0}] service NOT installed";
        }
        message = string.Format(message, _serviceName);
        Console.WriteLine(message);
        log.Info(message);
      }
      else if (args[0] == "-u")
      {
        result = RouterServiceInstaller.UninstallService();
        if (result == true)
        {
          message = "SUCCESS - [{0}] service uninstalled";
        }
        else
        {
          message = "FAILURE - [{0}] service NOT uninstalled";
        }
        Console.WriteLine(message);
        log.Info(message);
      }
      else if (args[0] == "-pci")
      {
        ConsoleMain.InstallPerformanceCounters();
      }
      else if (args[0] == "-pcu")
      {
        ConsoleMain.UninstallPerformanceCounters();
      }
      else if (args[0] == "-c")
      {
        ConsoleMain.RunService();
      }
    }


    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor
    /// </summary>
    private void InitializeComponent()
    {
      // 
      // RouterServiceService
      // 
      this.ServiceName = "AntigonisRouterServiceService";
    }


    /// <summary>
    /// Clean up any resources being used
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }


    /// <summary>
    /// Start the service thread
    /// </summary>
    protected override void OnStart(string[] args)
    {
      try
      {
        log.Info("SERVICE STARTING...");
        Corp.ServerHost.ServerHost.StartServer();
        log.Info("SERVICE STARTED");
      }
      catch (Exception ex)
      {
        log.Error("SERVICE START EXCEPTION", ex);
        throw;
      }
    }


    /// <summary>
    /// Stop the service thread
    /// </summary>
    protected override void OnStop()
    {
      try
      {
        log.Info("SERVICE STOPPING...");
        Corp.ServerHost.ServerHost.StopServer();
        log.Info("SERVICE STOPPED");
      }
      catch (Exception ex)
      {
        log.Error("SERVICE START EXCEPTION", ex);
        throw;
      }
    }


    /// <summary>
    /// Sets the current directory to the directory where this executable
    /// is located. Setting the current directory to the executable
    /// directory allows us to place configuration files into the directory
    /// where the service .exe is installed (instead of C:\WINDOWS\system32)
    /// </summary>
    private static void SetCurrentDirectoryToAssemblyDirectory()
    {
      System.IO.Directory.SetCurrentDirectory(
          System.IO.Path.GetDirectoryName(
          System.Reflection.Assembly.GetExecutingAssembly().Location));
    }


  }
}
