using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using Discord.Commands;

[DataContract]
public class LEAGUEREGISTRATION : BaseChannel
{
    public LEAGUEREGISTRATION()
    {
        channelName = ChannelName.LEAGUEREGISTRATION;
        channelMessages = new List<MessageName> { MessageName.LEAGUEREGISTRATIONMESSAGE };

        /*
        // Add that there will be same amount of league registration messages than done leagues
        foreach (CategoryName leagueName in Enum.GetValues(typeof(CategoryName)))
        {
            Log.WriteLine("Looping on: " + leagueName.ToString(), LogLevel.VERBOSE);

            // Skip all the non-leagues
            int enumValue = (int)leagueName;
            if (enumValue > 100) continue;
            */
        //channelMessages.Add(MessageName.LEAGUEREGISTRATIONMESSAGE);
       // }
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

    public override async Task PrepareChannelMessages()
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

            /*
            Log.WriteLine("Printing all keys and values in: " + nameof(
                ChannelMessagesWithIds) + " that has count of: " +
                channelFeaturesWithMessageIds.Count, LogLevel.VERBOSE);
            foreach (var item in channelFeaturesWithMessageIds)
            {
                Log.WriteLine("Key in db: " + item.Key +
                    " with value: " + item.Value, LogLevel.VERBOSE);
            }*/

            /*
            // Checks if the message is present in the channelMessages
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
                    //containsMessage = true;
                }
            }

            
            // If the channelMessages features got this already, if yes, continue, otherwise finish
            // the operation then save it to the dictionary
            if (channelMessages.ContainsKey(
                leagueNameString) && containsMessage)
            {
                Log.WriteLine("The key " + leagueNameString + " was already found in: " +
                    nameof(channelFeaturesWithMessageIds) +
                    ", continuing.", LogLevel.VERBOSE);
                continue;
            }*/

            var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
            if (leagueInterface == null)
            {
                Log.WriteLine("leagueInterface was null!", LogLevel.CRITICAL);
                return;
            }

            var leagueInterfaceFromDatabase =
                Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

            /*
            ulong leagueRegistrationChannelMessageId =
                await LeagueChannelManager.CreateALeagueJoinButton(
                    _leagueRegistrationChannel, leagueInterfaceFromDatabase, leagueNameString);
            */

            Log.WriteLine("Starting to create a league join button for: " + leagueNameString, LogLevel.VERBOSE);

            if (leagueInterfaceFromDatabase == null)
            {
                Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine(nameof(leagueInterfaceFromDatabase) + " before creating leagueButtonRegisterationCustomId: "
                + leagueInterfaceFromDatabase.ToString(), LogLevel.VERBOSE);

            /*
            string leagueButtonRegisterationCustomId =
               "LEAGUEREGISTRATION" + leagueInterfaceFromDatabase.DiscordLeagueReferences.LeagueCategoryId;
 
            Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
                leagueButtonRegisterationCustomId, LogLevel.VERBOSE);           */

            InterfaceMessage interfaceMessage =
                (InterfaceMessage)EnumExtensions.GetInstance(channelMessages.ElementAt(0).ToString());
            Log.WriteLine("Created interfaceMessage instance: " +
                interfaceMessage.MessageName, LogLevel.VERBOSE);

            interfaceMessage.Message = leagueInterfaceFromDatabase.GenerateALeagueJoinButtonMessage();

            Log.WriteLine("interfaceMessage message: " + interfaceMessage.Message, LogLevel.VERBOSE);

            interfaceMessagesWithIds.Add(interfaceMessage, leagueName.ToString());

            Log.WriteLine("Added to the dictionary, count is now: " +
                interfaceMessagesWithIds.Count, LogLevel.VERBOSE);

            /*
            leagueInterfaceFromDatabase.DiscordLeagueReferences.LeagueRegistrationChannelMessageId =
                await ButtonComponents.CreateButtonMessage(
                    _leagueRegistrationChannel.Id,
                    leagueInterface.GenerateALeagueJoinButtonMessage(),
                    "Join",
                    leagueButtonRegisterationCustomId); // Maybe replace this with some other system
            Log.WriteLine("Done creating a league join button for: " + leagueNameString, LogLevel.DEBUG);

            //_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId;


            //Log.WriteLine("id:" + leagueRegistrationChannelMessageId, LogLevel.VERBOSE);

            /*
            channelFeaturesWithMessageIds.Add(
                leagueNameString, leagueRegistrationChannelMessageId);
            */

            Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);
        }

        await base.PostChannelMessages();
    }

    /*
    public async Task PostChannelMessages()
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

        await CreateLeagueMessages(channel);        
    }*/

    public async Task CreateLeagueMessages(ITextChannel _leagueRegistrationChannel)
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

            /*
            Log.WriteLine("Printing all keys and values in: " + nameof(
                ChannelMessagesWithIds) + " that has count of: " +
                channelFeaturesWithMessageIds.Count, LogLevel.VERBOSE);
            foreach (var item in channelFeaturesWithMessageIds)
            {
                Log.WriteLine("Key in db: " + item.Key +
                    " with value: " + item.Value, LogLevel.VERBOSE);
            }*/

            // Checks if the message is present in the channelMessages
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
                    //containsMessage = true;
                }
            }
            /*
            // If the channelMessages features got this already, if yes, continue, otherwise finish
            // the operation then save it to the dictionary
            if (channelMessages.ContainsKey(
                leagueNameString) && containsMessage)
            {
                Log.WriteLine("The key " + leagueNameString + " was already found in: " +
                    nameof(channelFeaturesWithMessageIds) +
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

            channelFeaturesWithMessageIds.Add(
                leagueNameString, leagueRegistrationChannelMessageId);

            Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE); */
        }
    }
}