using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CachedUsers : logClass<CachedUsers>, InterfaceLoggableClass
{
    public ConcurrentBag<ulong> CachedUserIDs
    {
        get => cachedUserIDs.GetValue();
        set => cachedUserIDs.SetValue(value);
    }

    [DataMember] private logConcurrentBag<ulong> cachedUserIDs = new logConcurrentBag<ulong>();

    public List<string> GetClassParameters()
    {
        return new List<string> { cachedUserIDs.GetLoggingClassParameters<ulong, ulong>() };
    }

    public void AddUserIdToCachedConcurrentBag(ulong _userId)
    {
        Log.WriteLine("Adding " + _userId + " to the cache ConcurrentBag", LogLevel.VERBOSE);
        if (!CachedUserIDs.Contains(_userId))
        {
            CachedUserIDs.Add(_userId);
            Log.WriteLine("Added " + _userId +
                " to cached users ConcurrentBag.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("User " + _userId + " is already on the ConcurrentBag", LogLevel.VERBOSE);
        }
    }

    public void RemoveUserFromTheCachedConcurrentBag(ulong _userId)
    {
        Log.WriteLine("Removing " + _userId + " from the cache ConcurrentBag", LogLevel.VERBOSE);

        if (!CachedUserIDs.Contains(_userId))
        {
            Log.WriteLine("User " + _userId + " is not present on the ConcurrentBag!", LogLevel.WARNING);
            return;
        }

        //CachedUserIDs.TryRemove(_userId);

        while (CachedUserIDs.TryTake(out ulong element) && !element.Equals(_userId))
        {
            // If the element is not the one to remove, add it back to the bag
            CachedUserIDs.Add(element);
        }

        Log.WriteLine("Removed " + _userId + " from the cached users ConcurrentBag.", LogLevel.DEBUG);
    }

}