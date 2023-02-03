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

    public override async Task<string> ActivateCommandFunction(SocketSlashCommand _command, string _firstOptionString)
    {
        ulong commandChannelId = _command.Channel.Id;
        ulong commandPlayerId = _command.User.Id;

        Log.WriteLine("Activating a comment command: " + commandChannelId +
            " by: " + commandPlayerId, LogLevel.DEBUG);

        if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
            commandChannelId))
        {
            return "You are not commenting on the match channel!";
        }

        var leagueInterfaceWithTheMatch =
            Database.Instance.Leagues.FindLeagueInterfaceAndLeagueMatchWithChannelId(commandChannelId);

        if (leagueInterfaceWithTheMatch.Item1 == null || leagueInterfaceWithTheMatch.Item2 == null)
        {
            Log.WriteLine(nameof(leagueInterfaceWithTheMatch) + " was null!", LogLevel.ERROR);
            return "Error while processing your command!";
        }

        if (!leagueInterfaceWithTheMatch.Item2.GetIdsOfThePlayersInTheMatchAsArray(
            leagueInterfaceWithTheMatch.Item1).Contains(commandPlayerId))
        {
            Log.WriteLine("User: " + commandPlayerId + " tried to comment on channel: " +
                commandChannelId + "!", LogLevel.WARNING);
            return "That's not your match to comment on!";
        }

        InterfaceMessage? reportingStatusMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                leagueInterfaceWithTheMatch.Item1.DiscordLeagueReferences.LeagueCategoryId).
                    Value.FindInterfaceChannelWithIdInTheCategory(
                        commandChannelId).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.REPORTINGSTATUSMESSAGE);

        if (reportingStatusMessage == null)
        {
            Log.WriteLine(nameof(reportingStatusMessage) + " was null!", LogLevel.CRITICAL);
            return nameof(reportingStatusMessage) + " was null!";
        }

        await leagueInterfaceWithTheMatch.Item2.MatchReporting.ProcessPlayersSentReportObject(
                 leagueInterfaceWithTheMatch.Item1,
                 commandPlayerId, reportingStatusMessage,
                 _firstOptionString, TypeOfTheReportingObject.COMMENTBYTHEUSER);

        return "Comment posted: " +  _firstOptionString;
    }
}