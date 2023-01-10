using Discord.WebSocket;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        string response = "EMPTY";

        Log.WriteLine("Button press detected by: " + _component.User.Id, LogLevel.VERBOSE);

        // Splits the button press action and the user ID
        string[] splitStrings = _component.Data.CustomId.Split("_");

        foreach (var item in splitStrings)
        {
            Log.WriteLine(item, LogLevel.VERBOSE);
        }

        /*
        LogLevel logLevel = LogLevel.DEBUG;

        ulong categoryId = 0;
        ulong channelId = 0;
        ulong messageId = 0;
        string message = "";


        foreach (var interfaceCategoryKvp in Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            if (interfaceCategoryKvp.Value.InterfaceChannels.Any(
                x => x.Value.ChannelId == _component.Channel.Id))
            {
                var interfaceChannelTemp =
                    interfaceCategoryKvp.Value.InterfaceChannels.First(
                        x => x.Value.ChannelId == _component.Channel.Id);

                if (!interfaceChannelTemp.Value.InterfaceMessagesWithIds.Any(
                    x => x.Value.MessageId == _component.Message.Id))
                {
                    Log.WriteLine("message not found! with " + _component.Message.Id, LogLevel.ERROR);
                    continue;
                }

                var interfaceMessageKvp =
                    interfaceChannelTemp.Value.InterfaceMessagesWithIds.First(
                        x => x.Value.MessageId == _component.Message.Id);

                categoryId = interfaceCategoryKvp.Key;
                channelId = interfaceChannelTemp.Value.ChannelId;
                messageId = interfaceMessageKvp.Value.MessageId;
                message = interfaceMessageKvp.Value.Message;
            }
        }

        Log.WriteLine("Found: " + channelId + " | " + messageId + " | " + message, LogLevel.DEBUG);

        if (categoryId == 0 || channelId == 0 || messageId == 0 || message == "")
        {
            Log.WriteLine("Channel id, msg or it's id was null!", LogLevel.ERROR);
        }

        */

        InterfaceButton interfaceButton = (InterfaceButton)EnumExtensions.GetInstance(splitStrings[0]);
        response = interfaceButton.ActivateButtonFunction(
            _component, channelId, messageId, message, splitStrings).Result;

        await SerializationManager.SerializeDB();

        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }

    private static InterfaceButton? FindInterfaceButtonFromTheDatabase(string[] _splitStrings)
    {
        //ulong categoryIdToLookFor = ulong.Parse(_splitStrings[0]);
        //ulong channelIdToLookFor = ulong.Parse(_splitStrings[1]);
        //ulong messageIdToLookfor = ulong.Parse(_splitStrings[2]);
        string buttonNameToLookFor = _splitStrings[2];
        int buttonIndexToLookFor = int.Parse(_splitStrings[3]);

        // Find the categoryId
        var databaseCategory = Database.Instance.Categories.CreatedCategoriesWithChannels.First(
            c => c.Key == ulong.Parse(_splitStrings[0]));
        if (databaseCategory.Value == null)
        {
            Log.WriteLine(nameof(databaseCategory.Value) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        // Find the categoryId
        var databaseChannel = databaseCategory.Value.InterfaceChannels.First(
            c => c.Value.ChannelId == ulong.Parse(_splitStrings[1]));
        if (databaseChannel.Value == null)
        {
            Log.WriteLine(nameof(databaseChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

    }
}