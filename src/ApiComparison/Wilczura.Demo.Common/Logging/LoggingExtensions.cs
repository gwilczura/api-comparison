using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Wilczura.Demo.Common.Consts;

namespace Wilczura.Demo.Common.Logging;

public static class LoggingExtensions
{

    public static void LogInformation(this ILogger logger, string message, EventId? eventId = null)
    {
        logger.LogInformation(new LogInfo(message, CommonConsts.EventCategoryProcess), eventId);
    }

    public static void LogInformation(this ILogger logger, LogInfo logInfo, EventId? eventId = null)
    {
        logger.Log(LogLevel.Information, logInfo, eventId);
    }

    public static void LogWarning(this ILogger logger, LogInfo logInfo, EventId? eventId = null)
    {
        logger.Log(LogLevel.Warning, logInfo, eventId);
    }

    public static void LogError(this ILogger logger, LogInfo logInfo, EventId? eventId = null)
    {
        logger.Log(LogLevel.Error, logInfo, eventId);
    }

    public static void Log(this ILogger logger, LogLevel logLevel, LogInfo logInfo, EventId? eventId = null)
    {
        ApplyActivity(logInfo);
        logger.Log(logLevel, eventId ?? LogEvents.Custom, logInfo, null, DictionaryMessageFormatter<LogInfo>);
    }

    private static void ApplyActivity(LogInfo logInfo)
    {
        if (string.IsNullOrEmpty(logInfo.TraceId))
        {
            if (Activity.Current is Activity activity)
            {
                if (activity.IdFormat == ActivityIdFormat.W3C)
                {
                    logInfo.TraceId = activity.TraceId.ToString();
                    logInfo.SpanId = activity.SpanId.ToString();
                    logInfo.Traceparent = activity.Id ?? string.Empty;

                }
                else
                {
                    logInfo.TraceId = activity.RootId ?? string.Empty;
                    logInfo.SpanId = activity.Id ?? string.Empty;
                }
            }
            else
            {
                logInfo.TraceId ??= Trace.CorrelationManager.ActivityId.ToString();
            }
        }
    }

    private static string DictionaryMessageFormatter<T>(T state, Exception? exception)
    {
        if (state is IDictionary<string, object> dictionary)
        {
            if (dictionary.TryGetValue(LogInfo.MessageKey, out var message))
            {
                return (string)message;
            }

            return $"missing [{LogInfo.MessageKey}] key";
        }

        return "unsupported type, provide dictionary<string,object>";
    }
}
