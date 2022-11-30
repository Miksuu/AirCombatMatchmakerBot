using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord.Net;
using Discord;
using Newtonsoft.Json;

public class CommandBuilder
{
    // Adds a new command based on parameters
    // TO DO: add support for options such as adding user, for operations such as challenging people 
    public static async void AddNewCommand(string _commandName, string _description)
    {
        Log.WriteLine("Installing a command: " + _commandName + ", with description: " +_description, LogLevel.DEBUG);

        var guildCommand = new SlashCommandBuilder()
            .WithName(_commandName)
            .WithDescription(_description);
        //.AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);

        if (BotReference.clientRef != null)
        {
            await BotReference.clientRef.Rest.CreateGuildCommand(guildCommand.Build(), BotReference.GuildID);
        }
        else
        {
            Exceptions.BotClientRefNull();
        }
    }
}