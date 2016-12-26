using System;
using System.Configuration;
using Corp.RouterService.Configuration;


namespace Corp.Antigonis.RouterServiceService
{
  /// <summary>
  /// Summary description for Config Parameters.
  /// </summary>
  public class Config
  {


    // log4net logger
    private static readonly LoggingLibrary.Log4Net.ILog Log = null;


    static Config()
    {
      Log4NetConfiguration.Configure();

      Log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
    public static object GetConfigValue(string name, object defaultValue)
    {
      string result = ConfigurationManager.AppSettings.Get(name);
      return (result ?? defaultValue);
    }


    public static string InstallServiceName
    {
      get
      {
        return Convert.ToString(GetConfigValue(
            "InstallServiceName", "AntigonisRouterService"));
      }
    }


    public static string InstallDisplayName
    {
      get
      {
        return Convert.ToString(GetConfigValue(
            "InstallDisplayName", "Corp Antigonis Tcp Server"));
      }
    }


    public static string InstallDescription
    {
      get
      {
        return Convert.ToString(GetConfigValue(
            "InstallDescription", "Corp Antigonis Tcp Server Service"));
      }
    }


  }
}
