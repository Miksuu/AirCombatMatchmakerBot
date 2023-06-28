using Discord;
using Pastel;
using System.Globalization;
using System.Runtime.CompilerServices;

public static class Log
{
    public static void WriteLine(
        string _message,
        LogLevel _logLevel = LogLevel.VERBOSE,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;

        string date = DateTime.Now.Date.ToString("dd.MM.yyyy", culture);
        string time = DateTime.Now.ToString("hh:mm:ss.fff", culture);

        string callerMethod = ": " + _memberName + "()";
        if (_memberName == "") callerMethod = string.Empty;
        string scriptName = Path.GetFileName(_filePath);


        string logMessageRaw = (date + " " + time + " {Thread: " + System.Environment.CurrentManagedThreadId + "} - [LOG | " + _logLevel + "] " +
            LogLevelNormalization.logLevelNormalizationStrings[_logLevel] + " " +
            scriptName + callerMethod +
            ", line " + _lineNumber + ": " + _message);

        string logMessageColor = logMessageRaw.Pastel(GetColorCode(_logLevel));

        WriteToFileLogFile(_logLevel, logMessageRaw, scriptName);

        Console.WriteLine(logMessageColor);

        // Restricts what logging go to the #log channel, by the log level in logging parameters.
        if (_logLevel <= LoggingParameters.BotLogDiscordChannelLevel)
        {
            BotMessageLogging.SendLogMessage(logMessageRaw, _logLevel);
        }
    }

    private static Color GetColorCode(LogLevel _logLevel)
    {        
        switch (_logLevel)
        {
            case (LogLevel.CRITICAL): return Color.DarkRed;
            case (LogLevel.ERROR): return Color.Red;
            case (LogLevel.WARNING): return Color.Orange;
            case (LogLevel.IMPORTANT): return Color.Gold;
            case (LogLevel.SERIALIZATION): return Color.Blue;
            case (LogLevel.DEBUG): return Color.Green;
            case (LogLevel.ADD_VERBOSE): return Color.DarkBlue;
            case (LogLevel.SET_VERBOSE): return Color.DarkTeal;
            case (LogLevel.GET_VERBOSE): return Color.Teal;
            case (LogLevel.VERBOSE): return Color.Purple;
        }

        return Color.Default;
    }

    private static void WriteToFileLogFile(LogLevel _logLevel, string _logMessage, string _scriptName)
    {
        // Move to some other class that is more accessible
        string logsDir = @"C:\AirCombatMatchmakerBot\Logs\";

        CheckIfDirectoryExistsAndAppendToTheFile(logsDir, _logLevel.ToString(), _logMessage);
        CheckIfDirectoryExistsAndAppendToTheFile(logsDir, "EVERYTHING", _logMessage);

        if (_scriptName == "MatchScheduler.cs")
        {
            CheckIfDirectoryExistsAndAppendToTheFile(logsDir, _scriptName, _logMessage);
        }
    }


    // Move this out of the Log.cs file
    private static void CheckIfDirectoryExistsAndAppendToTheFile(string _directory, string _logLevel, string _content)
    {
        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
        }

        string fileExtension = ".log";
        string pathToFile = _directory + _logLevel + fileExtension;

        string contentWithNewLine = Environment.NewLine + _content;

        FileManager.AppendText(pathToFile, contentWithNewLine);
    }
}