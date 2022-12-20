using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseLeagueChannel : BaseChannel, InterfaceLeagueChannel
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

    public abstract override List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public abstract override Task ActivateChannelFeatures();
}