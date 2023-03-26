using Discord;
using Pastel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

public static class LogLevelNormalization
{
    // Could automate this, maybe unnecessary
    readonly static int highestCount = 13;

    public static ConcurrentDictionary<LogLevel, string> logLevelNormalizationStrings = new ConcurrentDictionary<LogLevel, string>();

    public static void InitLogLevelNormalizationStrings()
    {
        foreach (LogLevel logLevel in Enum.GetValues(typeof(LogLevel)))
        {
            string finalNormalizationString = "";

            for (int i = 0; i < highestCount - logLevel.ToString().Count(); ++i)
            {
                finalNormalizationString += "-";
            }

            //Console.WriteLine( logLevel.ToString() + " | " + finalNormalizationString);

            logLevelNormalizationStrings.TryAdd(logLevel, finalNormalizationString);
        }
    }
}