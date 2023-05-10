using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[DataContract]
public class logFloat
{
    [DataMember] private float _value;

    public logFloat() { }
    public logFloat(float value)
    {
        Log.WriteLine("Creating " + nameof(logFloat) + " with value: " + value, LogLevel.SET_VERBOSE);
        _value = value;
    }

    public float GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting float " + _memberName + ": " + _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(float value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting float " + _memberName + ": " + _value + " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}
