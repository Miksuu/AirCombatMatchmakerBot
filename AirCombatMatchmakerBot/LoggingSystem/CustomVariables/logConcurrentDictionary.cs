using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logConcurrentDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, InterfaceLoggingClass
{
    [DataMember] private ConcurrentDictionary<TKey, TValue> _values;

    public logConcurrentDictionary()
    {
        _values = new ConcurrentDictionary<TKey, TValue>();
    }

    public logConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
        _values = new ConcurrentDictionary<TKey, TValue>(collection ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>());
    }

    public string GetLoggingClassParameters<TKey, TValue>()
    {
        StringBuilder membersBuilder = new StringBuilder();
        foreach (var kvp in _values)
        {
            string finalValueForTheProperty = string.Empty;

            List<Type> regularVariableTypes = new List<Type>
            {
                typeof(ulong), typeof(Int32), typeof(float), typeof(bool)
            };

            if (regularVariableTypes.Contains(kvp.Key.GetType()))
            {
                finalValueForTheProperty = kvp.Key.ToString();
            }
            else
            {
                if (kvp.Key is logClass<TKey>)
                {
                    finalValueForTheProperty = ((logClass<TKey>)(object)kvp.Key).GetParameter();
                }
            }

            if (regularVariableTypes.Contains(kvp.Value.GetType()))
            {
                finalValueForTheProperty = kvp.Value.ToString();
            }
            else
            {
                if (kvp.Value is logClass<TValue>)
                {
                    Log.WriteLine(kvp.Value.ToString(), LogLevel.VERBOSE);
                    finalValueForTheProperty = ((logClass<TValue>)(object)kvp.Value).GetParameter();
                }
            }

            membersBuilder.Append(finalValueForTheProperty).Append(", ");
        }

        return membersBuilder.ToString().TrimEnd(',', ' ');
    }


    public ConcurrentDictionary<TKey, TValue> GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting ConcurrentBag " + _memberName + " with count: " +
            _values.Count + " that has members of: " + GetLoggingClassParameters<TKey,TValue>(),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _values;
    }

    public void SetValue(ConcurrentDictionary<TKey, TValue> values,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ConcurrentBag " + _memberName + " with count: " +_values.Count +
            " that has members of: " + GetLoggingClassParameters<TKey, TValue>()+ " TO: " + " with count: " +
            values.Count + " that has members of: " + GetLoggingClassParameters<TKey, TValue>(),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        _values = values;
    }

    public void Add(KeyValuePair<TKey, TValue> _itemKvp,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Adding item to ConcurrentBag " + _memberName + ": " + _itemKvp +
            " with count: " + _values.Count + " that has members of: " + GetLoggingClassParameters<TKey, TValue>(),
            LogLevel.ADD_VERBOSE, _filePath, "", _lineNumber);

        var key = _itemKvp.Key;
        var val = _itemKvp.Value;
        _values.TryAdd(key, val);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
