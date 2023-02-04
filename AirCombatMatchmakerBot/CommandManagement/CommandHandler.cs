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

        PrepareCommands();

        // Listens for command usage
        client.SlashCommandExecuted += SlashCommandHandler;

        return Task.CompletedTask;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand _command)
    {
        Log.WriteLine("Start of SlashCommandHandler", LogLevel.VERBOSE);

        string? firstOptionString = null;

        Log.WriteLine("OptionsCount: " + _command.Data.Options.Count, LogLevel.VERBOSE);

        if (_command.Data.Options.Count > 0)
        {
            var firstOption = _command.Data.Options.FirstOrDefault();
            if(firstOption == null)
            {
                Log.WriteLine(nameof(firstOption) + " was null! ", LogLevel.ERROR);
                return;
            }

            firstOptionString = firstOption.Value.ToString();

            Log.WriteLine("The command " + _command.Data.Name + " had " + _command.Data.Options.Count + " options in it." +
    "            The first command had an argument: " + firstOptionString, LogLevel.DEBUG);

            // Add a for loop here to print the command arguments, if multiple later on.
        }
        else
        {
            Log.WriteLine("The command " + _command.Data.Name + " does not have any options in it.", LogLevel.DEBUG);
        }

        string response = "";
        /*
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
                    if (firstOptionString != null)
                    {
                        Log.WriteLine("firstOptionValue is not null", LogLevel.VERBOSE);
                        // Registers the player profile and returns a bool as task if if was succesful,
                        // otherwise inform the user the user that he tried to register in to the database was already in it

                        if (Database.Instance.PlayerData.AddNewPlayerToTheDatabaseById(UInt64.Parse(firstOptionString)).Result)
                        {
                            response = "Added: " + firstOptionString + " to the database.";
                        }
                        // Did find the user from the database
                        else
                        {
                            response = "Player id: " + firstOptionString + " is already present in the database!";
                        }
                        Log.WriteLine("register response: " + response, LogLevel.VERBOSE);
                    }
                    else
                    {
                        response = nameof(firstOptionString) + " was null!";
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
                    if (firstOptionString != null)
                    {
                        // Deletes the player profile and returns a bool as task if if was succesful,
                        // otherwise inform the user that it didn't exist in the database
                        if (Database.Instance.PlayerData.DeletePlayerProfile(firstOptionString).Result)
                        {
                            response = "Deleted: " + firstOptionString + " from the database.";
                        }
                        // Didn't find it from the DB
                        else
                        {
                            response = "Unknown player id: " + firstOptionString + " could not find it from the database.";
                        }
                    }
                    else
                    {
                        response = nameof(firstOptionString) + " was null!";
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
       


        Log.WriteLine("FINAL RESPONSE: " + response, logLevel); */

        InterfaceCommand interfaceCommand = GetCommandInstance(_command.CommandName.ToUpper().ToString());

        if (firstOptionString == null)
        {
            Log.WriteLine("firstOptionString was null! ", LogLevel.ERROR);
            return;
        }

        response = await interfaceCommand.ReceiveCommandAndCheckForAdminRights(_command, firstOptionString);

        await SerializationManager.SerializeDB();

        await _command.RespondAsync(BotMessaging.GetMessageResponse(
            _command.Data.Name, response, _command.Channel.Name), ephemeral: true);
        
        Log.WriteLine("Sending and responding to the message done.", LogLevel.VERBOSE);
    }

    public static async Task PrepareCommands()
    {
        Log.WriteLine("Starting to prepare the commands.", LogLevel.VERBOSE);

        var commandEnumValues = Enum.GetValues(typeof(CommandName));
        Log.WriteLine(nameof(commandEnumValues) +
            " length: " + commandEnumValues.Length, LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        foreach (CommandName commandName in commandEnumValues)
        {
            Log.WriteLine("Looping on cmd" + nameof(commandName), LogLevel.VERBOSE);

            InterfaceCommand interfaceCommand = GetCommandInstance(commandName.ToString());
            Log.WriteLine("after getting command interface", LogLevel.VERBOSE);
            if (interfaceCommand == null)
            {
                Log.WriteLine(nameof(interfaceCommand).ToString() +
                    " was null!", LogLevel.CRITICAL);
                return;
            }

            // For commands without option, need to implement it with null check
            await interfaceCommand.AddNewCommandWithOption(client);
        }

        /*
        // Command for showing a test gif
        AddNewCommand("cats", "Prints a cute cat!");

        AddNewCommandWithOption("register",
            "registers an user profile manually",
            "userid",
            "what discord ID do you want to register?"
            );

        // Command for eliminating a player's profile
        AddNewCommandWithOption("terminate",
            "deletes a player profile, completely",
            "userid",
            "which user do you want to terminate?"
            );
        */
        Log.WriteLine("Done preparing the commands.", LogLevel.VERBOSE);

        return;
    }

    public static InterfaceCommand GetCommandInstance(string _commandName)
    {
        return (InterfaceCommand)EnumExtensions.GetInstance(_commandName.ToString());
    }
}