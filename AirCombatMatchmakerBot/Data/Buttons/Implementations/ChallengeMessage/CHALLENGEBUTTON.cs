using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEBUTTON : BaseButton
{
    LeagueCategoryComponents lcc;
    public CHALLENGEBUTTON()
    {
        buttonName = ButtonName.CHALLENGEBUTTON;
        thisInterfaceButton.ButtonLabel = "Enter Queue";
        buttonStyle = ButtonStyle.Success;
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

        Log.WriteLine("Starting processing a challenge by: " +
            playerId + " in channel: " + channelId, LogLevel.VERBOSE);

        lcc = new LeagueCategoryComponents(_interfaceMessage.MessageCategoryId);
        if (lcc.interfaceLeagueCached == null)
        {
            string errorMsg = nameof(lcc.interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        //Log.WriteLine("Found: " + nameof(mcc), LogLevel.DEBUG);

        var challengeStatusOfTheCurrentLeague = lcc.interfaceLeagueCached.LeagueData.ChallengeStatus;
        Log.WriteLine(nameof(challengeStatusOfTheCurrentLeague) + challengeStatusOfTheCurrentLeague, LogLevel.DEBUG);

        var response = challengeStatusOfTheCurrentLeague.AddTeamFromPlayerIdToTheQueue(
            playerId, _interfaceMessage);


        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return response;
    }
}