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
        channelType = ChannelType.LEAGUEREGISTRATION;
        channelMessages = new List<MessageName> { MessageName.LEAGUEREGISTRATIONMESSAGE };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
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
        Log.WriteLine("Starting to to prepare channel messages on " + channelType, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Add to a method later
        var databaseInterfaceChannel =
            Database.Instance.Categories.CreatedCategoriesWithChannels.First(
                x => x.Key == channelsCategoryId).Value.InterfaceChannels.First(
                    x => x.ChannelId == channelId);

        Log.WriteLine("After db find", LogLevel.VERBOSE);

        foreach (CategoryType leagueName in Enum.GetValues(typeof(CategoryType)))
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

            var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
            if (leagueInterface == null)
            {
                Log.WriteLine("leagueInterface was null!", LogLevel.CRITICAL);
                return;
            }

            var leagueInterfaceFromDatabase =
                Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);


            Log.WriteLine("Starting to create a league join button for: " + leagueNameString, LogLevel.VERBOSE);

            if (leagueInterfaceFromDatabase == null)
            {
                Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine(nameof(leagueInterfaceFromDatabase) + " before creating leagueButtonRegisterationCustomId: "
                + leagueInterfaceFromDatabase.ToString(), LogLevel.VERBOSE);


            InterfaceMessage interfaceMessage =
                (InterfaceMessage)EnumExtensions.GetInstance(channelMessages.ElementAt(0).ToString());
            Log.WriteLine("Created interfaceMessage instance: " +
                interfaceMessage.MessageName, LogLevel.VERBOSE);

            interfaceMessage.Message = leagueInterfaceFromDatabase.GenerateALeagueJoinButtonMessage();

            Log.WriteLine("interfaceMessage message: " + interfaceMessage.Message, LogLevel.VERBOSE);

            if (databaseInterfaceChannel.InterfaceMessagesWithIds.ContainsKey(leagueName.ToString())) continue;

            databaseInterfaceChannel.InterfaceMessagesWithIds.Add(leagueName.ToString(), interfaceMessage);

            Log.WriteLine("Added to the dictionary, count is now: " +
                databaseInterfaceChannel.InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);

            Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);
        }

        Log.WriteLine("Before entering PostChannelMessages", LogLevel.VERBOSE);
        await base.PostChannelMessages(guild, databaseInterfaceChannel);
    }
}