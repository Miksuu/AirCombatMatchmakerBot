using Discord;
using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEBUTTON : BaseButton
{
    public CHALLENGEBUTTON()
    {
        buttonName = ButtonName.CHALLENGEBUTTON;
        buttonLabel = "CHALLENGE";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, string _splitString)
    {

        Log.WriteLine("Starting processing a challenge by: " + _component.User.Id +
             " for league: " + _splitString, LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance = 
            Database.Instance.Leagues.FindLeagueInterfaceWithSplitStringPart(_splitString);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "";
        }

        dbLeagueInstance.LeagueData.PostChallengeToThisLeague(
            _component.User.Id, dbLeagueInstance.LeaguePlayerCountPerTeam);

        /*
        string response = "";
        // Checks that the player does not exist in the database already, true if this is not the case
        if (Database.Instance.PlayerData.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
        {
            Database.Instance.CachedUsers.AddUserIdToCachedList(_component.User.Id);

            response = _component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " registration complete, welcome!",
                    _component.Channel.Name);
        }
        else
        {
            response = _component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " You are already registered!",
                    _component.Channel.Name);
        }
        return response;*/

        return "";
    }
}