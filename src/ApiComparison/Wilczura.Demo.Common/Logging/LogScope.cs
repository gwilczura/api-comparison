using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilczura.Demo.Common.Activities;

namespace Wilczura.Demo.Common.Logging;

public class LogScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly LogInfo _logInfo;
    private readonly Stopwatch _stopwatch = new();
    private readonly LogLevel _level;
    private readonly EventId? _eventId;
    private readonly Activity? _activity;
    private readonly string _action;

    public LogInfo LogInfo { get { return _logInfo; } }

    public LogScope(
        ILogger logger,
        LogInfo logInfo,
        LogLevel? logLevel = null,
        EventId? eventId = null,
        string? activityName = null)
    {
        _action = logInfo.EventAction ?? "unknown";
        logInfo.EventAction = $"{_action}-begin";
        _stopwatch.Start();
        if (activityName != null)
        {
            _activity = CustomActivitySource.Source.Value.StartActivity(activityName, ActivityKind.Internal);
        }
        _logger = logger;
        _logInfo = logInfo;
        _level = (logLevel ?? LogLevel.Information);
        _eventId = eventId;
        _logger.Log(_level, _logInfo, _eventId);
    }


    public void Dispose()
    {
        _stopwatch.Stop();
        _logInfo.EventDuration = _stopwatch.ElapsedMilliseconds;
        if (_activity != null)
        {
            _activity.SetEndTime(DateTime.UtcNow);
        }

        _logInfo.EventAction = $"{_action}-end";
        _logger.Log(_level, _logInfo, _eventId);
        if (_activity != null)
        {
            _activity.Dispose();
        }
    }
}
