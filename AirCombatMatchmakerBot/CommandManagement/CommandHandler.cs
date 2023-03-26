﻿using Discord.WebSocket;

public static class CommandHandler
{
    // Installs the commands that are predefined in the code itself
    public async static Task InstallCommandsAsync()
    {
        Log.WriteLine("Starting to install the commands.", LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();

        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        await PrepareCommands();

        // ConcurrentBagens for command usage
        client.SlashCommandExecuted += SlashCommandHandler;

        return;
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