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

        string response = "EMPTY";
        LogLevel logLevel = LogLevel.DEBUG;
        // Checks with first element of the split string (action)
        switch (splitString[0])
        {
            // Player registeration, 2nd part of split string is hes ID
            case "registeration":
                // Check that the button is the user's one
                if (_component.User.Id.ToString() == splitString[1])
                {
                    // Checks that the player does not exist in the database already, true if this is not the case
                    if (UserManager.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
                    {
                        await ChannelManager.DeleteUsersRegisterationChannel(_component.User.Id);
                        /*
                        response = _component.User.Mention + ", " +
                            BotMessaging.GetMessageResponse(
                                _component.Data.CustomId,
                                " registeration complete, welcome! \n" +
                                "This channel will close soon.",
                                _component.Channel.Name); */
                    }
                    // This should not be the case, the registeration channel should not be available for the user
                    // TO DO: Also remember to remove the button!!
                    else
                    {
                        response = _component.User.Mention + ", " +
                            BotMessaging.GetMessageResponse(
                                _component.Data.CustomId,
                                " You are already registered \n" +
                                "one of our admins has been informed about a possible bug in the program.",
                                _component.Channel.Name);

                        // Admin warning
                        logLevel = LogLevel.WARNING;
                    }
                }
                else
                {
                    response = _component.User.Mention + ", " +
                        BotMessaging.GetMessageResponse(
                            _component.Data.CustomId,
                            " that's not your button!",
                            _component.Channel.Name);
                }
                break;
            default:
                response = "Something went wrong with the button press!";
                logLevel = LogLevel.ERROR;
                break;
        }

        await SerializationManager.SerializeDB();

        Log.WriteLine(response, logLevel);
        if (response != "EMPTY") await _component.RespondAsync(response);
    }

    private static async Task InformThatButtonDoesntBelongToAUser(SocketMessageComponent _component)
    {
        await _component.RespondAsync(
           );
    }
}