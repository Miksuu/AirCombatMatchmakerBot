/*using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[DataContract]
public class logUlong
{
    [DataMember] private ulong _value;

    public logUlong() { }
    public logUlong(ulong value)
    {
        Log.WriteLine("Creating " + nameof(logUlong) + " with value: " + value, LogLevel.SET_VERBOSE);
        _value = value;
    }

    public ulong GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting ulong " + _memberName + ": " + _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(ulong value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ulong " + _memberName + ": " + _value + " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}*/