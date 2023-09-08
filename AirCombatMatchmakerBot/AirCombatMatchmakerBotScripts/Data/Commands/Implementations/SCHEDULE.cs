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

        matchStateAllowedWithMessage = (MatchState.SCHEDULINGPHASE, "You can only use the /schedule command during scheduling phase.");
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
            Log.WriteLine(errorMsg, LogLevel.ERROR);
            return new Response(errorMsg, false);
        }

        if (!Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId.ContainsKey(
            commandChannelId))
        {
            return new Response("You are not commenting on the match channel!", false);
        }

        Response matchStateResponse =
            CheckThatPlayerIsInTheMatchAndReturnResponseFromMatchStatesThatAreNotAllowed(
                commandPlayerId, commandChannelId);
        if (!matchStateResponse.serialize)
        {
            return matchStateResponse;
        }

        return await mcc.leagueMatchCached.CreateScheduleSuggestion(commandPlayerId, _firstOptionString);
    }
}