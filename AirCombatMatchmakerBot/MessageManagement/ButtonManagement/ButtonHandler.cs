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
        response = interfaceButton.ActivateButtonFunction(_component);

        /*
        // Checks with first element of the split string (action)
        switch (splitString[0])
        {
            case "mainRegistration":
                response = await ButtonFunctionality.MainRegistration(_component);
                break;
            case "leagueRegistration":
                await ButtonFunctionality.LeagueRegistration(_component, splitString[1]);
                break;
            case "challenge":
                await ButtonFunctionality.PostChallenge(_component, splitString[1]);
                break;
            default:
                response = "Something went wrong with the button press!";
                logLevel = LogLevel.ERROR;
                break;
        }

        Log.WriteLine("Before serialization on ButtonHandler", LogLevel.VERBOSE);
        */
        await SerializationManager.SerializeDB();

        if (splitString[0] != "leagueRegistration")
        {
            Log.WriteLine(response, logLevel);
        }
        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }
}