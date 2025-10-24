using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Wilczura.Demo.Common.Logging;

namespace Wilczura.Demo.Common.Activities;

public class CustomActivityListener : IDisposable
{
    private readonly string _activityName;
    private readonly ILogger? _logger;
    private readonly ActivityListener _listener;

    // ActivityListener - required by ActivitySource.Create()
    public CustomActivityListener(string activitySourceName, ILogger? logger)
    {
        _activityName = activitySourceName;
        _logger = logger;
        _listener = new ActivityListener
        {
            ShouldListenTo = ShouldListen,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = OnActivityStarted,
            ActivityStopped = OnActivityStopped
        };
        ActivitySource.AddActivityListener(_listener);

    }


    private bool ShouldListen(ActivitySource source)
    {
        _logger?.LogInformation(LogInfo.From($"ActivitySource detected").WithAdditionalInfo($"{source.Name} / {source.Version}"));
        return source.Name == _activityName;
    }

    private void OnActivityStarted(Activity activity)
    {
        // ignore
    }

    private void OnActivityStopped(Activity activity)
    {
        // ignore
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
