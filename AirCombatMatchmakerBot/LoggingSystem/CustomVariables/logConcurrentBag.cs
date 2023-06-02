using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logConcurrentBag<T> : IEnumerable<T>, InterfaceLoggingClass where T : notnull
{
    [DataMember] private ConcurrentBag<T> _values = new ConcurrentBag<T>();

    public logConcurrentBag() { }

    public logConcurrentBag(IEnumerable<T> collection)
    {
        _values = new ConcurrentBag<T>(collection);
    }

    public string GetLoggingClassParameters()
    {
        StringBuilder membersBuilder = new StringBuilder();
        foreach (T concurrentBag in _values)
        {
            string? finalValueForTheProperty = string.Empty;

            List<Type> regularVariableTypes = new List<Type>
        {
            typeof(ulong), typeof(Int32), typeof(float), typeof(bool)
        };

            if (concurrentBag == null)
            {
                finalValueForTheProperty = "null";
            }
            else if (regularVariableTypes.Contains(concurrentBag.GetType()))
            {
                finalValueForTheProperty = concurrentBag.ToString();
            }
            else if (concurrentBag is logVar<T> logClass)
            {
                finalValueForTheProperty = logClass.GetParameter();
            }

            membersBuilder.Append(finalValueForTheProperty).Append(", ");
        }

        return membersBuilder.ToString().TrimEnd(',', ' ');
    }


    public ConcurrentBag<T> GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting ConcurrentBag " + _memberName + " with count: " +
            _values.Count + " that has members of: " + GetLoggingClassParameters(),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _values;
    }

    public void SetValue(ConcurrentBag<T> values,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ConcurrentBag " + _memberName + " with count: " +_values.Count +
            " that has members of: " + GetLoggingClassParameters() + " TO: " + " with count: " +
            values.Count + " that has members of: " + GetLoggingClassParameters(),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        _values = values;
    }

    public void Add(T _item,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Adding item to ConcurrentBag " + _memberName + ": " + _item +
            " with count: " + _values.Count + " that has members of: " + GetLoggingClassParameters(),
            LogLevel.ADD_VERBOSE, _filePath, "", _lineNumber);
        _values.Add(_item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
