using Discord;
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
            Log.WriteLine("Failed to receive the _leagueCategoryId!", LogLevel.ERROR);
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
        Log.WriteLine("starting leagueRegistration");

        string[] splitStrings = _component.Data.CustomId.Split('_');

        /*
        foreach (var item in splitStrings)
        {
            Log.WriteLine("item: " + item);
        }*/

        // Add try-catch here
        InterfaceLeague interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(ulong.Parse(splitStrings[0]));
        if (interfaceLeague == null)
        {
            string errorMsg = nameof(interfaceLeague) + " was null! Could not find the league.";
            Log.WriteLine(errorMsg, LogLevel.ERROR);
            return Task.FromResult(new Response(errorMsg, false));
        }

        var response = interfaceLeague.RegisterUserToALeague(_component.User.Id).Result;

        if (response.serialize)
        {
            // Improved response time
            new Thread(() => InitMessageModifyOnSecondThread(_interfaceMessage, ulong.Parse(splitStrings[0]))).Start();
        }

        return Task.FromResult(response);
    }

    private async void InitMessageModifyOnSecondThread(InterfaceMessage _interfaceMessage, ulong _leagueCategoryId)
    {
        _interfaceMessage.GenerateAndModifyTheMessage(_leagueCategoryId);

        await SerializationManager.SerializeDB();
    }
}