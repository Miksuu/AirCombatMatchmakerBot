using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Reflection.Metadata.Ecma335;

[DataContract]
public class MATCHCHANNEL : BaseChannel
{
    public MATCHCHANNEL()
    {
        channelType = ChannelType.MATCHCHANNEL;
        channelMessages = new List<MessageName>
        {
            MessageName.MATCHSTARTMESSAGE,
            MessageName.REPORTINGMESSAGE,
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        List<Overwrite> listOfOverwrites = new List<Overwrite>();

        Log.WriteLine("Overwriting permissions for: " + channelName +
            " users that will be allowed access count: " +
            _allowedUsersIdsArray.Length, LogLevel.VERBOSE);

        listOfOverwrites.Add(new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)));

        foreach (ulong userId in _allowedUsersIdsArray)
        {
            Log.WriteLine("Adding " + userId + " to the permission allowed list on: " +
                channelName, LogLevel.VERBOSE);

            listOfOverwrites.Add(
                new Overwrite(userId, PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow)));
        }

        return listOfOverwrites;
    }
}