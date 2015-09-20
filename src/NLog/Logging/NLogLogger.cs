﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;
using System.Threading.Tasks;
using Nybus.Utils;

namespace Nybus.Logging
{
    public class NLogLogger : LoggerBase
    {
        public static string LoggerName = "Nybus";

        private readonly NLog.ILogger _logger;

        public NLogLogger(NLog.ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
        }

        protected override Task LogEvent(LogLevel level, string message, object data, string callerMemberName)
        {
            var dictionary = ObjectToDictionary.Convert(data);

            var logLevel = ConvertLevel(level);
            var logEventInfo = CreateLogEventInfo(logLevel, message, dictionary);

            _logger.Log(logEventInfo);

            return Task.CompletedTask;

        }

        private LogEventInfo CreateLogEventInfo(NLog.LogLevel level, string message, IReadOnlyDictionary<string, object> dictionary)
        {
            string assemblyProp = string.Empty;
            string classProp = string.Empty;
            string methodProp = string.Empty;
            string messageProp = string.Empty;
            string innerMessageProp = string.Empty;

            var exception = ExtractException(dictionary);

            LogEventInfo logEvent = new LogEventInfo(level, LoggerName, message);

            foreach (var item in dictionary)
            {
                if (item.Key != "exception")
                    logEvent.Properties[item.Key] = item.Value;
            }

            if (exception != null)
            {
                assemblyProp = exception.Source;
                classProp = exception.TargetSite.DeclaringType.FullName;
                methodProp = exception.TargetSite.Name;
                messageProp = exception.Message;

                if (exception.InnerException != null)
                {
                    innerMessageProp = exception.InnerException.Message;
                }
            }

            logEvent.Properties["error-source"] = assemblyProp;
            logEvent.Properties["error-class"] = classProp;
            logEvent.Properties["error-method"] = methodProp;
            logEvent.Properties["error-message"] = messageProp;
            logEvent.Properties["inner-error-message"] = innerMessageProp;

            return logEvent;
        }

        private Exception ExtractException(IReadOnlyDictionary<string, object> dictionary)
        {
            object item = null;

            if (dictionary.TryGetValue("exception", out item))
            {
                return item as Exception;
            }

            return null;
        }

        private NLog.LogLevel ConvertLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                default:
                    return NLog.LogLevel.Info;
            }
        }
    }
}
