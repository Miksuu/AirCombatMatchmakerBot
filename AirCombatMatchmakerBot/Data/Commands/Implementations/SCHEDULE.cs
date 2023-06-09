using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class SCHEDULE : BaseMatchCommand
{
    public SCHEDULE()
    {
        commandName = CommandName.SCHEDULE;
        commandDescription = "Schedules a match to a specific time";
        commandOption = new("time", "Enter your time here");
        isAdminCommand = false;

        matchStateAllowedWithMessage = new Dictionary<MatchState, string>
            {
                { MatchState.SCHEDULINGPHASE, "This match isn't on the scheduling phase!" }
            };

    }

    protected override async Task<Response> ActivateCommandFunction(SocketSlashCommand _command, string _firstOptionString)
    {
        /*
        ulong commandChannelId = _command.Channel.Id;
        ulong commandPlayerId = _command.User.Id;

        Log.WriteLine("Activating a comment command: " + commandChannelId +
            " by: " + commandPlayerId + " with: " + _firstOptionString, LogLevel.DEBUG);

        mcc = new MatchChannelComponents(commandChannelId);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
            commandChannelId))
        {
            return new Response("You are not commenting on the match channel!", false);
        }

        Response matchStateResponse =
            CheckThatPlayerIsInTheMatchAndReturnResponseFromMatchStatesThatAreNotAllowed(
                commandPlayerId, commandChannelId);
        if (matchStateResponse.serialize == false)
        {
            return matchStateResponse;
        }

        var finalResponseTuple =
            await mcc.leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
                 mcc.interfaceLeagueCached, commandPlayerId,
                    _firstOptionString, TypeOfTheReportingObject.COMMENTBYTHEUSER,
                    mcc.interfaceLeagueCached.LeagueCategoryId, commandChannelId);

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
        }*/

        return new Response("Couldn't schedule (not implemented yet)", false);
    }
}