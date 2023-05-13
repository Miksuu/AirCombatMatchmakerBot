using Newtonsoft.Json.Linq;
using System.Reflection;
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
                typeof(ulong), typeof(System.Int32), typeof(float), typeof(bool)
            };

        if (regularVariableTypes.Contains(typeof(T)))
        {
            finalVal = _value.ToString();
        }
        else 
        {
            StringBuilder membersBuilder = new StringBuilder();

            var interfaceLoggableClass = Activator.CreateInstance(typeof(T)) as InterfaceLoggableClass;
            Log.WriteLine(typeof(T).ToString(), LogLevel.VERBOSE);
            MethodInfo getParametersMethod = interfaceLoggableClass.GetType().GetMethod("GetClassParameters");
            if (getParametersMethod != null)
            {
                List<string> parameters = getParametersMethod.Invoke(interfaceLoggableClass, null) as List<string>;
                if (parameters != null)
                {
                    foreach (string param in parameters)
                    {
                        membersBuilder.Append(param).Append(", ");
                    }
                }
            }
            else
            {
                Log.WriteLine(typeof(T) + " does not have: " + nameof(interfaceLoggableClass.GetClassParameters), LogLevel.WARNING);
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

            var interfaceLoggableClass = Activator.CreateInstance(typeof(T)) as InterfaceLoggableClass;
            Log.WriteLine(typeof(T).ToString(), LogLevel.VERBOSE);
            MethodInfo getParametersMethod = interfaceLoggableClass.GetType().GetMethod("GetClassParameters");
            if (getParametersMethod != null)
            {
                List<string> parameters = getParametersMethod.Invoke(interfaceLoggableClass, null) as List<string>;
                if (parameters != null)
                {
                    foreach (string param in parameters)
                    {
                        membersBuilder.Append(param).Append(", ");
                    }
                }
            }

            finalVal = membersBuilder.ToString().TrimEnd(',', ' ');
        }

        Log.WriteLine("Setting " + _value.GetType() + " " + _memberName + ": " + finalVal +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }

    public string GetParameter()
    {
        if (_value == null) return "[null]";

        return _value.ToString();
    }
}
