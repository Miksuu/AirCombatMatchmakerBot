using Discord;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.Serialization;

[DataContract]
public class Database
{
    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, ulong> MatchChannelsIdWithCategoryId
    {
        get => matchChannelsIdWithCategoryId.GetValue();
        set => matchChannelsIdWithCategoryId.SetValue(value);
    }

    public ConcurrentBag<LeagueMatch> ArchivedLeagueMatches
    {
        get => archivedLeagueMatches.GetValue();
        set => archivedLeagueMatches.SetValue(value);
    }

    public static Database Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }
        set
        {
            instance = value;
        }
    }

    // Singleton stuff
    private static Database? instance;
    private static readonly object padlock = new object();

    static string appName = GetApplicationName();



    // The Database components
    [DataMember] public CachedUsers CachedUsers = new CachedUsers();
    [DataMember] public Leagues Leagues = new Leagues();
    [DataMember] public PlayerData PlayerData = new PlayerData();

    [DataMember]
    public logConcurrentDictionary<ulong, ulong> matchChannelsIdWithCategoryId =
    new logConcurrentDictionary<ulong, ulong>();
    [DataMember]
    public logConcurrentBag<LeagueMatch> archivedLeagueMatches = new logConcurrentBag<LeagueMatch>();

    static string GetApplicationName()
    {
        // Get the current assembly (the assembly where your application is defined)
        Assembly assembly = Assembly.GetEntryAssembly();

        // Get the assembly's full name, which includes the application name
        string assemblyName = assembly?.GetName()?.FullName;

        // Extract the application name from the full name
        // The application name is the part before the first comma in the full name
        int commaIndex = assemblyName.IndexOf(',');
        string appName = (commaIndex > 0) ? assemblyName.Substring(0, commaIndex) : assemblyName;

        return appName;
    }
}