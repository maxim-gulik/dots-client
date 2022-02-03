using log4net.Appender;
using log4net.Core;
using UnityEngine;

namespace AgletApp.Infra.Log
{
    public class UnityAppender : AppenderSkeleton
    {
        private readonly ILogHandler _logHandler;

        public UnityAppender(ILogHandler logHandler)
        {
            _logHandler = logHandler;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = RenderLoggingEvent(loggingEvent);

            if (Level.Compare(loggingEvent.Level, Level.Fatal) >= 0)
            {
                _logHandler.LogFormat(LogType.Exception, default, message);
            }
            else if (Level.Compare(loggingEvent.Level, Level.Error) >= 0)
            {
                _logHandler.LogFormat(LogType.Error, default, message);
            }
            else if (Level.Compare(loggingEvent.Level, Level.Warn) >= 0)
            {
                _logHandler.LogFormat(LogType.Warning, default, message);
            }
            else if(Level.Compare(loggingEvent.Level, Level.Info) >= 0 || Level.Compare(loggingEvent.Level, Level.Debug) >= 0)
            {
                _logHandler.LogFormat(LogType.Log, default, message);
            }
        }
    }
}