using Discord;

public class CommandBuilder
{
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