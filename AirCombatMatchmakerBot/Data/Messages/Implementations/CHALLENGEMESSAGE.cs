using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class CHALLENGEMESSAGE : BaseMessage
{
    public CHALLENGEMESSAGE()
    {
        messageName = MessageName.CHALLENGEMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int> 
        {
            { ButtonName.CHALLENGEBUTTON, 1 }
        };
        message = "Insert the challenge message here";
    }

    public override string GenerateMessage(ulong _channelId, ulong _channelCategoryId)
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " +
            _channelId, LogLevel.VERBOSE);

        foreach (var createdCategoriesKvp in
            Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("On league: " +
                createdCategoriesKvp.Value.CategoryType, LogLevel.VERBOSE);

            string leagueName =
                EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryType);

            Log.WriteLine("Full league name: " + leagueName, LogLevel.VERBOSE);

            if (createdCategoriesKvp.Value.InterfaceChannels.Any(
                    x => x.Value.ChannelId == _channelId))
            {
                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.First(
                        x => x.Value.ChannelId == _channelId).Value.ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor, LogLevel.VERBOSE);

                if (_channelId == channelIdToLookFor)
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