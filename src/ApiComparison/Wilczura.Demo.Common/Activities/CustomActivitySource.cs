using System.Diagnostics;

namespace Wilczura.Demo.Common.Activities;

public static class CustomActivitySource
{
    public const string Name = "DemoActivitySource";

    public static readonly Lazy<ActivitySource> Source = new Lazy<ActivitySource>(() =>
                new ActivitySource(CustomActivitySource.Name));
}