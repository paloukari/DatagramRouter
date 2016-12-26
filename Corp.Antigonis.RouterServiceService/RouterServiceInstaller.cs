using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace Corp.Antigonis.RouterServiceService
{

  
  /// <summary>
  /// Customized ServiceInstaller Class
  /// </summary>
  [RunInstaller(true)]
  public sealed class RouterServiceServiceInstaller : ServiceInstaller
  {
    //***********************************************************************/
    // Do not forget to set _Description, _DisplayName, _ServiceName values */
    // These three values can also be read from app.config file,            */
    // or maybe set by command line arguments to a static variable          */
    //***********************************************************************/

    internal static string _Description
    { 
      get 
      {
        return Config.InstallDescription;
        //return "Corp Antigonis Tcp Server Service";
      } 
    }

    internal static string _DisplayName
    {
      get
      {
        return Config.InstallDisplayName;
        //return "Corp Antigonis Tcp Server";
      }
    }

    internal static string _ServiceName
    {
      get
      {
        return Config.InstallServiceName;
        //return "AntigonisRouterService";
      }
    }

    public RouterServiceServiceInstaller()
    {
      this.Description = _Description;
      this.DisplayName = _DisplayName;
      this.ServiceName = _ServiceName;
      this.StartType = ServiceStartMode.Automatic;
    }
  }


  /// <summary>
  /// Customized ServiceProcessInstaller Class
  /// </summary>
  [RunInstaller(true)]
  public sealed class RouterServiceServiceProcessInstaller : ServiceProcessInstaller
  {
    public RouterServiceServiceProcessInstaller()
    {
      this.Account = ServiceAccount.LocalSystem;
    }
  }


  /// <summary>
  /// Managed Installer Class
  /// </summary>
  internal sealed class RouterServiceInstaller
  {
    internal static bool InstallService()
    {
      bool result;
      string[] args = new string[] 
        {
          "/i",
          "/LogFile=Corp.Antigonis.RouterServiceService.InstallLog.txt",
          "/LogToConsole=false"
        };

      result = Execute(false, args);
      return result;
    }


    internal static bool UninstallService()
    {
      bool result;
      string[] args = new string[] 
        {
          "/u",
          "/LogFile=Corp.Antigonis.RouterServiceService.InstallLog.txt",
          "/LogToConsole=false"
        };

      result = Execute(true, args);
      return result;
    }


    private static bool Execute(bool uninstallFlag, string[] args)
    {
      bool result = false;

      try
      {
        Console.WriteLine(uninstallFlag ? "Uninstalling..." : "Installing...");

        using (AssemblyInstaller objAssemblyInstaller =
            new AssemblyInstaller(typeof(RouterServiceServiceProcessInstaller).Assembly, args))
        {
          IDictionary state = new Hashtable();
          objAssemblyInstaller.UseNewContext = true;

          try
          {
            if (uninstallFlag)
            {
              objAssemblyInstaller.Uninstall(state);
              result = true;
            }
            else
            {
              objAssemblyInstaller.Install(state);
              objAssemblyInstaller.Commit(state);
              result = true;
            }
          }
          catch
          {
            try
            {
              objAssemblyInstaller.Rollback(state);
            }
            catch
            {
              // Eat Rollback exception
            }
            
            // Rethrow the original exception
            throw;
          }
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex.Message);
      }

      return result;
    }
  }

}
