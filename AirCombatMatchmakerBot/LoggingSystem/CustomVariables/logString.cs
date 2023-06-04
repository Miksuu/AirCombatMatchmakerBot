using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logString
{
    [DataMember] private string _value;

    public logString() { }
    public logString(string value)
    {
        Log.WriteLine("Creating " + nameof(logString) + " with value: " + value, LogLevel.SET_VERBOSE);
        _value = value;
    }

    public string GetParameter()
    {
        return _value;
    }

    public string GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting string " + _memberName + ": " + _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(string value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting string " + _memberName + ": " + _value + " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}
