using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;

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

        InterfaceButton interfaceButton = (InterfaceButton)EnumExtensions.GetInstance(splitString[0]);
        response = interfaceButton.ActivateButtonFunction(_component, splitString[1]).Result;

        await SerializationManager.SerializeDB();

        if (splitString[0] != "leagueRegistration")
        {
            Log.WriteLine(response, logLevel);
        }
        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }
}