using System;
using Microsoft.Extensions.Logging;
using ILogger = NWebDav.Server.Logging.ILogger;
using ILoggerFactory = NWebDav.Server.Logging.ILoggerFactory;
using LogLevel = NWebDav.Server.Logging.LogLevel;

namespace eventphone.guru3.carddav
{
    public class DAVLoggerFactory : ILoggerFactory
    {
        private readonly Microsoft.Extensions.Logging.ILoggerFactory _factory;

        public DAVLoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory factory)
        {
            _factory = factory;
        }

        private class DAVLogger : ILogger
        {
            private readonly Microsoft.Extensions.Logging.ILogger _logger;

            public DAVLogger(Microsoft.Extensions.Logging.ILogger logger)
            {
                _logger = logger;
            }

            public bool IsLogEnabled(LogLevel logLevel)
            {
                var level = GetLogLevel(logLevel);
                return _logger.IsEnabled(level);
            }

            public void Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                if (!IsLogEnabled(logLevel))
                    return;
                var level = GetLogLevel(logLevel);
                if (exception != null)
                    _logger.Log(level, exception, messageFunc());
                else
                    _logger.Log(level, messageFunc());
            }

            private static Microsoft.Extensions.Logging.LogLevel GetLogLevel(LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        return Microsoft.Extensions.Logging.LogLevel.Debug;
                    case LogLevel.Info:
                        return Microsoft.Extensions.Logging.LogLevel.Information;
                    case LogLevel.Warning:
                        return Microsoft.Extensions.Logging.LogLevel.Warning;
                    case LogLevel.Error:
                        return Microsoft.Extensions.Logging.LogLevel.Error;
                    case LogLevel.Fatal:
                        return Microsoft.Extensions.Logging.LogLevel.Critical;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
                }
            }
        }

        public ILogger CreateLogger(Type type)
        {
            var logger = _factory.CreateLogger(type);
            return new DAVLogger(logger);
        }
    }
}