using Discord;
using Discord.WebSocket;
using System.ComponentModel;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        // Splits the button press action and the user ID
        string[] splitString = _component.Data.CustomId.Split('_');

        Log.WriteLine("Button press detected by: " + _component.User.Id + " | splitStrings: " +
            splitString[0] + " | " + splitString[1], LogLevel.DEBUG);

        // Checks with first element of the split string (action)
        switch (splitString[0])
        {
            // Player registeration, 2nd part of split string is hes ID
            case "registeration":
                // Check that the button is the user's one
                if (_component.User.Id.ToString() == splitString[1])
                {
                    await PlayerManager.AddNewPlayer(_component);

                    await _component.RespondAsync(
                        _component.User.Mention + ", " +
                        BotMessaging.GetMessageResponse(
                            _component.Data.CustomId,
                            " registeration complete, welcome! \n" +
                            "This channel will close soon.",
                            _component.Channel.Name));
                }
                else
                {
                    await InformThatButtonDoesntBelongToAUser(_component);
                }
                break;
            default:
                Log.WriteLine("Something went wrong with the button press!", LogLevel.CRITICAL);
                break;
        }
    }

    private static async Task InformThatButtonDoesntBelongToAUser(SocketMessageComponent _component)
    {
        await _component.RespondAsync(
            _component.User.Mention + ", " +
            BotMessaging.GetMessageResponse(
                _component.Data.CustomId,
                " that's not your button!",
                _component.Channel.Name));
    }
}