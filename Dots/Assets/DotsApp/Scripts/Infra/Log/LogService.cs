using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using UnityEngine;

namespace AgletApp.Infra.Log
{
    public class LogService : ILogService
    {
        private readonly Dictionary<LogLevel, Level> _logLevelsCache = new Dictionary<LogLevel, Level>
        {
            { LogLevel.Debug, Level.Debug },
            { LogLevel.Info, Level.Info },
            { LogLevel.Warn, Level.Warn },
            { LogLevel.Error, Level.Error },
            { LogLevel.Fatal, Level.Fatal },
            { LogLevel.Off, Level.Off }
        };

        private readonly Hierarchy _hierarchy;

        public LogService()
        {
            _hierarchy = (Hierarchy) LogManager.GetRepository();
        }

        public static ILogger GetLogger(string loggerName)
        {
            return new Log4NetWrapper(LogManager.GetLogger(loggerName));
        }

        public static ILogger GetLogger(Type loggerType)
        {
            return new Log4NetWrapper(LogManager.GetLogger(loggerType));
        }

        ILogger ILogService.GetLogger(string loggerName) => GetLogger(loggerName);
        ILogger ILogService.GetLogger(Type loggerType) => GetLogger(loggerType);

        public void Initialize()
        {
            BasicConfigurator.Configure(
                SetUpFileLogAppender(),
                SetUpUnityLogAppender());

            //change the default unity handler to transmit any Unity.Debug calls through Log4net
            Debug.unityLogger.logHandler = new Log4NetUnityLogHandler();
        }

        public void SetMinLogLevel(LogLevel type)
        {
            if(!_logLevelsCache.TryGetValue(type, out var lo4NetLevel))
            {
                throw new InvalidOperationException($"Undefined log type. LogLevelId:{type}");
            }

            _hierarchy.Root.Level = lo4NetLevel;
            _hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }

        private static IAppender SetUpFileLogAppender()
        {
            var layout = new PatternLayout
            {
                ConversionPattern = "%date [%-5level] [%logger] - %message%newline"
            };
            layout.ActivateOptions();

            var appender = new RollingFileAppender
            {
                AppendToFile = false,
                File = Path.Combine(Application.persistentDataPath, "Logs", "GameLog.txt"),
                Layout = layout,
                MaxSizeRollBackups = -1,
                MaximumFileSize = "50MB",
                DatePattern = "dd-MM-yyyy",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true,
            };
            appender.ActivateOptions();

            return appender;
        }

        private static IAppender SetUpUnityLogAppender()
        {
            var layout = new PatternLayout
            {
                ConversionPattern = "[%logger] - %message%newline"
            };
            layout.ActivateOptions();

            var appender =  new UnityAppender(Debug.unityLogger.logHandler)
            {
                Layout = layout
            };
            appender.ActivateOptions();

            return appender;
        }
    }
}