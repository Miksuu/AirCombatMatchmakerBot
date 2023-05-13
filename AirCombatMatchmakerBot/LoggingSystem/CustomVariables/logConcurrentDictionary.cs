using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    public string GetLoggingClassParameters()
    {
        StringBuilder membersBuilder = new StringBuilder();
        foreach (KeyValuePair<TKey, TValue> kvp in _values)
        {
            string? finalValueForTheProperty = string.Empty;

            List<Type> regularVariableTypes = new List<Type>
            {
                typeof(ulong), typeof(Int32), typeof(float), typeof(bool)
            };

            if (kvp.Key?.GetType() is Type keyType && regularVariableTypes.Contains(keyType))
            {
                finalValueForTheProperty = kvp.Key.ToString();
            }
            else if (kvp.Key is logClass<TKey> keyLogClass)
            {
                finalValueForTheProperty = keyLogClass.GetParameter();
            }

            if (kvp.Value?.GetType() is Type valueType && regularVariableTypes.Contains(valueType))
            {
                finalValueForTheProperty = kvp.Value.ToString();
            }
            else if (kvp.Value is logClass<TValue> valueLogClass)
            {
                finalValueForTheProperty = valueLogClass.GetParameter();
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
            _values.Count + " that has members of: " + GetLoggingClassParameters(),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _values;
    }

    public void SetValue(ConcurrentDictionary<TKey, TValue> values,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ConcurrentBag " + _memberName + " with count: " +_values.Count +
            " that has members of: " + GetLoggingClassParameters()+ " TO: " + " with count: " +
            values.Count + " that has members of: " + GetLoggingClassParameters(),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        _values = values;
    }

    public void Add(KeyValuePair<TKey, TValue> _itemKvp,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Adding item to ConcurrentBag " + _memberName + ": " + _itemKvp +
            " with count: " + _values.Count + " that has members of: " + GetLoggingClassParameters(),
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
