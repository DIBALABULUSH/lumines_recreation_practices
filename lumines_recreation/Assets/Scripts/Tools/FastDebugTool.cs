using System;
using System.Text;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// This class provides utility functions for debugging in Unity.
    /// </summary>
    public static class D
    {
        /// <summary>
        /// A flag indicating whether debugging is enabled.
        /// </summary>
        public static bool isDebugOpen = true;

        /// <summary>
        /// The separator string to use when combining multiple objects into a single string.
        /// </summary>
        public static string separator = ",";

        /// <summary>
        /// Logs a message to the Unity console with the specified log type and color.
        /// </summary>
        /// <param name="format">A string format to use when combining the arguments into a single string.</param>
        /// <param name="logType">The type of log message to output.</param>
        /// <param name="color">The color of the log message in the console.</param>
        /// <param name="args">The arguments to combine into a single string using the provided format.</param>
        public static void Log(string format = null, LogType logType = LogType.Information, UnityEngine.Color color = default,
            params object[] args)
        {
            if (color == default) color = Color.white;

            switch (logType)
            {
                case LogType.Information:
                {
                    if (!isDebugOpen) return;
                    
                    Info(Combine(format, args), color);
                    break;
                }
                case LogType.Warning:
                {
                    if (!isDebugOpen) return;
                    
                    Debug.LogWarning($"<color=#fff52f> {Combine(format, args)} </color>");
                    break;
                }
                case LogType.Error:
                {
                    Debug.LogError($"<color=#ff0015> {Combine(format, args)} </color>");
                    break;
                }
                case LogType.Exception:
                {
                    Debug.LogException(new Exception(Combine(format, args)));
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
                }
            }
        }

        /// <summary>
        /// Logs a message to the Unity console with the specified color.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color of the log message in the console.</param>
        public static void Info(string message, Color color)
        {
            if (!isDebugOpen) return;
            Debug.Log($"<color=#{(byte) (color.r * 255f):X2}{(byte) (color.g * 255f):X2}{(byte) (color.b * 255f):X2}>{message}</color>");
        }

        /// <summary>
        /// Logs a message to the Unity console with the specified log type.
        /// </summary>
        /// <param name="logType">The type of log message to output.</param>
        /// <param name="args">The arguments to combine into a single string using the default separator string.</param>
        private static void Write(LogType logType, params object[] args) =>
            D.Log(null, logType, default, args);

        /// <summary>
        /// Logs a message to the Unity console with the default log type (LogType.Information) and the default separator string.
        /// </summary>
        /// <param name="args">The arguments to combine into a single string using the default separator string.</param>
        private static void Log(params object[] args) => D.Log(LogType.Information, args);
        
        /// <summary>
        /// Logs a warning message to the Unity console with the default separator string.
        /// </summary>
        /// <param name="args">The arguments to combine into a single string using the default separator string.</param>
        public static void Warning(params object[] args) => D.Write(LogType.Warning, args);
        
        /// <summary>
        /// Logs a "To Do" message to the Unity console in cyan color with the default separator string.
        /// </summary>
        /// <param name="args">The arguments to combine into a single string using the default separator string.</param>
        public static void ToDo(params object[] args) => Info(Combine(null, args), Color.cyan);
        
        /// <summary>
        /// Logs an error message to the Unity console with the default separator string.
        /// </summary>
        /// <param name="args">The arguments to combine into a single string using the default separator string.</param>
        public static void Err(params object[] args) => D.Write(LogType.Error, args);
        
        /// <summary>
        /// Logs an exception to the Unity console.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        public static void Ex(Exception e) => D.Log(LogType.Exception, e);
        
        /// <summary>
        /// Combines the provided arguments into a single string using the specified format or the default separator string.
        /// </summary>
        /// <param name="format">A string format to use when combining the arguments into a single string.</param>
        /// <param name="args">The arguments to combine into a single string.</param>
        /// <returns>A single string containing the combined arguments.</returns>
        private static string Combine(string format = null, params object[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(format))
            {
                stringBuilder.AppendFormat(format, args);
            }
            else
            {
                int count = 0;
                foreach (var arg in args)
                {
                    stringBuilder.Append(arg);
                    count++;
                    if (count != args.Length)
                    {
                        stringBuilder.Append(separator);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}

