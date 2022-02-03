using System;
using log4net;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AgletApp.Infra.Log
{
    public class Log4NetUnityLogHandler : ILogHandler
    {
        private static readonly ILog Log = LogManager.GetLogger("unity_debugger");

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            var message = string.Format(format, args);
            switch (logType)
            {
                case LogType.Exception:
                    Log.Fatal(message);
                    break;
                case LogType.Error:
                    Log.Error(message);
                    break;
                case LogType.Warning:
                case LogType.Assert:
                    Log.Warn(message);
                    break;
                case LogType.Log:
                    Log.Info(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }

        public void LogException(Exception exception, Object context)
        {
            Log.Fatal(exception);
        }
    }
}