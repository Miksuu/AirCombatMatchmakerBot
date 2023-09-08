using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Concurrent;

[DataContract]
public class LEAGUEREGISTRATION : BaseChannel
{
    public LEAGUEREGISTRATION()
    {
        thisInterfaceChannel.ChannelType = ChannelType.LEAGUEREGISTRATION;

        thisInterfaceChannel.ChannelMessages = new ConcurrentDictionary<MessageName, bool>(
            new ConcurrentBag<KeyValuePair<MessageName, bool>>()
            {
                new KeyValuePair<MessageName, bool>(MessageName.LEAGUEREGISTRATIONMESSAGE, false),
            });
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        var guild = BotReference.GetGuildRef();

        return new List<Overwrite>
        {
            new Overwrite(
                guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny)),
            new Overwrite(RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                "Member").Result.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Allow)),
        };
    }

    public async override Task<bool> HandleChannelSpecificGenerationBehaviour()
    {
        Log.WriteLine("Starting to to prepare channel messages on " + thisInterfaceChannel.ChannelType +
            " count: " + Enum.GetValues(typeof(CategoryType)).Length);

        foreach (LeagueName leagueName in Enum.GetValues(typeof(LeagueName)))
        {
            Log.WriteLine("Looping on to find leagueName: " + leagueName.ToString());

            string leagueNameString = EnumExtensions.GetEnumMemberAttrValue(leagueName);
            Log.WriteLine("leagueNameString after enumValueCheck: " + leagueNameString);
            if (leagueNameString == null)
            {
                Log.WriteLine(nameof(leagueNameString) + " was null!", LogLevel.ERROR);
                continue;
            }

            try
            {
                //var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
                var leagueInterfaceFromDatabase =
                    ApplicationDatabase.Instance.Leagues.GetILeagueByCategoryName(leagueName);

                Log.WriteLine("Starting to create a league join button for: " + leagueNameString);

                Log.WriteLine(nameof(leagueInterfaceFromDatabase) + " before creating leagueButtonRegisterationCustomId: "
                    + leagueInterfaceFromDatabase.ToString());

                if (leagueInterfaceFromDatabase.LeagueRegistrationMessageId != 0) continue;

                InterfaceMessage interfaceMessage =
                    (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString());

                var newInterfaceMessage = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
                        thisInterfaceChannel, true, true, leagueInterfaceFromDatabase.LeagueCategoryId);

                leagueInterfaceFromDatabase.LeagueRegistrationMessageId = interfaceMessage.MessageId;

                thisInterfaceChannel.InterfaceMessagesWithIds.TryAdd(
                    leagueInterfaceFromDatabase.LeagueCategoryId,
                        (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString()));

                Log.WriteLine("Added to the ConcurrentDictionary, count is now: " +
                    thisInterfaceChannel.InterfaceMessagesWithIds.Count);

                Log.WriteLine("Done looping on: " + leagueNameString);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }

        return true;
    }
}