using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class COMMENT : BaseCommand
{
    public COMMENT()
    {
        commandName = CommandName.COMMENT;
        commandDescription = "Posts a comment about your match.";

        commandOption = new("comment", "Enter your comment here.");
    }

    public override async Task<string> ActivateCommandFunction(SocketSlashCommand _command)
    {
        Log.WriteLine("Activating a comment command: " + _command.ChannelId +
            " by: " + _command.User.Id, LogLevel.DEBUG);

        if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
            _command.Channel.Id))
        {
            return "You are not commenting on the match channel!";
        }
    }
}