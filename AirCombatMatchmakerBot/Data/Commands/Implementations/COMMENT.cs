using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class COMMENT : BaseCommand
{
    public COMMENT()
    {
        string removeComment = " Type " + @"""" + " - " + @"""" + " to remove your comment";

        commandName = CommandName.COMMENT;
        commandDescription = "Posts a comment about your match." + removeComment;
        commandOption = new("comment", "Enter your comment here." + removeComment);
        isAdminCommand = false;
    }

    protected override async Task<Response> ActivateCommandFunction(SocketSlashCommand _command, string _firstOptionString)
    {
        ulong commandChannelId = _command.Channel.Id;
        ulong commandPlayerId = _command.User.Id;

        Log.WriteLine("Activating a comment command: " + commandChannelId +
            " by: " + commandPlayerId + " with: " + _firstOptionString, LogLevel.DEBUG);

        if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
            commandChannelId))
        {
            return new Response("You are not commenting on the match channel!", false);
        }

        var leagueInterfaceWithTheMatch =
            Database.Instance.Leagues.FindLeagueInterfaceAndLeagueMatchWithChannelId(commandChannelId);

        if (leagueInterfaceWithTheMatch.Item1 == null || leagueInterfaceWithTheMatch.Item2 == null)
        {
            Log.WriteLine(nameof(leagueInterfaceWithTheMatch) + " was null!", LogLevel.ERROR);
            return new Response("Error while processing your command!", false);
        }

        if (!leagueInterfaceWithTheMatch.Item2.GetIdsOfThePlayersInTheMatchAsArray(
            leagueInterfaceWithTheMatch.Item1).Contains(commandPlayerId))
        {
            Log.WriteLine("User: " + commandPlayerId + " tried to comment on channel: " +
                commandChannelId + "!", LogLevel.WARNING);
            return new Response("That's not your match to comment on!", false);
        }

        var finalResponseTuple =
            await leagueInterfaceWithTheMatch.Item2.MatchReporting.ProcessPlayersSentReportObject(
                 leagueInterfaceWithTheMatch.Item1, commandPlayerId,
                    _firstOptionString, TypeOfTheReportingObject.COMMENTBYTHEUSER,
                    leagueInterfaceWithTheMatch.Item1.LeagueCategoryId, commandChannelId);

        if (finalResponseTuple.serialize)
        {
            if (_firstOptionString == "-")
            {
                return new Response("Comment removed!", true);
            }
            else
            {
                return new Response("Comment posted: " + _firstOptionString, true);
            }
            
        }

        return new Response("Couldn't post comment", false);
    }
}