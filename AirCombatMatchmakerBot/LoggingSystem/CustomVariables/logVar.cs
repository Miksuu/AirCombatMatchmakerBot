using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logVar<T> 
{
    [DataMember] private T? _value;

    public logVar()
    {
        _value = default(T);
    }

    public logVar(T?_initialValue = default(T))
    {
        if (_initialValue == null)
        {
            _value = default(T);
        }
        else
        {
            _value = _initialValue;
        }
    }

    public T GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting " + _value?.GetType() + " " + _memberName + ": " +
                    _value, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(T value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting " + _value?.GetType() + " " + _memberName + ": " +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }

    public string GetParameter()
    {
        return _value?.ToString() ?? "[null]";
    }
}
