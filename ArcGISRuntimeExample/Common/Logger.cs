
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace ArcGISRuntimeExample.Common
{
    public static class Logger
    {

        public static LogLevel Level
        {
            get;
            set;
        } = LogLevel.Debug;

        public enum LogLevel
        {
            Error = 1,
            Warn = 2,
            Info = 3,
            Debug = 4
        }

        /// <summary>
        /// Retrieves the exception message and stack trace as a string. 
        /// Includes inner exceptions.
        /// </summary>
        /// <param name="ex">Exception to retrieve details of.</param>
        /// <returns>String with message and stacktrace info.</returns>
        private static string GetExceptionDetails(Exception ex)
        {
            string output = string.Empty;
            if (ex != null)
            {
                output += "Exception: " + ex.Message;
                output += Environment.NewLine + "Stack Trace: " + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    output +=
                        Environment.NewLine +
                        GetExceptionDetails(ex.InnerException);
                }
            }
            return output;
        }

        /// <summary>
        /// Appends an error to the log by adding the given log details.
        /// </summary>
        /// <param name="exception">Exception details. Null allowed.</param>
        public static void Log(
            Exception exception,
            bool trackException = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Error, exception, string.Empty, trackException, memberName, sourceFilePath, sourceLineNumber);
        }


        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Debug(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Debug, null, message, false, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Info(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Info, null, message, false, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Warn(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Warn, null, message, false, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Error(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Error, null, message, false, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Debug(
            Exception ex,
            string message = "",
            bool trackException = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Debug, ex, message, trackException, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="exception">Exception details. Null allowed.</param>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Info(
            Exception ex,
            string message = "",
            bool trackException = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Info, ex, message, trackException, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="exception">Exception details. Null allowed.</param>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Warn(
            Exception ex,
            string message = "",
            bool trackException = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Warn, ex, message, trackException, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="exception">Exception details. Null allowed.</param>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Error(
            Exception ex,
            string message = "",
            bool trackException = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Error, ex, message, trackException, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Log(
            LogLevel errorLevel,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(errorLevel, null, message, false, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Appends to the log by adding the given log details.
        /// </summary>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="exception">Exception details. Null allowed.</param>
        /// <param name="message">An explicit message. Null allowed.</param>
        public static void Log(
            LogLevel errorLevel,
            Exception exception,
            string message = "",
            bool trackException = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if ((int)errorLevel >= (int)Logger.Level && (exception != null || !string.IsNullOrEmpty(message)))
                {
                    string logMessage = string.Empty;

                    if (!string.IsNullOrEmpty(message))
                    {
                        logMessage += "\t" + message;
                    }

                    if (exception != null)
                    {
                        logMessage +=
                            Environment.NewLine +
                            GetExceptionDetails(exception);
                    }
                    logMessage += $"\t:: {sourceFilePath} :: {memberName} :: {sourceLineNumber}";

                    //Log(logMessage);
                    switch (errorLevel)
                    {
                        case LogLevel.Debug:
                            System.Diagnostics.Debug.WriteLine($"DEBUG {logMessage}");
                            break;
                        case LogLevel.Error:
                            System.Diagnostics.Debug.WriteLine($"ERROR {logMessage}");
                            break;
                        case LogLevel.Info:
                            System.Diagnostics.Debug.WriteLine($"INFO {logMessage}");
                            break;
                        case LogLevel.Warn:
                            System.Diagnostics.Debug.WriteLine($"WARN {logMessage}");
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR Failed to log Error: {ex.Message} Message: {message}");
                return;
            }
        }

        public static void Log(
            LogLevel logLevel,
            string className,
            string methodName,
            string description)
        {
            try
            {
                string logMessage
                    = className + "::" + methodName + "\t::\t" + description;
                Log(logLevel, null, logMessage, false, string.Empty, string.Empty, 0);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void Log(
            LogLevel logLevel,
            Exception ex,
            string className,
            string methodName,
            string description)
        {
            try
            {
                string logMessage
                    = className + "::" + methodName + "\t::\t" + description;
                Log(logLevel, ex, logMessage, false, string.Empty, string.Empty, 0);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
