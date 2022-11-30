using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord.Net;
using Discord;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

public static class CommandHandler
{
    //private static readonly CommandService commands;

    public static async Task InstallCommandsAsync()
    {
        BotReference.clientRef.Ready += PrepareCommands;
        BotReference.clientRef.SlashCommandExecuted += SlashCommandHandler;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand _command)
    {
        /*
        await _command.RespondAsync($"You executed {_command.Data.Name} command." +
            $" Here's a cat: https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687");
        */

        switch (_command.Data.Name)
        {
            case "cat":
                await _command.RespondAsync(BotMessaging.GetResponse(_command,
                    "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687"));
                break;
            default:
                await _command.RespondAsync(BotMessaging.GetResponse(_command, "Unknown command!", true));
                break;
        }

        Log.WriteLine("Sending message done", LogLevel.VERBOSE);
    }

    public static Task PrepareCommands()
    {
        //var guild = BotReference.clientRef.GetGuild(BotReference.GuildID);

        CommandBuilder.AddNewCommand("cat", "prints a cute cat");

        return Task.CompletedTask;
    }

    /*
    private static async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the _command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the _command begins
        int argPos = 0;

        // Determine if the message is a _command based on the prefix and make sure no bots trigger commandService
        if (!(message.HasCharPrefix('/', ref argPos) ||
            message.HasMentionPrefix(BotReference.clientRef.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based _command context based on the message
        var context = new SocketCommandContext(BotReference.clientRef, message);

        // Execute the _command with the _command context we just
        // created, along with the service provider for precondition checks.
        await BotReference.commandService.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: null);
    } */


    /*
    public static Task HandleCommand(SocketMessage _Message)
    {        
        ISocketMessageChannel MessageChannel = _Message.Channel;
        //ulong senderID = _Message.Author.Id;

        // IF YOU WANT TO DO ANY LOGGING ABOVE THIS LINE,
        // NEED TO MAKE A CIRCULAR DEPENDENCY FIX OR CONFIRM THAT THE MESSAGE IS NOT IN THE #log CHANNEL

        // Returns if the message doesn't start with the prefix, or the author is a bot
        if ((!_Message.Content.StartsWith('!') || _Message.Author.IsBot)) return Task.CompletedTask;

        //Log.WriteLine("messageTest: " + message + " | " + message.ToString(), LogLevel.DEBUG);

        Log.WriteLine("Message received from: " + _Message.Author.Id + " in: " + MessageChannel, LogLevel.VERBOSE);

        Log.WriteLine("Received message! in " + MessageChannel, LogLevel.VERBOSE);

        //if (senderID != ulongHere) return;

        // Split in to the parameters
        string[] cmdParameters = _Message.Content.Split(' ');

        // Just for logging
        foreach (var msgPart in cmdParameters)
        {
            Log.WriteLine("msgPart: " + msgPart, LogLevel.VERBOSE);
        }

        Log.WriteLine("msg content: " + _Message.Content.ToString(), LogLevel.VERBOSE);

        // The main switch case for handling the commandService
        switch (cmdParameters[0]) // The first part of the message, the _command
        {
            case "!cat":
                BotMessaging.GetResponse(MessageChannel, _Message, "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687");
                break;
            case "!logprint":
                BotMessaging.GetResponse(MessageChannel, _Message, "PRINTING ALL LOG LEVELS");

                foreach (var item in Enum.GetValues(typeof(LogLevel)))
                {
                    Log.WriteLine("Log level: " + item.ToString(), (LogLevel)item);
                }

                break;
            default:
                BotMessaging.GetResponse(MessageChannel, _Message, "Unknown _command.", true);
                break;
        }

        return Task.CompletedTask;
    }*/
}