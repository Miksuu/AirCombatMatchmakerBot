using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Reflection.Emit;

public abstract class BaseCommand : InterfaceCommand
{
    CommandName InterfaceCommand.CommandName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commandName) + ": " +
                commandName, LogLevel.VERBOSE);
            return commandName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commandName) + commandName
                + " to: " + value, LogLevel.VERBOSE);
            commandName = value;
        }
    }

    string InterfaceCommand.CommandDescription
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commandDescription) + ": " +
                commandOption, LogLevel.VERBOSE);
            return commandDescription;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commandOption) + commandOption
                + " to: " + value, LogLevel.VERBOSE);
            commandDescription = value;
        }
    }

    CommandOption? InterfaceCommand.CommandOption
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commandOption) + ": " +
                commandOption, LogLevel.VERBOSE);
            return commandOption;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commandOption) + commandOption
                + " to: " + value, LogLevel.VERBOSE);
            commandOption = value;
        }
    }

    protected CommandName commandName;
    protected string commandDescription = "";
    protected CommandOption? commandOption;

    public abstract Task ActivateCommandFunction();

    public async Task AddNewCommandWithOption(Discord.WebSocket.DiscordSocketClient _client)
    {
        if (commandOption == null)
        {
            Log.WriteLine(nameof(commandOption) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Installing a command: " + commandName.ToString() + " | with description: " +
            commandDescription + " | that has an option with name: " + commandOption.OptionName +
            " | and optionDescription: " + commandOption.OptionDescription, LogLevel.DEBUG);

        var guildCommand = new Discord.SlashCommandBuilder()
            .WithName(commandName.ToString().ToLower())
            .WithDescription(commandDescription).AddOption(
            commandOption.OptionName, ApplicationCommandOptionType.String,
            commandOption.OptionDescription, isRequired: true);

        await _client.Rest.CreateGuildCommand(
            guildCommand.Build(), BotReference.GetGuildID());

        Log.WriteLine("Done creating a command with option: " + guildCommand.Name, LogLevel.DEBUG);
    }
}