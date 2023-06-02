using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logClass
{
    [DataMember]
    protected object _value;

    public logClass()
    {
        _value = null;
    }

    public logClass(object _initialValue)
    {
        _value = _initialValue;
    }

    public object GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        string finalVal;

        List<System.Type> regularVariableTypes = new List<System.Type>
        {
            typeof(ulong), typeof(System.Int32), typeof(float), typeof(bool)
        };

        if (regularVariableTypes.Contains(_value?.GetType()))
        {
            finalVal = _value?.ToString();
        }
        else
        {
            StringBuilder membersBuilder = new StringBuilder();

            var interfaceLoggableClass = _value as InterfaceLoggableClass;
            MethodInfo getParametersMethod = interfaceLoggableClass?.GetType().GetMethod("GetClassParameters");
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
            else if (_value is System.Enum)
            {
                finalVal = _value?.ToString();
            }
            else
            {
                Log.WriteLine(_value?.GetType() + " does not have the method: " + nameof(interfaceLoggableClass.GetClassParameters), LogLevel.WARNING);
            }

            finalVal = membersBuilder.ToString().TrimEnd(',', ' ');
        }

        Log.WriteLine("Getting " + _value?.GetType() + " " + _memberName + ": " +
                    finalVal, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);

        return _value;
    }

    public void SetValue(object value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        string finalVal;

        List<System.Type> regularVariableTypes = new List<System.Type>
        {
            typeof(ulong), typeof(System.Int32), typeof(float), typeof(bool)
        };

        if (regularVariableTypes.Contains(_value?.GetType()))
        {
            finalVal = _value?.ToString();
        }
        else
        {
            StringBuilder membersBuilder = new StringBuilder();

            var interfaceLoggableClass = _value as InterfaceLoggableClass;
            MethodInfo getParametersMethod = interfaceLoggableClass?.GetType().GetMethod("GetClassParameters");
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
            else if (_value is System.Enum)
            {
                finalVal = _value?.ToString();
            }
            else
            {
                Log.WriteLine(_value?.GetType() + " does not have the method: " + nameof(interfaceLoggableClass.GetClassParameters), LogLevel.WARNING);
            }

            finalVal = membersBuilder.ToString().TrimEnd(',', ' ');
        }

        Log.WriteLine("Setting " + _value?.GetType() + " " + _memberName + ": " + finalVal +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }

    public string GetParameter()
    {
        return _value?.ToString() ?? "[null]";
    }
}
