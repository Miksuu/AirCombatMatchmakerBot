using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class MATCHFINALRESULTMESSAGE : BaseMessage
{
    public MATCHFINALRESULTMESSAGE()
    {
        messageName = MessageName.CONFIRMATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
        };
        message = "Insert the confirmation message here";
    }

    public override string GenerateMessage()
    {
        string finalMessage = "FINAL REPORTING RESULTS:\n";

        InterfaceLeague? interfaceLeague = 
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(messageCategoryId);

        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "";
        }

        LeagueMatch? leagueMatch = 
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(messageChannelId);

        if (leagueMatch == null)
        {
            Log.WriteLine(nameof(leagueMatch) +
                " was null! Could not find the match.", LogLevel.CRITICAL);
            return "";
        }

        Dictionary<int, ReportData>? matchReportingTeamIdsWithReportData = leagueMatch.MatchReporting.TeamIdsWithReportData;

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.TeamName + ": " + reportDataKvp.Value.ReportedScore.ObjectValue + "\n";
        }

        Log.WriteLine("Returning: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}