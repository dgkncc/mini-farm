using Minifarm._Core.Common;
using UnityEngine;

namespace Minifarm.Managers
{
    public class DebugManager : SingleMonoBehaviour<DebugManager>
    {
        [SerializeField] private bool debugEnabled = true;

        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        [SerializeField] private bool showInfoLogs = true;
        [SerializeField] private bool showWarningLogs = true;
        [SerializeField] private bool showErrorLogs = true;

        public void SetDebugging(bool enabled)
        {
            debugEnabled = enabled;
        }

        public void ConfigureLogLevels(bool showInfo, bool showWarnings, bool showErrors)
        {
            showInfoLogs = showInfo;
            showWarningLogs = showWarnings;
            showErrorLogs = showErrors;
        }

        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (!debugEnabled)
                return;

            switch (level)
            {
                case LogLevel.Info:
                    if (showInfoLogs)
                        Debug.Log(message);
                    break;
                case LogLevel.Warning:
                    if (showWarningLogs)
                        Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                    if (showErrorLogs)
                        Debug.LogError(message);
                    break;
            }
        }

        public void Log(string message, Object context, LogLevel level = LogLevel.Info)
        {
            if (!debugEnabled)
                return;

            switch (level)
            {
                case LogLevel.Info:
                    if (showInfoLogs)
                        Debug.Log(message, context);
                    break;
                case LogLevel.Warning:
                    if (showWarningLogs)
                        Debug.LogWarning(message, context);
                    break;
                case LogLevel.Error:
                    if (showErrorLogs)
                        Debug.LogError(message, context);
                    break;
            }
        }
    }
}
