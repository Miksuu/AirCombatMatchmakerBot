using Discord;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.Serialization;

[DataContract]
public class ApplicationDatabase : Database
{
    ApplicationDatabase()
    {
        dataDirectory = DatabasePaths.applicationDataDirectory;
        dbTempPathWithFileName = dataDirectory + @"\" + "database.tmp";
    }

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

    // The Database components
    [DataMember] public CachedUsers CachedUsers = new CachedUsers();
    [DataMember] public Leagues Leagues = new Leagues();
    [DataMember] public PlayerData PlayerData = new PlayerData();

    [DataMember]
    public logConcurrentDictionary<ulong, ulong> matchChannelsIdWithCategoryId =
    new logConcurrentDictionary<ulong, ulong>();
    [DataMember]
    public logConcurrentBag<LeagueMatch> archivedLeagueMatches = new logConcurrentBag<LeagueMatch>();
}