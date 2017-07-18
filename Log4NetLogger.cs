using System;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using System.Reflection;

using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILog4netLogger = log4net.Core.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace log4net.AspNetCore
{ 
  public class Log4NetLogger : ILogger
  {
      private readonly string _name;
      private readonly ILog4netLogger _log;

      public Log4NetLogger(string name, Log4NetConfig config = null)
      {
          if (name == null)
            throw new ArgumentNullException(nameof(name));
          
          _name = name;

          var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetEntryAssembly());
          var patternLayout = new PatternLayout();
          patternLayout.ConversionPattern =
            (config == null || string.IsNullOrWhiteSpace(config.ConversionPattern))
              ? "%date [%thread] %-5level %logger - %message%newline"
              : config.ConversionPattern;

          patternLayout.ActivateOptions();
          hierarchy.Root.AddAppender(new ConsoleAppender());

          var level = Level.Info;
          if (config != null)
            switch (config.MinLevel)
            {
                case "FATAL":
                    level = Level.Fatal;
                    break;
                case "DEBUG":
                    level = Level.Debug;
                    break;
                case "ERROR":
                    level = Level.Error;
                    break;
                case "INFO":
                    level = Level.Info;
                    break;
                case "WARN":
                    level = Level.Warn;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"unknown level type: {config.ConversionPattern}.");
            }

          hierarchy.Root.Level = level;  
          hierarchy.Configured = true;
          _log = hierarchy.GetLogger(name);
      }

      public IDisposable BeginScope<TState>(TState state)
      {
          throw new NotImplementedException("BeginScope is not implemented yet.");
      }

      public bool IsEnabled(LogLevel logLevel)
      {   
          switch (logLevel)
          {
              case LogLevel.Critical:
                  return _log.IsEnabledFor(Level.Critical);
              case LogLevel.Debug:
              case LogLevel.Trace:
                  return _log.IsEnabledFor(Level.Debug);
              case LogLevel.Error:
                  return _log.IsEnabledFor(Level.Error);
              case LogLevel.Information:
                  return _log.IsEnabledFor(Level.Info);
              case LogLevel.Warning:
                  return _log.IsEnabledFor(Level.Warn);
              default:
                  throw new ArgumentOutOfRangeException(nameof(logLevel));
          }
      }

      private void InternalLog(string message, Level level)
      {
        _log.Log(null, level, message, null);

        var cfg = new Log4NetConfig
        {
          MinLevel = "DEBUG",
          ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
        };
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
          Exception exception, Func<TState, Exception, string> formatter)
      {
          if (!IsEnabled(logLevel))
          {
              return;
          }

          if (formatter == null)
          {
              throw new ArgumentNullException(nameof(formatter));
          }
          string message = null;
          if (null != formatter)
          {
              message = formatter(state, exception);
          }
          if (!string.IsNullOrEmpty(message) || exception != null)
          {
              switch (logLevel)
              {
                  case LogLevel.Critical:
                      InternalLog(message, Level.Critical);
                      break;
                  case LogLevel.Debug:
                  case LogLevel.Trace:
                      InternalLog(message, Level.Debug);
                      break;
                  case LogLevel.Error:
                      InternalLog(message, Level.Error);
                      break;
                  case LogLevel.Information:
                      InternalLog(message, Level.Info);
                      break;
                  case LogLevel.Warning:
                      InternalLog(message, Level.Warn);
                      break;
                  default:
                      throw new ArgumentOutOfRangeException(nameof(logLevel));
              }
          }
      }
  }
  public static class Log4netExtensions
  {
      public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, Log4NetConfig cfg)
      {
          factory.AddProvider(new Log4NetProvider(cfg));
          return factory;
      }
  }
}
