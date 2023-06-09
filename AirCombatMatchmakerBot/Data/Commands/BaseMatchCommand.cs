using System.Collections.Concurrent;
using System.Runtime.Serialization;

public abstract class BaseMatchCommand : BaseCommand
{
    protected MatchChannelComponents mcc;

    // Match states not allowed
    protected Dictionary<MatchState, string> matchStatesNotAllowedWithMessage
        = new Dictionary<MatchState, string>();

    protected (MatchState, string) matchStateAllowedWithMessage;

    protected Response CheckThatPlayerIsInTheMatchAndReturnResponseFromMatchStatesThatAreNotAllowed(
        ulong _commandPlayerId, ulong _matchChannelId)
    {
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        MatchState matchState = mcc.leagueMatchCached.MatchReporting.MatchState;

        if (!mcc.leagueMatchCached.GetIdsOfThePlayersInTheMatchAsArray(
                mcc.interfaceLeagueCached).Contains(_commandPlayerId))
        {
            Log.WriteLine("User: " + _commandPlayerId + " tried to comment on channel: " +
                _matchChannelId + "!", LogLevel.WARNING);
            return new Response("That's not your match to comment on!", false);
        }

        Log.WriteLine("Starting to check: " + mcc.leagueMatchCached.MatchReporting.MatchState, LogLevel.VERBOSE);
        if (matchStatesNotAllowedWithMessage.ContainsKey(matchState))
        {
            Log.WriteLine(nameof(matchStatesNotAllowedWithMessage) + " does not contain key: " +
                matchState + " returning false", LogLevel.VERBOSE);
            return new Response(matchStatesNotAllowedWithMessage[matchState], false);
        }

        else
        {
            Log.WriteLine(nameof(matchStatesNotAllowedWithMessage) + " does not contain key: " +
                matchState + " returning false", LogLevel.VERBOSE);
            return new Response("", true);
        }
    }
}