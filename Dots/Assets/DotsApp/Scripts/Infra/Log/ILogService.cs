using System;

namespace AgletApp.Infra.Log
{
    public interface ILogService
    {
        void Initialize();
        void SetMinLogLevel(LogLevel level);
        ILogger GetLogger(string loggerName);
        ILogger GetLogger(Type loggerType);
    }
}