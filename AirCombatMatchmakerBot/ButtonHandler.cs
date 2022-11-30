using Discord;
using Discord.WebSocket;
using System.ComponentModel;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        // Splits the button press action and the user ID
        string[] splitString = _component.Data.CustomId.Split('_');

        Log.WriteLine("Button press detected: " + splitString[0] + " | " + splitString[1], LogLevel.DEBUG);

        // Checks with first element of the split string (action)
        switch (splitString[0])
        {
            // Player registeration, 2nd part of split string is hes ID
            case "registeration":



                await _component.RespondAsync(_component.User.Mention + "," +
                    BotMessaging.GetMessageResponse(_component.Data.CustomId,
                    " registeration complete, welcome! \n" +
                    "This channel will close soon.", 
                    _component.Channel.Name));
                break;
            default:
                Log.WriteLine("Something went wrong with the button press!", LogLevel.CRITICAL);
                break;
        }
    }
}