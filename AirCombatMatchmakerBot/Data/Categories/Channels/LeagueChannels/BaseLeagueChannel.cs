using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseLeagueChannel : InterfaceLeagueChannel
{
    LeagueChannelName InterfaceLeagueChannel.LeagueChannelName
    {
        get => leagueChannelName;
        set => leagueChannelName = value;
    }

    ulong InterfaceLeagueChannel.LeagueChannelId
    {
        get => leagueChannelId;
        set => leagueChannelId = value;
    }
    Dictionary<string, ulong> InterfaceLeagueChannel.LeagueChannelFeaturesWithMessageIds
    {
        get => leagueChannelFeaturesWithMessageIds;
        set => leagueChannelFeaturesWithMessageIds = value;
    }

    public LeagueChannelName leagueChannelName;
    public ulong leagueChannelId;

    public Dictionary<string, ulong> leagueChannelFeaturesWithMessageIds;
    public BaseLeagueChannel()
    {
        leagueChannelFeaturesWithMessageIds = new Dictionary<string, ulong>();
    }

    public abstract List<Overwrite> GetGuildLeaguePermissions(SocketGuild _guild);

    public abstract Task ActivateLeagueChannelFeatures();
}