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
            "userid",
            "which user do you want to terminate?"
            );

        Log.WriteLine("Done preparing the commands.", LogLevel.VERBOSE);

        return Task.CompletedTask;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand _command)
    {
        var firstOptionValue = _command.Data.Options.First().Value.ToString();

        if (firstOptionValue == null)
        {
            Log.WriteLine("The command " + _command.Data.Name + " does not have any options in it.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("The command " + _command.Data.Name + " had " + _command.Data.Options.Count + " in it." +
                " The first command had an argument: " + firstOptionValue, LogLevel.DEBUG);

            // Add a for loop here to print the command arguments, if multiple later on.
        }

        switch (_command.Data.Name)
        {
            case "cat":
                await _command.RespondAsync(BotMessaging.GetMessageResponse(_command.Data.Name,
                    "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687", _command.Channel.Name));
                break;
            // ADMIN COMMAND
            case "terminate":
                if (CheckIfCommandSenderWasAnAdmin(_command))
                {
                    await PlayerManager.DeletePlayerProfile(firstOptionValue);
                }
                break;
            default:
                await _command.RespondAsync(BotMessaging.GetMessageResponse(_command.Data.Name,
                    "Unknown command!",
                    _command.Channel.Name));
                break;
        }

        Log.WriteLine("Sending and responding to the message done.", LogLevel.VERBOSE);
    }

    private static bool CheckIfCommandSenderWasAnAdmin(SocketSlashCommand _command)
    {
        return Database.Instance.adminIDs.Contains(_command.User.Id);
    }
}