using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class BOTLOG : BaseChannel
{
    public BOTLOG()
    {
        channelType = ChannelType.BOTLOG;
    }

    public override ConcurrentBag<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new ConcurrentBag<Overwrite>
        {
        };
    }

    /*
    public override Task PrepareChannelMessages()
    {
        Log.WriteLine("Setting logging channel id with: " + channelId, LogLevel.VERBOSE);
        return Task.CompletedTask;
    }*/
}