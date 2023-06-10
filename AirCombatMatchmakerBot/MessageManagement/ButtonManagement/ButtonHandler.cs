using Discord.WebSocket;
using System.Diagnostics;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        try
        {
            Log.WriteLine("Button press detected by: " + _component.User.Id, LogLevel.VERBOSE);

            ulong componentChannelId = _component.Channel.Id;
            ulong componentMessageId = _component.Message.Id;

            InterfaceMessage interfaceMessage = Database.Instance.Categories.FindInterfaceMessageWithComponentChannelIdAndMessageId(
            componentChannelId, componentMessageId);

            Log.WriteLine("Found: " + interfaceMessage.MessageChannelId + " | " +
                interfaceMessage.MessageId + " | " + interfaceMessage.MessageDescription, LogLevel.DEBUG);

            if (interfaceMessage.MessageCategoryId == 0 || interfaceMessage.MessageChannelId == 0 ||
                interfaceMessage.MessageId == 0 || interfaceMessage.MessageDescription == "")
            {
                Log.WriteLine("Channel id, msg or it's id was null!", LogLevel.ERROR);
            }

            InterfaceButton databaseButton = FindInterfaceButtonFromTheDatabase(
                _component, interfaceMessage.MessageCategoryId);

            var response = databaseButton.ActivateButtonFunction(
                _component, interfaceMessage).Result;

            // Only serialize when the interaction was something that needs to be serialized (defined in ActivateButtonFunction())
            if (response.serialize)
            {
                await SerializationManager.SerializeDB();
            }

            Log.WriteLine(response.responseString + " | " + response.serialize, LogLevel.DEBUG);

            await _component.RespondAsync(response.responseString, ephemeral: databaseButton.EphemeralResponse);
            //else { Log.WriteLine("the response was: " + responseTuple.Item1, LogLevel.CRITICAL); }
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }

    private static InterfaceButton FindInterfaceButtonFromTheDatabase(
        SocketMessageComponent _component, ulong _categoryId)
    {
        // Find the category by id
        var databaseCategory = Database.Instance.Categories.CreatedCategoriesWithChannels.FirstOrDefault(
            c => c.Key == _categoryId);
        if (databaseCategory.Value == null)
        {
            Log.WriteLine(nameof(databaseCategory.Value) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(databaseCategory.Value) + " was null!");
        }

        Log.WriteLine("Found category: " + databaseCategory.Value.CategoryType, LogLevel.VERBOSE);

        // Find the channel by id
        var databaseChannel = databaseCategory.Value.InterfaceChannels.FirstOrDefault(
            c => c.Value.ChannelId == _component.Channel.Id);
        if (databaseChannel.Value == null)
        {
            Log.WriteLine(nameof(databaseChannel) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(databaseChannel) + " was null!");
        }

        Log.WriteLine("Found channel: " + databaseChannel.Value.ChannelType, LogLevel.VERBOSE);

        // Find the database MessageDescription
        var databaseMessage = databaseChannel.Value.InterfaceMessagesWithIds.FirstOrDefault(
            m => m.Value.MessageId == _component.Message.Id);
        if (databaseMessage.Value == null)
        {
            Log.WriteLine(nameof(databaseMessage) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(databaseMessage) + " was null!");
        }

        Log.WriteLine("Found channel: " + databaseMessage.Value.MessageName, LogLevel.VERBOSE);

        // Find multiple buttons where the button name is the one being looked for
        InterfaceButton? foundButton = databaseMessage.Value.ButtonsInTheMessage.FirstOrDefault(
            b => b.ButtonCustomId == _component.Data.CustomId);
        if (foundButton == null)
        {
            Log.WriteLine(nameof(foundButton) + " was null", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(foundButton) + " was null!");
        }

        Log.WriteLine("Found the specific button: " + foundButton.ButtonName +
            " with label: " + foundButton.ButtonLabel, LogLevel.DEBUG);

        return foundButton;
    }
}