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
        if (BotReference.clientRef != null)
        {
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
        CommandBuilder.AddNewCommand("cat", "prints a cute cat");

        return Task.CompletedTask;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand _command)
    {
        switch (_command.Data.Name)
        {
            case "cat":
                await _command.RespondAsync(BotMessaging.GetResponse(_command,
                    "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687"));
                break;
            default:
                await _command.RespondAsync(BotMessaging.GetResponse(_command, "Unknown command!"));
                break;
        }

        Log.WriteLine("Sending and responding to the message done.", LogLevel.VERBOSE);
    }
}