using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class CONFIRMATIONMESSAGE : BaseMessage
{
    public CONFIRMATIONMESSAGE()
    {
        messageName = MessageName.CONFIRMATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
            { ButtonName.CONFIRMMATCHRESULTBUTTON, 1 },
            { ButtonName.MODIFYMATCHRESULTBUTTON, 1 },
            { ButtonName.DISPUTEMATCHRESULTBUTTON, 1 }
        };
        message = "Insert the confirmation message here";
    }

    public override string GenerateMessage()
    {
        string finalMessage = "FINAL REPORTING RESULTS:\n";

        // Generate the message here

        /*
        Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            messageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                messageChannelId).FindInterfaceMessageWithNameInTheChannel(MessageName.CONFIRMATIONMESSAGE); */

        var matchReportingTeamIdsWithReportData = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(
            messageCategoryId).LeagueData.Matches.FindLeagueMatchByTheChannelId(
                messageChannelId).MatchReporting.TeamIdsWithReportData;

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.TeamName + ": " + reportDataKvp.Value.ReportedScore.ObjectValue + "\n";
        }

        finalMessage += "\nYou can either Confirm, Modify or Dispute the result below.";

        Log.WriteLine("Returning: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}