using System.Runtime.CompilerServices;

public class logUlong
{
    private ulong _value;

    public ulong GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting " + _memberName + ": " + _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(ulong value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting " + _memberName + ": " + _value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}
