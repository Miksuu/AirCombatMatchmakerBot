using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;
using System.Threading.Channels;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        Log.WriteLine(_component.Data.CustomId, LogLevel.DEBUG);

        // Splits the button press action and the user ID
        string[] splitString = _component.Data.CustomId.Split('_');

        Log.WriteLine("Button press detected by: " + _component.User.Id + " | splitStrings: " +
            splitString[0] + " | " + splitString[1], LogLevel.DEBUG);
        
        string response = "EMPTY";
        LogLevel logLevel = LogLevel.DEBUG;


        /*
        // Add to a method later
        var databaseInterfaceChannel =
            Database.Instance.Categories.CreatedCategoriesWithChannels.First(
                x => x.Key == _component).Value.InterfaceChannels.First(
                    x => x.ChannelId == channelId);
        */

        /*
        var databaseInterfaceChannel =
            Database.Instance.Categories.CreatedCategoriesWithChannels.First(
                x => x.Key == _component).Value.InterfaceChannels.First(
                    x => x.ChannelId == channelId);*/

        ulong channelId = 0;
        ulong messageId = 0;

        foreach (var interfaceCategoryKvp in Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            if (interfaceCategoryKvp.Value.InterfaceChannels.Any(x => x.ChannelId == _component.Channel.Id))
            {
                var interfaceChannelTemp = interfaceCategoryKvp.Value.InterfaceChannels.First(x => x.ChannelId == _component.Channel.Id);
                if (!interfaceChannelTemp.InterfaceMessagesWithIds.Any(x => x.Value.MessageId == _component.Message.Id))
                {
                    Log.WriteLine("message not found! with " + _component.Message.Id, LogLevel.ERROR);
                    continue;
                }

                var temp = interfaceChannelTemp.InterfaceMessagesWithIds.First(x => x.Value.MessageId == _component.Message.Id);

                channelId = interfaceChannelTemp.ChannelId;
                messageId = temp.Value.MessageId;
            }
        }

        Log.WriteLine("Found: " + channelId + " | " + messageId, LogLevel.VERBOSE);

        if (channelId == 0 || messageId == 0)
        {
            Log.WriteLine("Channel id or msg id was null!", LogLevel.ERROR);
        }

        InterfaceButton interfaceButton = (InterfaceButton)EnumExtensions.GetInstance(splitString[0]);
        response = interfaceButton.ActivateButtonFunction(_component, splitString[1], channelId, messageId).Result;

        await SerializationManager.SerializeDB();

        if (splitString[0] != "leagueRegistration")
        {
            Log.WriteLine(response, logLevel);
        }
        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }
}