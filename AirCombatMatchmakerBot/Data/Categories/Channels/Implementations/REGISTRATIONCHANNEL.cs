using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class REGISTRATIONCHANNEL : BaseChannel
{
    public REGISTRATIONCHANNEL()
    {
        channelName = ChannelName.REGISTRATIONCHANNEL;
        botChannelType = BotChannelType.CHANNEL;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
            new Overwrite(RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                _guild, "Member").Result.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }

    public override async Task ActivateChannelFeatures()
    {
        Log.WriteLine("Activating channel features on " +
            nameof(REGISTRATIONCHANNEL) + " id: " + base.channelId, LogLevel.VERBOSE);

        string channelFeatureKey = "mainRegistration";

        if (channelFeaturesWithMessageIds.ContainsKey(channelFeatureKey))
        {
            Log.WriteLine("Already contains key " + channelFeatureKey, LogLevel.VERBOSE);
            return;
        }

        Log.WriteLine("Does not contain the key: " + channelFeatureKey + ", continuing", LogLevel.VERBOSE);

        channelFeaturesWithMessageIds.Add(channelFeatureKey,
            await PlayerRegisteration.CreateMainRegisterationChannelButton(channelId));

        Log.WriteLine("Done activating channel features on " +
            nameof(REGISTRATIONCHANNEL) + " id: " + base.channelId, LogLevel.VERBOSE);
    }
}