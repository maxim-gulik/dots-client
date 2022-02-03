using System;
using log4net;

namespace AgletApp.Infra.Log
{
    internal class Log4NetWrapper : ILogger
    {
        private readonly ILog _log;

        public Log4NetWrapper(ILog log)
        {
            _log = log;
        }

        public bool IsDebugEnabled => _log.IsDebugEnabled;

        public void Debug(string message) => _log.Debug(message);
        public void Debug(string message, Exception exception) => _log.Debug(message, exception);
        public void Info(string message) => _log.Info(message);
        public void Info(string message, Exception exception) => _log.Info(message, exception);
        public void Warn(string message) => _log.Warn(message);
        public void Warn(string message, Exception exception) => _log.Warn(message, exception);
        public void Error(string message) => _log.Error(message);
        public void Error(string message, Exception exception) => _log.Error(message, exception);
        public void Fatal(string message) => _log.Fatal(message);
        public void Fatal(string message, Exception exception) => _log.Fatal(message, exception);
    }
}