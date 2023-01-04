using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGE : BaseChannel
{
    public CHALLENGE()
    {
        channelType = ChannelType.CHALLENGE;
        channelMessages = new List<MessageName>
        {
            MessageName.CHALLENGEMESSAGE,
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    { 
        return new List<Overwrite>
        {
        };
    }

    public override async Task PrepareChannelMessages()
    {
        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var databaseInterfaceChannel =
            Database.Instance.Categories.CreatedCategoriesWithChannels.First(
                x => x.Key == channelsCategoryId).Value.InterfaceChannels.First(
                    x => x.ChannelId == channelId);

        InterfaceMessage interfaceMessage =
            (InterfaceMessage)EnumExtensions.GetInstance(channelMessages[0].ToString());

        if (!databaseInterfaceChannel.InterfaceMessagesWithIds.ContainsKey(
            channelMessages[0].ToString()))
        {
            Log.WriteLine("Does not contain the key: " +
                channelMessages[0] + ", continuing", LogLevel.VERBOSE);

            // Generate the initial message
            interfaceMessage.Message = GenerateChallengeQueueMessage();

            databaseInterfaceChannel.InterfaceMessagesWithIds.Add(
                channelMessages[0].ToString(), interfaceMessage);
        }

        await base.PostChannelMessages(guild, databaseInterfaceChannel);
    }

    public string GenerateChallengeQueueMessage()
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " +
            channelId, LogLevel.VERBOSE);

        foreach (var createdCategoriesKvp in
            Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("On league: " +
                createdCategoriesKvp.Value.CategoryType, LogLevel.VERBOSE);

            string leagueName =
                EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryType);

            Log.WriteLine("Full league name: " + leagueName, LogLevel.VERBOSE);

            if (createdCategoriesKvp.Value.InterfaceChannels.Any(
                    x => x.ChannelId == channelId))
            {
                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.First(
                        x => x.ChannelId == channelId).ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor, LogLevel.VERBOSE);

                if (channelId == channelIdToLookFor)
                {
                    Log.WriteLine("Found: " + channelIdToLookFor +
                        " is league: " + leagueName, LogLevel.DEBUG);

                    string challengeMessage = ". \n" +
                        leagueName + " challenge. Players In The Queue: \n";

                    return challengeMessage;
                }
            }
        }

        Log.WriteLine("Did not find a channel id to generate a challenge" +
            " queue message on!", LogLevel.ERROR);
        return string.Empty;
    }
}