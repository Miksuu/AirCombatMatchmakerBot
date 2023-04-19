using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.ComponentModel;

[DataContract]
public class CHALLENGEBUTTON : BaseChallengeChannelButton
{
    public CHALLENGEBUTTON()
    {
        buttonName = ButtonName.CHALLENGEBUTTON;
        buttonLabel = "Enter Queue";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = true;
    }

    public override async Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        ulong playerId = _component.User.Id;
        ulong channelId = _component.Channel.Id;
        
        Log.WriteLine("Starting processing a challenge by: " +
            playerId + " in channel: " + channelId, LogLevel.VERBOSE);
        
        FindInterfaceLeagueAndCacheIt(channelId);
        if (interfaceLeagueCached == null)
        {
            string errorMsg = nameof(interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        var challengeStatusOfTheCurrentLeague = interfaceLeagueCached.LeagueData.ChallengeStatus;

        Team? playerTeam =
            interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);
        if (playerTeam == null)
        {
            Log.WriteLine(nameof(playerTeam) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return ("Error! Team not found", false);
        }

        Log.WriteLine("Team found: " + playerTeam.GetTeamName(interfaceLeagueCached.LeaguePlayerCountPerTeam) +
            " (" + playerTeam.TeamId + ")" + " adding it to the challenge queue.", LogLevel.VERBOSE);

        // Add to method
        foreach (InterfaceLeague league in Database.Instance.Leagues.StoredLeagues)
        {
            var challengeStatusOfTheTempLeague = league.LeagueData.ChallengeStatus;

            Log.WriteLine("Loop on " + nameof(league) + ": " + league.LeagueCategoryName +
                " with cache: " + interfaceLeagueCached.LeagueCategoryName, LogLevel.VERBOSE);
            if (league.LeagueCategoryName == interfaceLeagueCached.LeagueCategoryName)
            {
                Log.WriteLine("on " + league.LeagueCategoryName + ", skipping", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine("Searching: " + league.LeagueCategoryName, LogLevel.VERBOSE);

            if (!league.LeagueData.CheckIfPlayerIsParcipiatingInTheLeague(playerId))
            {
                Log.WriteLine(playerId + " is not parcipiating in this league: " +
                    interfaceLeagueCached.LeagueCategoryName + ", disregarding", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine(playerId + " is parcipiating in this league: " + league.LeagueCategoryName, LogLevel.VERBOSE);

            Team? teamToSearchFor = league.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);
            if (teamToSearchFor == null)
            {
                Log.WriteLine(nameof(teamToSearchFor) +
                    " was null! Could not find the team.", LogLevel.CRITICAL);
                return ("Error! Team null!", false);
            }

            if (challengeStatusOfTheTempLeague.CheckIfPlayerTeamIsAlreadyInQueue(teamToSearchFor))
            {
                Log.WriteLine(playerId + " already at queue at: " + channelId, LogLevel.VERBOSE);
                // Add link to the channel
                return ("You are already in the queue at another league: " + league.LeagueCategoryName, false);
            }

            Log.WriteLine(playerId + " not in the queue at: " + channelId + "name: " + league.LeagueCategoryName, LogLevel.VERBOSE);
        }

        string response = challengeStatusOfTheCurrentLeague.PostChallengeToThisLeague(
            interfaceLeagueCached.LeaguePlayerCountPerTeam, interfaceLeagueCached, playerTeam);
        if (response == "alreadyInQueue")
        {
            Log.WriteLine(playerId + " was already in the queue!", LogLevel.VERBOSE);
            return ("You are already in the queue!", false);
        }
        Log.WriteLine("response was: " + response, LogLevel.VERBOSE);

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return ("", true);
    }
}