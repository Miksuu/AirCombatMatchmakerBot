using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[DataContract]
public class logInt
{
    [DataMember] private int _value;

    public logInt() { }
    public logInt(int value)
    {
        Log.WriteLine("Creating " + nameof(logInt) + " with value: " + value, LogLevel.SET_VERBOSE);
        _value = value;
    }

    public int GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting int " + _memberName + ": " + _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(int value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting int " + _memberName + ": " + _value + " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}
