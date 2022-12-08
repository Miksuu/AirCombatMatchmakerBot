using Discord;

public class CommandBuilder
{
    public static Task PrepareCommands()
    {
        Log.WriteLine("Starting to prepare the commands.", LogLevel.VERBOSE);

        // TO DO: Move these in the some CommandLibrary that loads from JSON?

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

        Log.WriteLine("Done preparing the commands.", LogLevel.VERBOSE);

        return Task.CompletedTask;
    }


    // Adds a new command based on parameters
    // TO DO: add support for options such as adding user, for operations such as challenging people 
    public static Discord.SlashCommandBuilder AddNewCommand(string _commandName, string _description, bool _optionIncluded = false)
    {
        var guildCommand = new Discord.SlashCommandBuilder()
            .WithName(_commandName)
            .WithDescription(_description);

        if (!_optionIncluded) 
        {
            Log.WriteLine("Installing a command: " + _commandName + ", with description: " + _description, LogLevel.DEBUG);

            if (BotReference.clientRef != null)
            {
                BotReference.clientRef.Rest.CreateGuildCommand(guildCommand.Build(), BotReference.GuildID);
            }
            else
            {
                Exceptions.BotClientRefNull();
            }
        }

        return guildCommand;
    }

    public static async void AddNewCommandWithOption(string _commandName, string _description, string _optionName, string _optionDescription)
    {
        var guildCommandWithOptions = AddNewCommand(_commandName, _description, true).AddOption(
            _optionName, ApplicationCommandOptionType.String,
            _optionDescription, isRequired: true);

        Log.WriteLine("Installing a command: " + _commandName + " | with description: " + _description + 
            " | that has an option with name: " + _optionName + " | and optionDescription: " + _optionDescription, LogLevel.DEBUG);

        if (BotReference.clientRef != null)
        {
            await BotReference.clientRef.Rest.CreateGuildCommand(guildCommandWithOptions.Build(), BotReference.GuildID);
        }
        else
        {
            Exceptions.BotClientRefNull();
        }
    }
}