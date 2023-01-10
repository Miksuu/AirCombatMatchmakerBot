using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class REPORTSCOREBUTTON : BaseButton
{
    public REPORTSCOREBUTTON()
    {
        buttonName = ButtonName.REPORTSCOREBUTTON;
        buttonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, ulong _channelId,
        ulong _messageId, string _message)
    {
        string[] splitStrings = buttonCustomId.Split('_');

        string finalResponse = "Something went wrong with the reporting the match result of: " +
            splitStrings[1] + ". An admin has been informed";

        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + _channelId + 
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        foreach (var item in splitStrings)
        {
            Log.WriteLine(item, LogLevel.DEBUG);
        }

        // Find the league with the cached category ID
        var league =
            Database.Instance.Leagues.StoredLeagues.FirstOrDefault(
                l => l.DiscordLeagueReferences.LeagueCategoryId == buttonCategoryId);
        if (league == null)
        {
            Log.WriteLine("League with: " + buttonCategoryId + " was not found!", LogLevel.CRITICAL);
            return Task.FromResult(finalResponse);
        }

        Log.WriteLine("Found league: " + league.LeagueCategoryName, LogLevel.VERBOSE);

        LeagueMatch? foundMatch = league.LeagueData.Matches.MatchesList.FirstOrDefault(m => m.MatchChannelId == _channelId);
        if (foundMatch == null)
        {
            Log.WriteLine("Match with: " + _channelId + " was not found.", LogLevel.CRITICAL);
            return Task.FromResult(finalResponse);
        }

        Log.WriteLine("Found match: " + foundMatch.MatchId +
            " with channelId: " + foundMatch.MatchChannelId, LogLevel.DEBUG);

        // Find the teams that the players is in the league that was selected earlier
        List<Team> teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn =
            league.LeagueData.Teams.TeamsList.Where(
                t => t.CheckIfATeamContainsAPlayerById(playerId)).ToList();

        if (teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn == null ||
            teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn.Count < 1)
        {
            Log.WriteLine("Error! " + nameof(teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn) +
                " was null or empty!", LogLevel.CRITICAL);
            return Task.FromResult(finalResponse);
        }

        Log.WriteLine("count: " + teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn.Count +
            " of all teams: ", LogLevel.VERBOSE);

        foreach (Team team in teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn)
        {
            Log.WriteLine("Team: " + team.TeamName + " with id: " + team.TeamId + 
                " active: " + team.TeamActive, LogLevel.VERBOSE);
        }

        List<Team> isActiveInTeams =
            teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn.Where(
                t => t.TeamActive).ToList();

        if (isActiveInTeams == null || isActiveInTeams.Count < 1)
        {
            Log.WriteLine("Error! " + nameof(isActiveInTeams) + " was null or empty!", LogLevel.CRITICAL);
            return Task.FromResult(finalResponse);
        }

        Log.WriteLine("count: " + isActiveInTeams.Count + " of all teams: ", LogLevel.VERBOSE);

        // Should be always only one, because the player should be able to parcipiate only with one team at the time
        if (isActiveInTeams.Count != 1)
        {
            Log.WriteLine("count was not 1!!", LogLevel.DEBUG);

            foreach (Team team in isActiveInTeams)
            {
                Log.WriteLine("Team: " + team.TeamName + " with id: " + team.TeamId +
                    " active: " + team.TeamActive, LogLevel.DEBUG);
            }

            Log.WriteLine("Error! The player was active in two teams at the same time.", LogLevel.ERROR);

            // Handle error, take in the ID's of the current teams in the match and pick the player from there
            // Might be something that doesn't need to be handled if the system is proven to work fine
        }

        Team? foundTeam = isActiveInTeams.FirstOrDefault();

        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult("");
        }

        Log.WriteLine("Found team: " + foundTeam.TeamName + " with id: " + foundTeam.TeamId +
            " that should be active: " + foundTeam.TeamActive, LogLevel.DEBUG);

        // First time pressing the report button for the team
        if (!foundMatch.MatchReporting.TeamIdWithReportedResult.ContainsKey(foundTeam.TeamId))
        {
            Log.WriteLine("Key wasn't found, the team is first time reporting.", LogLevel.VERBOSE);
            foundMatch.MatchReporting.TeamIdWithReportedResult.Add(foundTeam.TeamId, playerReportedResult);
            finalResponse = "You reported score of: " + playerReportedResult;
        }
        // Replacing the result
        else
        {
            Log.WriteLine("Key was, the team is not their first time reporting.", LogLevel.VERBOSE);
            foundMatch.MatchReporting.TeamIdWithReportedResult[foundTeam.TeamId] = playerReportedResult;
            finalResponse = "You replaced the reported score to: " + playerReportedResult;
        }

        foreach (var reportedTeamKvp in foundMatch.MatchReporting.TeamIdWithReportedResult)
        {
            Log.WriteLine("Reported team: " + reportedTeamKvp.Key +
                " with value: " + reportedTeamKvp.Value, LogLevel.VERBOSE);
        }

        int reportedTeamsCount = foundMatch.MatchReporting.TeamIdWithReportedResult.Count;

        Log.WriteLine("Reported teams count: " + reportedTeamsCount, LogLevel.VERBOSE);

        if (reportedTeamsCount > 2)
        {
            Log.WriteLine("Count was: " + reportedTeamsCount + ", Error!", LogLevel.ERROR);

            // Maybe handle the error
        }

        Log.WriteLine("Reached end before the return with player id: " + playerId, LogLevel.DEBUG);

        return Task.FromResult(finalResponse);
    }
}