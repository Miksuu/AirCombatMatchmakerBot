using Discord;

public class CommandBuilder
{
    public static Task PrepareCommands()
    {
        Log.WriteLine("Starting to prepare the commands.", LogLevel.VERBOSE);

        var commandEnumValues = Enum.GetValues(typeof(CommandName));
        Log.WriteLine(nameof(commandEnumValues) +
            " length: " + commandEnumValues.Length, LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return Task.CompletedTask;
        }

        foreach (CommandName commandName in commandEnumValues)
        {
            Log.WriteLine("Looping on cmd" + nameof(commandName), LogLevel.VERBOSE);

            InterfaceCommand interfaceCommand = GetCommandInstance(commandName);
            Log.WriteLine("after getting command interface", LogLevel.VERBOSE);
            if (interfaceCommand == null)
            {
                Log.WriteLine(nameof(interfaceCommand).ToString() +
                    " was null!", LogLevel.CRITICAL);
                return Task.CompletedTask;
            }

            // For commands without option, need to implement it with null check
            interfaceCommand.AddNewCommandWithOption(client);
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

        return Task.CompletedTask;
    }





    public static InterfaceCommand GetCommandInstance(CommandName _commandName)
    {
        return (InterfaceCommand)EnumExtensions.GetInstance(_commandName.ToString());
    }
}