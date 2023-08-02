using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class MATCHCHANNEL : BaseChannel
{
    public MATCHCHANNEL()
    {
        thisInterfaceChannel.ChannelType = ChannelType.MATCHCHANNEL;

        thisInterfaceChannel.ChannelMessages = new ConcurrentDictionary<MessageName, bool>(
            new ConcurrentBag<KeyValuePair<MessageName, bool>>()
            {
                new KeyValuePair<MessageName, bool>(MessageName.MATCHSTARTMESSAGE, false),
                new KeyValuePair<MessageName, bool>(MessageName.CONFIRMMATCHENTRYMESSAGE, false),
            });
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        var guild = BotReference.GetGuildRef();

        List<Overwrite> listOfOverwrites = new List<Overwrite>();

        Log.WriteLine("Overwriting permissions for: " + thisInterfaceChannel.ChannelName +
            " users that will be allowed access count: " +
            _allowedUsersIdsArray.Length);

        listOfOverwrites.Add(new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)));

        foreach (ulong userId in _allowedUsersIdsArray)
        {
            Log.WriteLine("Adding " + userId + " to the permission allowed List on: " +
                thisInterfaceChannel.ChannelName);

            listOfOverwrites.Add(
                new Overwrite(userId, PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow)));
        }

        return listOfOverwrites;
    }
}