using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class ACCEPTSCHEDULEDTIME : BaseButton
{
    MatchChannelComponents mcc;
    public ACCEPTSCHEDULEDTIME()
    {
        buttonName = ButtonName.ACCEPTSCHEDULEDTIME;
        thisInterfaceButton.ButtonLabel = "ACCEPT";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = false;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public async override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        mcc = new MatchChannelComponents(_interfaceMessage);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        var playerId = _component.User.Id;

        return await mcc.leagueMatchCached.AcceptMatchScheduling(
            playerId,
            mcc.interfaceLeagueCached.LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(playerId).TeamId);
    }
}