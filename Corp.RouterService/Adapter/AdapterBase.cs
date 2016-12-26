

namespace Corp.RouterService.Adapter
{
  public abstract class AdapterBase
  {

    AdapterSettings _settings;

    internal AdapterLiveStatistics _liveStatistics;


    protected static LoggingLibrary.Log4Net.ILog log = null;

    AdapterState _state = AdapterState.Uninitialized;
    object _stateSync = new object();

    public LoggingLibrary.Log4Net.ILog Log { get { return log; } }

    public void LogDebug(object data)
    {
      if (log != null && log.IsDebugEnabled)
      {
        log.Debug(this.ToString() + " : " + data != null ? data.ToString() : "NULL");
      }
    }

    public void LogInfo(object data)
    {
      if (log != null && log.IsInfoEnabled)
      {
        log.Info(this.ToString() + " : " + data != null ? data.ToString() : "NULL");
      }
    }

    public void LogWarn(object data)
    {
      if (log != null && log.IsWarnEnabled)
      {
        log.Warn(this.ToString() + " : " + data != null ? data.ToString() : "NULL");
      }
    }
    public void LogError(object data)
    {
      if (log != null && log.IsErrorEnabled)
      {
        log.Error(this.ToString() + " : " + data != null ? data.ToString() : "NULL");
      }
    }

    public string AdapterName { get { return this.ToString(); } }

    public AdapterState State
    {
      get
      {
        lock (_stateSync)
          return _state;
      }
      set
      {
        if (log != null && log.IsInfoEnabled)
          log.Info("Change of Adapter " + AdapterName + " (" + _settings.ToString() + ") state from " + _state + " to " + value);

        lock (_stateSync)
          _state = value;
      }
    }



    private AdapterBase()
    {

    }

    protected AdapterBase(AdapterSettings settings)
    {
      _settings = settings;

      if (_settings.UsePerformanceCounters)
        _liveStatistics = new AdapterLiveStatistics(_settings);
    }

    protected void IncrementReceivedMessages()
    {
      if (_liveStatistics != null)
        _liveStatistics.IncrementReceivedMessages();
    }


    protected void IncrementSentMessages()
    {
      if (_liveStatistics != null)
        _liveStatistics.IncrementSentMessages();
    }

  }
}
