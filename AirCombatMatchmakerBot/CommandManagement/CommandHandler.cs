using Discord.WebSocket;

public static class CommandHandler
{
    // Installs the commands that are predefined in the code itself
    public static Task InstallCommandsAsync()
    {
        Log.WriteLine("Starting to install the commands.", LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();

        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return Task.CompletedTask;
        }

        client.Ready += CommandBuilder.PrepareCommands;

        // Listens for command usage
        client.SlashCommandExecuted += SlashCommandHandler;

        return Task.CompletedTask;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand _command)
    {
        Log.WriteLine("Start of SlashCommandHandler", LogLevel.VERBOSE);

        string? firstOptionValue = null;

        Log.WriteLine("OptionsCount: " + _command.Data.Options.Count, LogLevel.VERBOSE);

        if (_command.Data.Options.Count > 0)
        {
            firstOptionValue = _command.Data.Options.First().Value.ToString();

            Log.WriteLine("The command " + _command.Data.Name + " had " + _command.Data.Options.Count + " options in it." +
    "            The first command had an argument: " + firstOptionValue, LogLevel.DEBUG);

            // Add a for loop here to print the command arguments, if multiple later on.
        }
        else
        {
            Log.WriteLine("The command " + _command.Data.Name + " does not have any options in it.", LogLevel.DEBUG);
        }

        string response = "EMPTY REPONSE";
        LogLevel logLevel = LogLevel.DEBUG;
        switch (_command.Data.Name)
        {
            case "cat":
                response = "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687";
                break;
            // ADMIN COMMANDS
            case "register":
                Log.WriteLine("On register case. ", LogLevel.VERBOSE);
                // Check if the user is admin
                if (Database.Instance.Admins.CheckIfCommandSenderWasAnAdmin(_command))
                {
                    Log.WriteLine("Admin check pass", LogLevel.VERBOSE);
                    if (firstOptionValue != null)
                    {
                        Log.WriteLine("firstOptionValue is not null", LogLevel.VERBOSE);
                        // Registers the player profile and returns a bool as task if if was succesful,
                        // otherwise inform the user the user that he tried to register in to the database was already in it

                        if (UserManager.AddNewPlayerToTheDatabaseById(UInt64.Parse(firstOptionValue)).Result)
                        {
                            response = "Added: " + firstOptionValue + " to the database.";
                        }
                        // Did find the user from the database
                        else
                        {
                            response = "Player id: " + firstOptionValue + " is already present in the database!";
                        }
                        Log.WriteLine("register response: " + response, LogLevel.VERBOSE);
                    }
                    else
                    {
                        response = nameof(firstOptionValue) + " was null!";
                    }
                }
                // Inform the non-admin
                else
                {
                    response = "You are not an admin, this command is not for you!";
                }
                break;

            case "terminate":
                // Check if the user is admin
                if (Database.Instance.Admins.CheckIfCommandSenderWasAnAdmin(_command))
                {
                    if (firstOptionValue != null)
                    {
                        // Deletes the player profile and returns a bool as task if if was succesful,
                        // otherwise inform the user that it didn't exist in the database
                        if (Database.Instance.PlayerData.DeletePlayerProfile(firstOptionValue).Result)
                        {
                            response = "Deleted: " + firstOptionValue + " from the database.";
                        }
                        // Didn't find it from the DB
                        else
                        {
                            response = "Unknown player id: " + firstOptionValue + " could not find it from the database.";
                        }
                    }
                    else
                    {
                        response = nameof(firstOptionValue) + " was null!";
                    }
                }
                // Inform the non-admin
                else
                {
                    response = "You are not an admin, this command is not for you!";
                }
                break;
            default:
                response = "Default response! Something's wrong";
                logLevel = LogLevel.ERROR;
                break;
        }

        await SerializationManager.SerializeDB();

        Log.WriteLine("FINAL RESPONSE: " + response, logLevel);

        // Respond to the user based on the string result
        await _command.RespondAsync(BotMessaging.GetMessageResponse(
            _command.Data.Name, response, _command.Channel.Name)); ;

        Log.WriteLine("Sending and responding to the message done.", LogLevel.VERBOSE);
    }
}