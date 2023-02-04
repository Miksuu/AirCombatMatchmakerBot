using System.Runtime.Serialization;

[DataContract]
public class CachedUsers
{
    public List<ulong> CachedUserIDs
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

    [DataMember] private List<ulong> cachedUserIDs { get; set; }

    public CachedUsers()
    {
        cachedUserIDs = new List<ulong>();
    }

    public void AddUserIdToCachedList(ulong _userId)
    {
        Log.WriteLine("Adding " + _userId + " to the cache list", LogLevel.VERBOSE);
        if (!CachedUserIDs.Contains(_userId))
        {
            cachedUserIDs.Add(_userId);
            Log.WriteLine("Added " + _userId +
                " to cached users list.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("User " + _userId + " is already on the list", LogLevel.VERBOSE);
        }
    }

    public void RemoveUserFromTheCachedList(ulong _userId)
    {
        Log.WriteLine("Removing " + _userId + " from the cache list", LogLevel.VERBOSE);

        if (!CachedUserIDs.Contains(_userId))
        {
            Log.WriteLine("User " + _userId + " is not present on the list!", LogLevel.WARNING);
            return;
        }

        CachedUserIDs.Remove(_userId);

        Log.WriteLine("Removed " + _userId + " from the cached users list.", LogLevel.DEBUG);
    }

}