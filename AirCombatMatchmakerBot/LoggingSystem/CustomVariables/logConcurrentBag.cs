using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;


[DataContract]
public class logConcurrentBag<T> : IEnumerable<T>
{
    [DataMember] private ConcurrentBag<T> _values = new ConcurrentBag<T>();

    public logConcurrentBag() : base() { }

    public logConcurrentBag(IEnumerable<T> collection) : base() { }

    public ConcurrentBag<T> GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting ConcurrentBag " + _memberName + " with count: " +
            _values.Count + " that has members of: " + GetConcurrentBagMembers(_values),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _values;
    }

    public void SetValue(ConcurrentBag<T> values,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ConcurrentBag " + _memberName + " with count: " +_values.Count +
            " that has members of: " + GetConcurrentBagMembers(_values) + " TO: " + " with count: " +
            values.Count + " that has members of: " + GetConcurrentBagMembers(values),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        _values = values;
    }

    public void Add(T _item,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Adding item to ConcurrentBag " + _memberName + ": " + _item +
            " with count: " + _values.Count + " that has members of: " + GetConcurrentBagMembers(_values),
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

    private string GetConcurrentBagMembers(ConcurrentBag<T> _customValues)
    {
        string members = string.Empty;

        var type = typeof(T);
        foreach (var item in _customValues)
        {
            if (item is UnitName unitName)
            {
                members += EnumExtensions.GetEnumMemberAttrValue(unitName) + ", ";
            }
            else if (item is ChannelType channelType)
            {
                members += EnumExtensions.GetEnumMemberAttrValue(channelType) + ", ";
            }
            else
            {
                Log.WriteLine("Tried to get type: " + type + " unknown, undefined type?", LogLevel.CRITICAL);
                break;
            }
        }

        if (!string.IsNullOrEmpty(members))
        {
            members = members.Substring(0, members.Length - 2);
        }

        return members;
    }
}
