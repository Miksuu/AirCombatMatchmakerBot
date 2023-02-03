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
        messageName = MessageName.MATCHFINALRESULTMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
        };
        message = "Insert the confirmation message here";
    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Starting to generate the message for the match final result", LogLevel.DEBUG);

        string finalMessage = " \n";

        var interfaceLeagueMatchTuple = Database.Instance.Leagues.FindMatchAndItsInterfaceLeagueByCategoryAndChannelId(
            messageCategoryId, messageChannelId);

        if (interfaceLeagueMatchTuple.Item1 == null || interfaceLeagueMatchTuple.Item2 == null)
        {
            Log.WriteLine(nameof(interfaceLeagueMatchTuple) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "Error, could not find the league or match";
        }

        finalMessage += "Match " + interfaceLeagueMatchTuple.Item2.MatchId + " has finished\n";

        Dictionary<int, ReportData>? matchReportingTeamIdsWithReportData =
            interfaceLeagueMatchTuple.Item2.MatchReporting.TeamIdsWithReportData;

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
            Log.WriteLine("FinalEloDelta on report message construction: " + reportDataKvp.Value.FinalEloDelta, LogLevel.DEBUG);

            finalMessage += reportDataKvp.Value.TeamName + " ";

            if (reportDataKvp.Value.FinalEloDelta > 0f)
            {
                finalMessage += EnumExtensions.GetEnumMemberAttrValue(EmojiName.RATINGUP) + " +" + reportDataKvp.Value.FinalEloDelta;
            }
            else if (reportDataKvp.Value.FinalEloDelta < 0f)
            {
                finalMessage += EnumExtensions.GetEnumMemberAttrValue(EmojiName.RATINGDOWN) + " " + reportDataKvp.Value.FinalEloDelta;
            }
            else
            {
                finalMessage += ":no_change_in_rating: " + reportDataKvp.Value.FinalEloDelta;
            }

            Log.WriteLine("finalMessage on report message construction: " + reportDataKvp.Value.FinalEloDelta, LogLevel.DEBUG);

            if (rIndex == 0) finalMessage += " | ";
            rIndex++;
        }

        finalMessage += "\nTacviews: ";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.TacviewLink.ObjectValue + "\n";
        }

        finalMessage += "\n";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            if (!reportDataKvp.Value.CommentByTheUser.FieldFilled)
            {
                Log.WriteLine(reportDataKvp.Value.TeamName + " did not comment.", LogLevel.VERBOSE);
                continue;
            }

            finalMessage += reportDataKvp.Value.TeamName + " commented: " + reportDataKvp.Value.CommentByTheUser.ObjectValue + "\n";
        }

        Log.WriteLine("Returning: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}