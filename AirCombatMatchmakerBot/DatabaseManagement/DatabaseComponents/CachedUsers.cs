using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CachedUsers
{
    public ConcurrentBag<ulong> CachedUserIDs
    {
        get
        {
            Log.WriteLine("Getting " + nameof(cachedUserIDs) + " with count of: " +
                cachedUserIDs.Count, LogLevel.VERBOSE);
            return cachedUserIDs;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(cachedUserIDs)
                + " to: " + value, LogLevel.VERBOSE);
            cachedUserIDs = value;
        }
    }

    [DataMember] private ConcurrentBag<ulong> cachedUserIDs { get; set; }

    public CachedUsers()
    {
        cachedUserIDs = new ConcurrentBag<ulong>();
    }

    public void AddUserIdToCachedConcurrentBag(ulong _userId)
    {
        Log.WriteLine("Adding " + _userId + " to the cache ConcurrentBag", LogLevel.VERBOSE);
        if (!CachedUserIDs.Contains(_userId))
        {
            cachedUserIDs.Add(_userId);
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