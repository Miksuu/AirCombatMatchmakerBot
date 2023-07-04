using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logEnum<T> where T : Enum
{
    [DataMember]
    private T value;

    public logEnum() : base()
    {
        value = default(T);
    }

    public T GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting " + _memberName,
                     LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return value;
    }

    public void SetValue(T value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting " + _memberName +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        this.value = value;
    }
}
