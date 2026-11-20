using System.Windows;
using Leagues.ViewModels;

namespace Leagues.Logging;

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
}

public static class Logger
{
    private static LogLevel logLevel = LogLevel.Info;
    public static void Info(string message)
    {
        if (logLevel <= LogLevel.Info)
            Log(message, "INFO");
    }

    public static void Warn(string message)
    {
        if (logLevel <= LogLevel.Warn)
            Log(message, "WARN");
    }

    public static void Error(string message)
    {
        if (logLevel <= LogLevel.Error)
            Log(message, "ERROR");
    }

    public static void Debug(string message)
    {
        if (logLevel <= LogLevel.Debug)
            Log(message, "DEBUG");
    }

    private static void Log(string message, string level)
    {
        // For thread affinity, we need to use the Dispatcher to update the ObservableCollection from a non-UI thread.:
        // https://stackoverflow.com/questions/18331723
        Application.Current.Dispatcher.Invoke((Action)delegate
        {
            HomeViewModel.LogEntries.Add($"{DateTime.Now:HH:m:ss} {level}: {message}");
        });
    }
}