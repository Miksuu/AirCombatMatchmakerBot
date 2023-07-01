using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logClass<T> 
{
    [DataMember] private T? _value;

    public logClass()
    {
        _value = default(T);
    }

    public logClass(T?_initialValue = default(T))
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
        Log.WriteLine("Getting " + _memberName,
                     LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(T value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting " + _memberName +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }
}
