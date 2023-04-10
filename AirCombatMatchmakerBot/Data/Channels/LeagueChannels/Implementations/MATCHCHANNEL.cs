using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Concurrent;

[DataContract]
public class MATCHCHANNEL : BaseChannel
{
    public MATCHCHANNEL()
    {
        channelType = ChannelType.MATCHCHANNEL;

        channelMessages = new ConcurrentDictionary<MessageName, bool>(
            new ConcurrentBag<KeyValuePair<MessageName, bool>>()
            {
                new KeyValuePair<MessageName, bool>(MessageName.MATCHSTARTMESSAGE, false),
                new KeyValuePair<MessageName, bool>(MessageName.REPORTINGMESSAGE, false),
                new KeyValuePair<MessageName, bool>(MessageName.REPORTINGSTATUSMESSAGE, false),
            });
    }

    public override ConcurrentBag<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        ConcurrentBag<Overwrite> ConcurrentBagOfOverwrites = new ConcurrentBag<Overwrite>();

        Log.WriteLine("Overwriting permissions for: " + channelName +
            " users that will be allowed access count: " +
            _allowedUsersIdsArray.Length, LogLevel.VERBOSE);

        ConcurrentBagOfOverwrites.Add(new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)));

        foreach (ulong userId in _allowedUsersIdsArray)
        {
            Log.WriteLine("Adding " + userId + " to the permission allowed ConcurrentBag on: " +
                channelName, LogLevel.VERBOSE);

            ConcurrentBagOfOverwrites.Add(
                new Overwrite(userId, PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow)));
        }

        return ConcurrentBagOfOverwrites;
    }

    public (InterfaceLeague?, LeagueMatch?, string) FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
        ulong _buttonCategoryId, ulong _messageChannelId)
    {
        Log.WriteLine("Trying to find a league match with: " + _buttonCategoryId, LogLevel.VERBOSE);

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(_buttonCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.CRITICAL);
            return (null, null, "Could not find the interface league");
        }

        LeagueMatch? foundMatch =
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(
                _messageChannelId);
        if (foundMatch == null)
        {
            Log.WriteLine("Match with: " + _messageChannelId +
                " was not found.", LogLevel.CRITICAL);
            return (interfaceLeague, null, "Could not find the LeagueMatch!");
        }

        return (interfaceLeague, foundMatch, "");
    }
}