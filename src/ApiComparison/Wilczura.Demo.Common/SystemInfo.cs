using System.Reflection;

namespace Wilczura.Demo.Common;

public static class SystemInfo
{
    private static string _info = string.Empty;
    private static readonly object _syncLocker = new();
    public static string GetInfo(string additionalInfo)
    {
        lock (_syncLocker)
        {
            if (string.IsNullOrEmpty(_info))
            {

                var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName();
                var version = entryAssemblyName?.Version?.ToString();
                _info = $"{entryAssemblyName?.Name} | {version} | {additionalInfo}";
            }
        }

        return _info;
    }

}