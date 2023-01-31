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
        Log.WriteLine("Starting to generate the message for the match final result", LogLevel.DEBUG);

        string finalMessage = " \n";

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

        finalMessage += "Match " + leagueMatch.MatchId + " has finished\n";

        Dictionary<int, ReportData>? matchReportingTeamIdsWithReportData = leagueMatch.MatchReporting.TeamIdsWithReportData;

        finalMessage += "\nPlayers: ";

        // Shows the team names
        int pIndex = 0;
        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.TeamName;
            if (pIndex == 0) finalMessage += " vs ";
            pIndex++;
        }

        finalMessage += "\nScore: ";

        // Shows the final score
        int sIndex = 0;
        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.ReportedScore.ObjectValue;
            if (sIndex == 0) finalMessage += " - ";
            sIndex++;
        }

        finalMessage += "\nRatings change: ";

        // Shows the rating change
        int rIndex = 0;
        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            string ratingEmoji = ":no_change_in_rating:";
            
            Log.WriteLine("FinalEloDelta on report message construction: " + reportDataKvp.Value.FinalEloDelta, LogLevel.DEBUG);

            if (reportDataKvp.Value.FinalEloDelta > 0f)
            {
                ratingEmoji = EnumExtensions.GetEnumMemberAttrValue(EmojiName.RATINGUP);
            }
            else if (reportDataKvp.Value.FinalEloDelta < 0f)
            {
                ratingEmoji = EnumExtensions.GetEnumMemberAttrValue(EmojiName.RATINGDOWN);
            }

            finalMessage += reportDataKvp.Value.TeamName + " " + ratingEmoji + " " + reportDataKvp.Value.FinalEloDelta;

            Log.WriteLine("finalMessage on report message construction: " + reportDataKvp.Value.FinalEloDelta, LogLevel.DEBUG);

            if (rIndex == 0) finalMessage += " | ";
            rIndex++;
        }

        finalMessage += "\nTacviews: ";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.TacviewLink.ObjectValue + " ";
        }

        finalMessage += "\n";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            if (!reportDataKvp.Value.CommentByTheUser.FieldFilled)
            {
                Log.WriteLine(reportDataKvp.Value.TeamName + " did not comment.", LogLevel.VERBOSE);
                continue;
            }

            finalMessage += reportDataKvp.Value.TeamName + " commented: " + reportDataKvp.Value.CommentByTheUser.ObjectValue;
        }

        Log.WriteLine("Returning: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}