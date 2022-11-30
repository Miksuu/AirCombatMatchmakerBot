using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord.Net;
using Discord;
using Newtonsoft.Json;


/*
// Create a module with no prefix
public class InfoModule : ModuleBase<SocketCommandContext>
{
    // ~say hello world -> hello world
    [Discord.Interactions.SlashCommand("say", "say something")]
    [Summary("Echoes a message.")]
    public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        => ReplyAsync(echo);

    // ReplyAsync is a method on ModuleBase 
}

// Create a module with the 'sample' prefix
[Group("sample")]
public class SampleModule : ModuleBase<SocketCommandContext>
{
    [Discord.Interactions.SlashCommand("square", "square a number")]
    // ~sample square 20 -> 400
    [Summary("Squares a number.")]
    public async Task SquareAsync(
        [Summary("The number to square.")]
        int num)
    {
        // We can also access the channel from the Command Context.
        await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
    }

    // ~sample userinfo --> foxbot#0282
    // ~sample userinfo @Khionu --> Khionu#8708
    // ~sample userinfo Khionu#8708 --> Khionu#8708
    // ~sample userinfo Khionu --> Khionu#8708
    // ~sample userinfo 96642168176807936 --> Khionu#8708
    // ~sample whois 96642168176807936 --> Khionu#8708
    [Discord.Interactions.SlashCommand("userinfo", "enter uid")]
    [Summary
    ("Returns info about the current user, or the user parameter, if one passed.")]
    [Alias("user", "whois")]
    public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")]
        SocketUser user = null)
    {
        var userInfo = user ?? Context.Client.CurrentUser;
        await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}
*/

public class CommandHandler
{
    public static async Task InstallCommandsAsync()
    {
        var guildCommand = new SlashCommandBuilder()
            .WithName("list-roles")
            .WithDescription("Lists all roles of a user.")
            .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);

        await BotReference.clientRef.Rest.CreateGuildCommand(guildCommand.Build(), BotReference.GuildID);

        // Hook the MessageReceived event into our command handler
        //BotReference.clientRef.MessageReceived += HandleCommandAsync;

        // Here we discover all of the command modules in the entry 
        // assembly and load them. Starting from Discord.NET 2.0, a
        // service provider is required to be passed into the
        // module registration method to inject the 
        // required dependencies.
        //
        // If you do not use Dependency Injection, pass null.
        // See Dependency Injection guide for more information.
        /*
        await BotReference.commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                        services: null);*/
    }

    /*
    private static async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commandService
        if (!(message.HasCharPrefix('/', ref argPos) ||
            message.HasMentionPrefix(BotReference.clientRef.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(BotReference.clientRef, message);

        // Execute the command with the command context we just
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
        switch (cmdParameters[0]) // The first part of the message, the command
        {
            case "!cat":
                BotMessaging.SendMessage(MessageChannel, _Message, "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687");
                break;
            case "!logprint":
                BotMessaging.SendMessage(MessageChannel, _Message, "PRINTING ALL LOG LEVELS");

                foreach (var item in Enum.GetValues(typeof(LogLevel)))
                {
                    Log.WriteLine("Log level: " + item.ToString(), (LogLevel)item);
                }

                break;
            default:
                BotMessaging.SendMessage(MessageChannel, _Message, "Unknown command.", true);
                break;
        }

        return Task.CompletedTask;
    }*/
}