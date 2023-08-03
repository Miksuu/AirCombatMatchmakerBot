using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAVEMATCHSCHEDULER : BaseButton
{
    LeagueCategoryComponents lcc;
    public LEAVEMATCHSCHEDULER()
    {
        buttonName = ButtonName.LEAVEMATCHSCHEDULER;
        thisInterfaceButton.ButtonLabel = "LEAVE";
        buttonStyle = ButtonStyle.Danger;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override async Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        ulong playerId = _component.User.Id;
        ulong channelId = _component.Channel.Id;

        Log.WriteLine("Starting processing a leave by: " +
            playerId + " in channel: " + channelId);

        lcc = new LeagueCategoryComponents(_interfaceMessage.MessageCategoryId);
        if (lcc.interfaceLeagueCached == null)
        {
            string errorMsg = nameof(lcc.interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.ERROR);
            return new Response(errorMsg, false);
        }

        var matchScheduler = lcc.interfaceLeagueCached.LeagueData.MatchScheduler;
        Log.WriteLine(nameof(matchScheduler) + matchScheduler, LogLevel.DEBUG);

        var response = matchScheduler.RemoveTeamFromTheMatchSchedulerWithPlayerId(playerId);

        //_interfaceMessage.GenerateAndModifyTheMessage();
        //Log.WriteLine("After modifying message");

        return response;
    }
}