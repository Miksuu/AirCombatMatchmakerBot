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

    protected override void GenerateCustomButtonProperties(int _buttonIndex, ulong _channelCategoryId)
    {
        string customId = string.Empty;

        if (_channelCategoryId == 0)
        {
            Log.WriteLine("Failed to receive the _channelCategoryId!", LogLevel.ERROR);
            return;
        }

        InterfaceButton buttonInterface = this;

        buttonInterface.CustomOperationId = _channelCategoryId.ToString() + "_" + _buttonIndex;
        Log.WriteLine("Set league-registration button CustomOperationId to: " +
            buttonInterface.CustomOperationId, LogLevel.DEBUG);
    }

    public override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("starting leagueRegistration");

        InterfaceButton buttonInterface = this;

        string[] splitStrings = buttonInterface.CustomOperationId.Split('_');

        Log.WriteLine(Database.GetInstance<ApplicationDatabase>().Leagues.StoredLeagues.Count.ToString());

        // Add try-catch here
        InterfaceLeague interfaceLeague =
            Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryId(ulong.Parse(splitStrings[0]));
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

    private async void InitMessageModifyOnSecondThread(InterfaceMessage _interfaceMessage, ulong _channelCategoryId)
    {
        _interfaceMessage.GenerateAndModifyTheMessage(_channelCategoryId);

        await SerializationManager.SerializeDB();
    }
}