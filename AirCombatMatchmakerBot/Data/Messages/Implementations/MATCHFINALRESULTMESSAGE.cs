using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class MATCHFINALRESULTMESSAGE : BaseMessage
{
    public MATCHFINALRESULTMESSAGE()
    {
        messageName = MessageName.MATCHFINALRESULTMESSAGE;
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>
        {
        };
        messageDescription = "Insert the confirmation message here";
    }

    public string AlternativeMessage
    {
        get
        {
            Log.WriteLine("Getting " + nameof(alternativeMessage) + ": " + alternativeMessage, LogLevel.VERBOSE);
            return alternativeMessage;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(alternativeMessage) + alternativeMessage
                + " to: " + value, LogLevel.VERBOSE);
            alternativeMessage = value;
        }
    }

    [DataMember] protected string alternativeMessage = "";

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Starting to generate the message for the match final result", LogLevel.VERBOSE);

        string finalMessage = string.Empty;

        var interfaceLeagueMatchTuple = Database.Instance.Leagues.FindMatchAndItsInterfaceLeagueByCategoryAndChannelId(
            thisInterfaceMessage.MessageCategoryId, thisInterfaceMessage.MessageChannelId);

        if (interfaceLeagueMatchTuple.Item1 == null || interfaceLeagueMatchTuple.Item2 == null)
        {
            Log.WriteLine(nameof(interfaceLeagueMatchTuple) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "Error, could not find the league or match";
        }

        messageEmbedTitle = "Match " + interfaceLeagueMatchTuple.Item2.MatchId + " has finished\n";

        ConcurrentDictionary<int, ReportData>? matchReportingTeamIdsWithReportData =
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

        finalMessage += "\n";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            if (reportDataKvp.Value.CommentByTheUser.CurrentStatus == EmojiName.YELLOWSQUARE)
            {
                Log.WriteLine(reportDataKvp.Value.TeamName + " did not comment.", LogLevel.VERBOSE);
                continue;
            }

            finalMessage += reportDataKvp.Value.TeamName + " commented: " + reportDataKvp.Value.CommentByTheUser.ObjectValue + "\n";
        }

        // Cache the finalMessage on to the alternativeMessage form for match results page where tacviews have link buttons
        alternativeMessage = finalMessage;

        finalMessage += "\nTacviews: ";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            finalMessage += reportDataKvp.Value.TacviewLink.ObjectValue + "\n";
        }

        Log.WriteLine("Returning: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}