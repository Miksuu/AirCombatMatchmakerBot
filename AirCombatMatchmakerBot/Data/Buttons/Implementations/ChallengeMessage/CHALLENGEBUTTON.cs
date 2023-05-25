using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.ComponentModel;

[DataContract]
public class CHALLENGEBUTTON : BaseButton
{
    LeagueCategoryComponents lcc = new LeagueCategoryComponents();
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
        Team playerTeam;


        Log.WriteLine("Starting processing a challenge by: " +
            playerId + " in channel: " + channelId, LogLevel.VERBOSE);

        lcc.FindCategorysLeagueAndInsertItToTheCache(_interfaceMessage.MessageCategoryId);
        if (lcc.interfaceLeagueCached == null)
        {
            string errorMsg = nameof(lcc.interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        //Log.WriteLine("Found: " + nameof(mcc), LogLevel.DEBUG);

        var challengeStatusOfTheCurrentLeague = lcc.interfaceLeagueCached.LeagueData.ChallengeStatus;
        Log.WriteLine(nameof(challengeStatusOfTheCurrentLeague) + challengeStatusOfTheCurrentLeague, LogLevel.DEBUG);

        try 
        {
            playerTeam =
                lcc.interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }

        Log.WriteLine("Team found: " + playerTeam.GetTeamName(lcc.interfaceLeagueCached.LeaguePlayerCountPerTeam) +
            " (" + playerTeam.TeamId + ")" + " adding it to the challenge queue.", LogLevel.VERBOSE);

        // Add to method
        foreach (InterfaceLeague league in Database.Instance.Leagues.StoredLeagues)
        {
            Team teamToSearchFor;

            var challengeStatusOfTheTempLeague = league.LeagueData.ChallengeStatus;

            Log.WriteLine("Loop on " + nameof(league) + ": " + league.LeagueCategoryName +
                " with cache: " + lcc.interfaceLeagueCached.LeagueCategoryName, LogLevel.VERBOSE);
            if (league.LeagueCategoryName == lcc.interfaceLeagueCached.LeagueCategoryName)
            {
                Log.WriteLine("on " + league.LeagueCategoryName + ", skipping", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine("Searching: " + league.LeagueCategoryName, LogLevel.VERBOSE);

            if (!league.LeagueData.CheckIfPlayerIsParcipiatingInTheLeague(playerId))
            {
                Log.WriteLine(playerId + " is not parcipiating in this league: " +
                    lcc.interfaceLeagueCached.LeagueCategoryName + ", disregarding", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine(playerId + " is parcipiating in this league: " + league.LeagueCategoryName, LogLevel.VERBOSE);

            try
            {
                teamToSearchFor = league.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                continue;
                //return new Response(ex.Message, false);
            }

            if (challengeStatusOfTheTempLeague.CheckIfPlayerTeamIsAlreadyInQueue(teamToSearchFor))
            {
                Log.WriteLine(playerId + " already at queue at: " + channelId, LogLevel.VERBOSE);
                // Add link to the channel
                return new Response("You are already in the queue at another league: " + league.LeagueCategoryName, false);
            }

            Log.WriteLine(playerId + " not in the queue at: " + channelId + "name: " + league.LeagueCategoryName, LogLevel.VERBOSE);
        }

        string response = challengeStatusOfTheCurrentLeague.PostChallengeToThisLeague(
            lcc.interfaceLeagueCached.LeaguePlayerCountPerTeam, lcc.interfaceLeagueCached, playerTeam);
        if (response == "alreadyInQueue")
        {
            Log.WriteLine(playerId + " was already in the queue!", LogLevel.VERBOSE);
            return new Response("You are already in the queue!", false);
        }
        Log.WriteLine("response was: " + response, LogLevel.VERBOSE);

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return new Response("", true);
    }
}