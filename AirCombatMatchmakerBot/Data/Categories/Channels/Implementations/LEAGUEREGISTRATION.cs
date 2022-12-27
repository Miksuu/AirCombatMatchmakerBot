using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUEREGISTRATION : BaseChannel
{
    public LEAGUEREGISTRATION()
    {
        channelName = ChannelName.LEAGUEREGISTRATION;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
            new Overwrite(RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                _guild, "Member").Result.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Allow)),
        };
    }

    public override async Task ActivateChannelFeatures()
    {
        LeagueManager.leagueRegistrationChannelId = channelId;

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var channel = guild.GetTextChannel(channelId) as ITextChannel;

        if (channel == null)
        {
            Log.WriteLine("Channel was null with id: " + channelId, LogLevel.ERROR);
            return;
        }
        Log.WriteLine("Channel found: " + channel.Name +
            "(" + channel.Id + ")", LogLevel.VERBOSE);

        await CreateLeagueMessages(this, channel);        
    }

    public static async Task CreateLeagueMessages(LEAGUEREGISTRATION _LEAGUEREGISTRATION,
    ITextChannel _leagueRegistrationChannel)
    {
        foreach (CategoryName leagueName in Enum.GetValues(typeof(CategoryName)))
        {
            Log.WriteLine("Looping on: " + leagueName.ToString(), LogLevel.VERBOSE);

            // Skip all the non-leagues
            int enumValue = (int)leagueName;
            if (enumValue > 100) continue;

            string? leagueNameString = EnumExtensions.GetEnumMemberAttrValue(leagueName);
            Log.WriteLine("leagueNameString: " + leagueNameString, LogLevel.VERBOSE);

            if (leagueNameString == null)
            {
                Log.WriteLine(nameof(leagueNameString) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Printing all keys and values in: " + nameof(
                _LEAGUEREGISTRATION.channelFeaturesWithMessageIds) + " that has count of: " +
                _LEAGUEREGISTRATION.channelFeaturesWithMessageIds.Count, LogLevel.VERBOSE);
            foreach (var item in _LEAGUEREGISTRATION.channelFeaturesWithMessageIds)
            {
                Log.WriteLine("Key in db: " + item.Key +
                    " with value: " + item.Value, LogLevel.VERBOSE);
            }

            // Checks if the message is present in the channelMessages
            bool containsMessage = false;
            var channelMessages =
                await _leagueRegistrationChannel.GetMessagesAsync(
                    50, CacheMode.AllowDownload).FirstAsync();

            Log.WriteLine("Searching: " + leagueNameString + " from: " + nameof(channelMessages) +
                " with a count of: " + channelMessages.Count, LogLevel.VERBOSE);

            foreach (var msg in channelMessages)
            {
                Log.WriteLine("Looping on msg: " + msg.Content.ToString(), LogLevel.VERBOSE);
                if (msg.Content.Contains(leagueNameString))
                {
                    Log.WriteLine($"contains: {msg.Content}", LogLevel.VERBOSE);
                    containsMessage = true;
                }
            }

            // If the channelMessages features got this already, if yes, continue, otherwise finish
            // the operation then save it to the dictionary
            if (_LEAGUEREGISTRATION.channelFeaturesWithMessageIds.ContainsKey(
                leagueNameString) && containsMessage)
            {
                Log.WriteLine("The key " + leagueNameString + " was already found in: " +
                    nameof(_LEAGUEREGISTRATION.channelFeaturesWithMessageIds) +
                    ", continuing.", LogLevel.VERBOSE);
                continue;
            }

            var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
            if (leagueInterface == null)
            {
                Log.WriteLine("leagueInterface was null!", LogLevel.CRITICAL);
                return;
            }

            var leagueInterfaceFromDatabase =
                Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

            ulong leagueRegistrationChannelMessageId =
                await LeagueChannelManager.CreateALeagueJoinButton(
                    _leagueRegistrationChannel, leagueInterfaceFromDatabase, leagueNameString);

            Log.WriteLine("id:" + leagueRegistrationChannelMessageId, LogLevel.VERBOSE);

            _LEAGUEREGISTRATION.channelFeaturesWithMessageIds.Add(
                leagueNameString, leagueRegistrationChannelMessageId);

            Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);
        }
    }
}