public static class LogLevelNormalization
{
    // Could automate this, maybe unnecessary
    readonly static int highestCount = 13;

    public static Dictionary<LogLevel, string> logLevelNormalizationStrings = new Dictionary<LogLevel, string>();

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