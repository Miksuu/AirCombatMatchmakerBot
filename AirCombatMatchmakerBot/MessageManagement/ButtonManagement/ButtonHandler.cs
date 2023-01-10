using Discord.WebSocket;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        Log.WriteLine("Button press detected by: " + _component.User.Id, LogLevel.VERBOSE);

        // Splits the button press action and the user ID
        string[] splitStrings = _component.Data.CustomId.Split('_');

        Log.WriteLine("splitStrings: " +
            splitStrings[0] + " | " + splitStrings[1] + " | " + splitStrings[2], LogLevel.DEBUG);

        string response = "EMPTY";
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



        InterfaceButton interfaceButton = (InterfaceButton)EnumExtensions.GetInstance(splitStrings[0]);
        response = interfaceButton.ActivateButtonFunction(
            _component, channelId, messageId, message, splitStrings).Result;

        await SerializationManager.SerializeDB();

        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }

    private static void FindInterfaceButtonFromTheDatabase(string _customId)
    {
        string[] splitStrings = _customId.Split("_");

        foreach (var item in splitStrings)
        {
            Log.WriteLine(item, LogLevel.VERBOSE);
        }

        //ulong categoryIdToLookFor = ulong.Parse(splitStrings[0]);
        ulong channelIdToLookFor = ulong.Parse(splitStrings[1]);
        ulong messageIdToLookfor = ulong.Parse(splitStrings[2]);
        string buttonNameToLookFor = splitStrings[3];
        int buttonIndexToLookFor = int.Parse(splitStrings[4]);

        // Find the categoryId
        var databaseCategory = Database.Instance.Categories.CreatedCategoriesWithChannels.First(
            c => c.Key == ulong.Parse(splitStrings[0]));
        if (databaseCategory.Value == null)
        {
            Log.WriteLine(nameof(databaseCategory.Value) + " was null!", LogLevel.CRITICAL);
            return;
        }

        // Find the categoryId
        var databaseChannelId = databaseCategory.Value.InterfaceChannels.First(
            c => c.Value.ChannelId == ulong.Parse(splitStrings[1]));
        if (databaseChannelId.Value == null)
        {
            Log.WriteLine(nameof(databaseChannelId) + " was null!", LogLevel.CRITICAL);
            return;
        }

    }
}