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
        string? finalVal = string.Empty;

        List<Type> regularVariableTypes = new List<Type>
            {
                typeof(ulong), typeof(System.Int32), typeof(float), typeof(bool)
            };

        if (regularVariableTypes.Contains(typeof(T)))
        {
            finalVal = _value != null ? _value.ToString() : null;
        }
        /*
        else
        {
            StringBuilder membersBuilder = new StringBuilder();

            var fields =
                typeof(T).GetFields();/*.Where(f => f.FieldType == typeof(logClass<T>) ||
                f.FieldType == typeof(logConcurrentBag<T>) ||
                f.FieldType == typeof(logConcurrentDictionary<T, T>) ||
                f.FieldType == typeof(string));

            if (fields.Count() > 0)
            {

                foreach (var item in fields)
                {
                    Log.WriteLine("fieldtype: " + item.FieldType, LogLevel.DEBUG);
                }


            }

            foreach (var field in fields)
            {
                var getParameterMethod = field.FieldType.GetMethod("GetParameter");
                var fieldValue = field.GetValue(null);

                if (getParameterMethod != null && fieldValue != null)
                {
                    var parameters = getParameterMethod.Invoke(fieldValue, null) as List<string>;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            membersBuilder.Append(param).Append(", ");
                        }
                    }
                }
            }

            if (membersBuilder.Length > 0)
            {
                membersBuilder.Length -= 2;
            }

            finalVal = membersBuilder.ToString();
        }*/

        Log.WriteLine("Getting " + _value?.GetType() + " " + _memberName + ": " +
                    finalVal, LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);

        return _value;
    }

    public void SetValue(T value,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        string? finalVal = string.Empty;

        List<Type> regularVariableTypes = new List<Type>
            {
                typeof(ulong), typeof(System.Int32), typeof(float), typeof(bool)
            };

        if (regularVariableTypes.Contains(typeof(T)))
        {
            finalVal = _value != null ? _value.ToString() : null;
        }
        /*
        else
        {
            StringBuilder membersBuilder = new StringBuilder();

            var fields =
                typeof(T).GetFields();/*.Where(f => f.FieldType == typeof(logClass<T>) ||
                f.FieldType == typeof(logConcurrentBag<T>) ||
                f.FieldType == typeof(logConcurrentDictionary<T, T>) ||
                f.FieldType == typeof(string));

            if (fields.Count() > 0)
            {

                foreach (var item in fields)
                {
                    Log.WriteLine("fieldtype: " + item.FieldType, LogLevel.DEBUG);
                }


            }

            foreach (var field in fields)
            {
                var getParameterMethod = field.FieldType.GetMethod("GetParameter");
                var fieldValue = field.GetValue(null);

                if (getParameterMethod != null && fieldValue != null)
                {
                    var parameters = getParameterMethod.Invoke(fieldValue, null) as List<string>;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            membersBuilder.Append(param).Append(", ");
                        }
                    }
                }
            }

            if (membersBuilder.Length > 0)
            {
                membersBuilder.Length -= 2;
            }

            finalVal = membersBuilder.ToString();
        }*/

        Log.WriteLine("Setting " + _value?.GetType() + " " + _memberName + ": " + finalVal +
            " TO: " + value, LogLevel.SET_VERBOSE, _filePath, "", _lineNumber);
        _value = value;
    }

    public string GetParameter()
    {
        return _value?.ToString() ?? "[null]";
    }
}
