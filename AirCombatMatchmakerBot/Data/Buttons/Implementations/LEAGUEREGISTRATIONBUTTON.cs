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
        buttonLabel = "Join";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    public override async Task<(string, bool)> ActivateButtonFunction(
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
            return (errorMsg, false);
        }

        var responseTuple = interfaceLeague.RegisterUserToALeague(_component.User.Id).Result;

        if (responseTuple.Item2)
        {
            // Improved response time
            new Thread(() => InitMessageModifyOnSecondThread(_interfaceMessage)).Start();
        }

        return responseTuple;
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