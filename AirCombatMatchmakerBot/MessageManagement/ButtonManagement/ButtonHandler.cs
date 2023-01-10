using Discord.WebSocket;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        string response = "EMPTY";

        Log.WriteLine("Button press detected by: " + _component.User.Id, LogLevel.VERBOSE);

        //LogLevel logLevel = LogLevel.DEBUG;

        // Maybe get this of this and make a method getting the button's message object
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

        InterfaceButton? databaseButton = FindInterfaceButtonFromTheDatabase(_component, categoryId);

        if (databaseButton == null)
        {
            Log.WriteLine(nameof(databaseButton) + " was null", LogLevel.CRITICAL);
            return;
        }

        response = databaseButton.ActivateButtonFunction(
            _component, channelId, messageId, message, splitStrings).Result;

        await SerializationManager.SerializeDB();

        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }

    private static InterfaceButton? FindInterfaceButtonFromTheDatabase(
        SocketMessageComponent _component, string[] _splitStrings, ulong _categoryId)
    {
        //ulong categoryIdToLookFor = ulong.Parse(_splitStrings[0]);
        //ulong channelIdToLookFor = ulong.Parse(_splitStrings[1]);
        //ulong messageIdToLookfor = ulong.Parse(_splitStrings[2]);
        //string buttonNameToLookFor = _splitStrings[0];
        //int buttonIndexToLookFor = int.Parse(_splitStrings[1]);

        // Find the category by id
        var databaseCategory = Database.Instance.Categories.CreatedCategoriesWithChannels.First(
            c => c.Key == _categoryId);
        if (databaseCategory.Value == null)
        {
            Log.WriteLine(nameof(databaseCategory.Value) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found category: " + databaseCategory.Value.CategoryType, LogLevel.VERBOSE);

        // Find the channel by id
        var databaseChannel = databaseCategory.Value.InterfaceChannels.First(
            c => c.Value.ChannelId == _component.Channel.Id);
        if (databaseChannel.Value == null)
        {
            Log.WriteLine(nameof(databaseChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found channel: " + databaseChannel.Value.ChannelType, LogLevel.VERBOSE);

        // Find the database message
        var databaseMessage = databaseChannel.Value.InterfaceMessagesWithIds.First(
            m => m.Value.MessageId == _component.Message.Id);
        if (databaseMessage.Value == null)
        {
            Log.WriteLine(nameof(databaseMessage) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found channel: " + databaseMessage.Value.MessageName, LogLevel.VERBOSE);

        // Find multiple buttons where the button name is the one being looked for
        var databaseButtons = databaseMessage.Value.ButtonsInTheMessage.First(
            b => b.ToString() == _splitStrings[0]).ToList();
        if (databaseButtons == null || databaseButtons.Count == 0)
        {
            Log.WriteLine(nameof(databaseButtons) + " was null, or count was 0", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found buttons count: " + databaseButtons.Count, LogLevel.VERBOSE);

        InterfaceButton foundButton = databaseButtons.First(
            b => b.ButtonLabel == _splitStrings[1]);

        if (foundButton == null)
        {
            Log.WriteLine(nameof(foundButton) + " was null", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found the specific button: " + foundButton.ButtonName +
            " with label: " + foundButton.ButtonLabel, LogLevel.DEBUG);

        return foundButton;
    }
}