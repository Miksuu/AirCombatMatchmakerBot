using Discord.WebSocket;
using System.Diagnostics;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        string response = "EMPTY";

        Log.WriteLine("Button press detected by: " + _component.User.Id, LogLevel.VERBOSE);

        InterfaceMessage? interfaceMessage = null;

        foreach (var interfaceCategoryKvp in Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            if (interfaceCategoryKvp.Value.InterfaceChannels.Any(
                x => x.Value.ChannelId == _component.Channel.Id))
            {
                var interfaceChannelTemp =
                    interfaceCategoryKvp.Value.InterfaceChannels.FirstOrDefault(
                        x => x.Value.ChannelId == _component.Channel.Id);

                if (!interfaceChannelTemp.Value.InterfaceMessagesWithIds.Any(
                    x => x.Value.MessageId == _component.Message.Id))
                {
                    Log.WriteLine("message not found! with " + _component.Message.Id, LogLevel.ERROR);
                    continue;
                }

                var interfaceMessageKvp =
                    interfaceChannelTemp.Value.InterfaceMessagesWithIds.FirstOrDefault(
                        x => x.Value.MessageId == _component.Message.Id);

                interfaceMessage = interfaceMessageKvp.Value;
            }
        }

        if (interfaceMessage == null)
        {
            Log.WriteLine(nameof(interfaceMessage) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Found: " + interfaceMessage.MessageChannelId + " | " +
            interfaceMessage.MessageId + " | " + interfaceMessage.Message, LogLevel.DEBUG);

        if (interfaceMessage.MessageCategoryId == 0 || interfaceMessage.MessageChannelId == 0 ||
            interfaceMessage.MessageId == 0 || interfaceMessage.Message == "")
        {
            Log.WriteLine("Channel id, msg or it's id was null!", LogLevel.ERROR);
        }

        InterfaceButton? databaseButton = FindInterfaceButtonFromTheDatabase(
            _component, interfaceMessage.MessageCategoryId);

        if (databaseButton == null)
        {
            Log.WriteLine(nameof(databaseButton) + " was null", LogLevel.CRITICAL);
            return;
        }

        response = databaseButton.ActivateButtonFunction(
            _component, interfaceMessage).Result;

        await SerializationManager.SerializeDB();

        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }

    private static InterfaceButton? FindInterfaceButtonFromTheDatabase(
        SocketMessageComponent _component, ulong _categoryId)
    {
        // Find the category by id
        var databaseCategory = Database.Instance.Categories.CreatedCategoriesWithChannels.FirstOrDefault(
            c => c.Key == _categoryId);
        if (databaseCategory.Value == null)
        {
            Log.WriteLine(nameof(databaseCategory.Value) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found category: " + databaseCategory.Value.CategoryType, LogLevel.VERBOSE);

        // Find the channel by id
        var databaseChannel = databaseCategory.Value.InterfaceChannels.FirstOrDefault(
            c => c.Value.ChannelId == _component.Channel.Id);
        if (databaseChannel.Value == null)
        {
            Log.WriteLine(nameof(databaseChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found channel: " + databaseChannel.Value.ChannelType, LogLevel.VERBOSE);

        // Find the database message
        var databaseMessage = databaseChannel.Value.InterfaceMessagesWithIds.FirstOrDefault(
            m => m.Value.MessageId == _component.Message.Id);
        if (databaseMessage.Value == null)
        {
            Log.WriteLine(nameof(databaseMessage) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found channel: " + databaseMessage.Value.MessageName, LogLevel.VERBOSE);

        // Find multiple buttons where the button name is the one being looked for
        InterfaceButton? foundButton = databaseMessage.Value.ButtonsInTheMessage.FirstOrDefault(
            b => b.ButtonCustomId == _component.Data.CustomId);
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