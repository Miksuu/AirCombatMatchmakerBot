using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logClass<T>
{
    [DataMember] private T _value;

    public logClass()
    {
        _value = default(T);
    }

    public logClass(T _initialValue = default(T))
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
        string finalVal = string.Empty;

        List<Type> regularVariableTypes = new List<Type>
            {
                typeof(ulong), typeof(Int32), typeof(float), typeof(bool)
            };

        if (regularVariableTypes.Contains(typeof(logClass<T>)))
        {
            finalVal = _value.ToString();
        }
        else 
        {
            StringBuilder membersBuilder = new StringBuilder();

            var interfaceLoggableClass = (InterfaceLoggableClass)this;

            foreach (var item in interfaceLoggableClass.GetClassParameters())
            {
                membersBuilder.Append(item).Append(", ");
            }

            finalVal = membersBuilder.ToString().TrimEnd(',', ' ');
        }

        Log.WriteLine("Getting " + _value.GetType() + " " + _memberName + ": " + 
            finalVal, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _value;
    }

    public void SetValue(T value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        string finalVal = string.Empty;

        List<Type> regularVariableTypes = new List<Type>
            {
                typeof(logClass<ulong>), typeof(logClass<Int32>),
            typeof(logClass<float>), typeof(logClass<bool>)
            };

        if (regularVariableTypes.Contains(typeof(logClass<T>)))
        {
            finalVal = _value.ToString();
        }
        else
        {
            StringBuilder membersBuilder = new StringBuilder();

            var interfaceLoggableClass = (InterfaceLoggableClass)this;

            foreach (var item in interfaceLoggableClass.GetClassParameters())
            {
                membersBuilder.Append(item).Append(", ");
            }

            finalVal = membersBuilder.ToString().TrimEnd(',', ' ');
        }

        Log.WriteLine("Setting " + _value.GetType() + " " + _memberName + ": " + finalVal +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }

    public string GetParameter<T>()
    {
        return _value.ToString();
    }
}
