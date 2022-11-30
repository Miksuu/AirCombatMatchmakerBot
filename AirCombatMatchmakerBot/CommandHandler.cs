using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord.Net;
using Discord;
using Newtonsoft.Json;

public static class CommandHandler
{
    public static Task InstallCommandsAsync()
    {
        Log.WriteLine("Starting to install the commands.", LogLevel.VERBOSE);

        if (BotReference.clientRef != null)
        {
            Log.WriteLine("clientRef is not null.", LogLevel.VERBOSE);
            BotReference.clientRef.Ready += PrepareCommands;
            BotReference.clientRef.SlashCommandExecuted += SlashCommandHandler;
        }
        else
        {
            Exceptions.BotClientRefNull();
        }

        return Task.CompletedTask;
    }

    private static Task PrepareCommands()
    {
        Log.WriteLine("Starting to prepare the commands.", LogLevel.VERBOSE);

        CommandBuilder.AddNewCommand("cats", "Prints a cute cat!");
        CommandBuilder.AddNewCommandWithOption("terminate",
            "deletes a player profile, completely",
            "userId", 
            "which user do you want to terminate?"
            );

        Log.WriteLine("Done preparing the commands.", LogLevel.VERBOSE);

        return Task.CompletedTask;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand _command)
    {
        switch (_command.Data.Name)
        {
            case "cat":
                await _command.RespondAsync(BotMessaging.GetMessageResponse(_command.Data.Name,
                    "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687", _command.Channel.Name));
                break;
            case "terminate":
                PlayerManager.DeletePlayerProfile();
                break;
            default:
                await _command.RespondAsync(BotMessaging.GetMessageResponse(_command.Data.Name,
                    "Unknown command!",
                    _command.Channel.Name));
                break;
        }

        Log.WriteLine("Sending and responding to the message done.", LogLevel.VERBOSE);
    }
}