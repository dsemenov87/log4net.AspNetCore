using System;
using System.Collections.Concurrent;
using log4net;
using log4net.Repository;
using Microsoft.Extensions.Logging;

namespace log4net.AspNetCore
{
  public class Log4NetProvider : ILoggerProvider
  {
    private readonly Log4NetConfig _cfg;
    
    private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers =
        new ConcurrentDictionary<string, Log4NetLogger>();

    public Log4NetProvider(Log4NetConfig cfg)
    {
      if (cfg == null)
        throw new ArgumentNullException(nameof(cfg));

      _cfg = cfg;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
    private Log4NetLogger CreateLoggerImplementation(string name)
    {
        return new Log4NetLogger(name, _cfg);
    }
  }
}