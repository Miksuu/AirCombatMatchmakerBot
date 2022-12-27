using System.Runtime.Serialization;

[DataContract]
public class CachedUsers
{
    List<ulong> CachedUserIDs
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

    /*
    public List<ulong> GetListOfCachedUserIds()
    {
        Log.WriteLine("Getting list of cachedUserIDs with count of: " + cachedUserIDs.Count, LogLevel.VERBOSE);
        return cachedUserIDs;
    }*/

    public async void AddUserIdToCachedList(ulong _userId)
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

    public async void RemoveUserFromTheCachedList(string _userName, ulong _userId)
    {
        Log.WriteLine("Removing " + _userName + "(" + _userId + ")" + " from the cache list", LogLevel.VERBOSE);

        if (!CachedUserIDs.Contains(_userId))
        {
            Log.WriteLine("User " + _userName + " is not present on the list!", LogLevel.WARNING);
            return;
        }

        CachedUserIDs.Remove(_userId);

        Log.WriteLine("Removed " + _userName + " from the cached users list.", LogLevel.DEBUG);

        await SerializationManager.SerializeDB();
    }

}