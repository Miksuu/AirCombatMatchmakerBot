using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

/*
[DataContract]
public abstract class BaseLeagueChannel : InterfaceChannel
{
    ChannelName InterfaceChannel.ChannelName
    {
        get => leagueChannelName;
        set => leagueChannelName = value;
    }

    ulong InterfaceChannel.ChannelId
    {
        get => leagueChannelId;
        set => leagueChannelId = value;
    }
    Dictionary<string, ulong> InterfaceChannel.ChannelFeaturesWithMessageIds
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

    public abstract Task ActivateLeagueChannelFeatures();
}*/