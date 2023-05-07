using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUEREGISTRATIONBUTTON : BaseButton
{
    public LEAGUEREGISTRATIONBUTTON()
    {
        buttonName = ButtonName.LEAGUEREGISTRATIONBUTTON;
        thisInterfaceButton.ButtonLabel = "Join";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        string customId = string.Empty;

        if (_leagueCategoryId == 0)
        {
            Log.WriteLine("Failed to receive the _leagueCategoryId!", LogLevel.CRITICAL);
            return customId;
        }

        customId = _leagueCategoryId.ToString() + "_" + _buttonIndex;
        Log.WriteLine("Setting league-registration button custom id to: " +
            customId, LogLevel.DEBUG);

        return customId;
    }

    public override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("starting leagueRegistration", LogLevel.VERBOSE);

        string[] splitStrings = _component.Data.CustomId.Split('_');

        foreach (var item in splitStrings)
        {
            Log.WriteLine("item: " + item, LogLevel.VERBOSE);
        }

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(ulong.Parse(splitStrings[0]));
        if (interfaceLeague == null)
        {
            string errorMsg = nameof(interfaceLeague) + " was null! Could not find the league.";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult(new Response(errorMsg, false));
        }

        var responseTuple = interfaceLeague.RegisterUserToALeague(_component.User.Id).Result;

        if (responseTuple.serialize)
        {
            // Improved response time
            new Thread(() => InitMessageModifyOnSecondThread(_interfaceMessage)).Start();
        }

        return Task.FromResult(responseTuple);
    }

    private async void InitMessageModifyOnSecondThread(InterfaceMessage _interfaceMessage)
    {
        LEAGUEREGISTRATIONMESSAGE? leagueRegistrationMessage = _interfaceMessage as LEAGUEREGISTRATIONMESSAGE;
        if (leagueRegistrationMessage == null)
        {
            string errorMsg = nameof(leagueRegistrationMessage) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return;
        }

        await _interfaceMessage.ModifyMessage(leagueRegistrationMessage.GenerateMessageForSpecificCategoryLeague());
    }
}