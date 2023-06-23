using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class MATCHFINALRESULTMESSAGE : BaseMessage
{
    MatchChannelComponents mcc;
    public MATCHFINALRESULTMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHFINALRESULTMESSAGE;
        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>
        {
        };
        thisInterfaceMessage.MessageDescription = "Insert the confirmation message here";
    }

    public string AlternativeMessage
    {
        get => alternativeMessage.GetValue();
        set => alternativeMessage.SetValue(value);
    }

    [DataMember] protected logString alternativeMessage = new logString();

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage()
    {
        Log.WriteLine("Starting to generate the message for the match final result", LogLevel.VERBOSE);

        mcc = new MatchChannelComponents(thisInterfaceMessage.MessageChannelId);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult(errorMsg);
        }

        string finalMessage = string.Empty;

        thisInterfaceMessage.MessageEmbedTitle = "Match " + mcc.leagueMatchCached.MatchId + " has finished\n";

        Dictionary<int, ReportData> matchReportingTeamIdsWithReportData =
            mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData.ToDictionary(x => x.Key, x => x.Value);

        finalMessage += "\nPlayers: ";

        // Shows the team names
        int pIndex = 0;
        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            var planeReportObject = reportDataKvp.Value.ReportingObjects.FirstOrDefault(
                x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.PLAYERPLANE) as PLAYERPLANE;

            //Log.WriteLine("Before getting team planes", LogLevel.DEBUG);
            finalMessage += reportDataKvp.Value.TeamName + " (" + planeReportObject.GetTeamPlanes() + ")";
            //Log.WriteLine("After getting team planes", LogLevel.DEBUG);
            if (pIndex == 0) finalMessage += " vs ";
            pIndex++;
        }

        finalMessage += "\nScore: ";

        // Shows the final score
        int sIndex = 0;
        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            var baseReportingObject = reportDataKvp.Value.ReportingObjects.FirstOrDefault(
                x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.REPORTEDSCORE) as BaseReportingObject;

            var interfaceObject = (InterfaceReportingObject) baseReportingObject;

            finalMessage += interfaceObject.ObjectValue;
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
            var baseReportingObject = reportDataKvp.Value.ReportingObjects.FirstOrDefault(
                x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.COMMENTBYTHEUSER) as BaseReportingObject;

            var interfaceObject = (InterfaceReportingObject)baseReportingObject;

            if (interfaceObject.CurrentStatus == EmojiName.YELLOWSQUARE)
            {
                Log.WriteLine(reportDataKvp.Value.TeamName + " did not comment.", LogLevel.VERBOSE);
                continue;
            }

            finalMessage += reportDataKvp.Value.TeamName + " commented: " + interfaceObject.ObjectValue + "\n";
        }

        // Cache the finalMessage on to the alternativeMessage form for match results page where tacviews have link buttons
        AlternativeMessage = finalMessage;

        finalMessage += "\nTacviews: ";

        foreach (var reportDataKvp in matchReportingTeamIdsWithReportData)
        {
            var baseReportingObject = reportDataKvp.Value.ReportingObjects.FirstOrDefault(
                x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.TACVIEWLINK) as BaseReportingObject;

            var interfaceObject = (InterfaceReportingObject)baseReportingObject;

            finalMessage += interfaceObject.ObjectValue + "\n";
        }

        Log.WriteLine("Returning: " + finalMessage, LogLevel.DEBUG);

        return Task.FromResult(finalMessage);
    }
}