using Wilczura.Demo.Common.Consts;
using Wilczura.Demo.Common.Logging;

namespace Wilczura.Demo.Host.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(
        RequestDelegate next,
        ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Exception? exception = null;
        var message = "Http in";
        var activityName = "http-in";
        var logInfo = new LogInfo(message, CommonConsts.EventCategoryWeb);

        logInfo.EventAction = activityName;
        var logScope = new LogScope(_logger, logInfo, LogLevel.Information, LogEvents.WebRequest, activityName: activityName);
        logInfo.HttpMethod = context.Request.Method;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            logInfo.EventReason = $"HTTP Code {context.Response.StatusCode}";

            if (exception != null)
            {
                logInfo.ApplyException(exception);
            }
            else if (context.Response.StatusCode >= 400)
            {
                logInfo.EventOutcome = CommonConsts.EventOutcomeFailure;
            }
            else if (context.Response.StatusCode >= 300)
            {
                logInfo.EventOutcome = CommonConsts.EventOutcomeUnknown;
            }
            else
            {
                logInfo.EventOutcome = CommonConsts.EventOutcomeSuccess;
            }

            //  logInfo.ApplyPrincipal(context.GetPrincipal());

            logScope.Dispose();
        }
    }
}
