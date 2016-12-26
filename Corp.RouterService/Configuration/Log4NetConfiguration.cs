


namespace Corp.RouterService.Configuration
{
  public class Log4NetConfiguration
  {    
    public static void Configure()
    {
      LoggingLibrary.LoggerManager.ConfigureLog4Net();
    }
  }
}
