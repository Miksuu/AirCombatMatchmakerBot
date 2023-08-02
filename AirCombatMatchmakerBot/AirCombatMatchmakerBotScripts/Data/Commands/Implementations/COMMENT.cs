using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class COMMENT : BaseMatchCommand
{
    public COMMENT()
    {
        string removeComment = " Type " + @"""" + " - " + @"""" + " to remove your comment";

        commandName = CommandName.COMMENT;
        commandDescription = "Posts a comment about your match." + removeComment;
        commandOption = new("comment", "Enter your comment here." + removeComment);
        isAdminCommand = false;

        matchStatesNotAllowedWithMessage = new Dictionary<MatchState, string>
            {
                { MatchState.SCHEDULINGPHASE, "You can not use the /comment command during the scheduling phase!" },
                { MatchState.PLAYERREADYCONFIRMATIONPHASE, "The match has not even begun yet, your comment was not posted!" },
                { MatchState.MATCHDONE, "That match is already done, your comment was not posted!" }
            };
    }

    protected override async Task<Response> ActivateCommandFunction(SocketSlashCommand _command, string _firstOptionString)
    {
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

        if (!Database.Instance.MatchChannelsIdWithCategoryId.ContainsKey(
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
                commandPlayerId, _firstOptionString, TypeOfTheReportingObject.COMMENTBYTHEUSER,
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
        }

        return new Response("Couldn't post comment", false);
    }
}