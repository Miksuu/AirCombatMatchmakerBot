using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class REPORTINGSTATUSMESSAGE : BaseMessage
{
    public REPORTINGSTATUSMESSAGE()
    {
        messageName = MessageName.REPORTINGSTATUSMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>();
        message = "Insert the reporting status message here";
    }

    public override string GenerateMessage()
    {
        string reportingStatusMessage = "Current reporting status: \n";

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(messageCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return "";
        }

        LeagueMatch? foundMatch =
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(messageChannelId);
        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return "";
        }

        foreach (var teamKvp in foundMatch.TeamsInTheMatch)
        {
            string reportingStatusPerTeam = teamKvp.Value + ": ";

            // Contains the reporting result, add to the message
            if (foundMatch.MatchReporting.TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                var teamReportData = foundMatch.MatchReporting.TeamIdsWithReportData[teamKvp.Key];
                var reportedResult = teamReportData.ReportedResult;
                var tacviewLink = teamReportData.TacviewLink;

                Log.WriteLine("Found team's: " + teamKvp.Key + " (" + teamKvp.Value + ")" +
                    " reported result: " + reportedResult, LogLevel.VERBOSE);

                reportingStatusPerTeam += reportedResult;
                if (tacviewLink != "") reportingStatusPerTeam += tacviewLink;
            }
            // Does not contain the reporting result, just add "none"
            else
            {
                reportingStatusPerTeam += "Not reported yet";
            }

            Log.WriteLine("Done looping through team: " + teamKvp.Key + " (" + teamKvp.Value +
                ")" + "with message: " + reportingStatusPerTeam, LogLevel.VERBOSE);
            reportingStatusMessage += reportingStatusPerTeam + "\n";
        }

        Log.WriteLine("Returning: " + reportingStatusMessage, LogLevel.DEBUG);

        return reportingStatusMessage;
    }
}