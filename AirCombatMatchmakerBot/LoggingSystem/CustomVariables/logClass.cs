using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[DataContract]
public class logClass<T>
{
    [DataMember] private T _value;

    public logClass(T initialValue = default(T))
    {
        if (initialValue == null)
        {
            _value = default(T);
        }
        else
        {
            _value = initialValue;
        }
    }

    public T GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting " + _value.GetType() + " " + _memberName + ": " + 
            _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(T value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting " + _value.GetType() + " "+ _memberName + ": " + _value +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}
