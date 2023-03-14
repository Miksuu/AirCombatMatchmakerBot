using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class MODIFYMATCHRESULTBUTTON : BaseButton
{
    public MODIFYMATCHRESULTBUTTON()
    {
        buttonName = ButtonName.MODIFYMATCHRESULTBUTTON;
        buttonLabel = "MODIFY";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    public async override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        var leagueInterfaceAndMatchTuple =
            Database.Instance.Leagues.FindLeagueInterfaceAndLeagueMatchWithChannelId(
                _component.Channel.Id);
        if (leagueInterfaceAndMatchTuple.Item1 == null || leagueInterfaceAndMatchTuple.Item2 == null)
        {
            Log.WriteLine(nameof(leagueInterfaceAndMatchTuple) + " was null!", LogLevel.CRITICAL);
            return ("", false);
        }

        var team = leagueInterfaceAndMatchTuple.Item1.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(
            _component.User.Id);
        if (team == null || team.TeamId == 0)
        {
            Log.WriteLine(nameof(team) + " was null!", LogLevel.CRITICAL);
            return ("Team was null!", false);
        }

        var teamReportData = leagueInterfaceAndMatchTuple.Item2.MatchReporting.TeamIdsWithReportData.FirstOrDefault(
            x => team.TeamId == x.Key).Value;

        if (teamReportData.ConfirmedMatch)
        {
            Log.WriteLine("Team: " + team.TeamName + " had already confirmed the match!", LogLevel.VERBOSE);
            return ("You have already confirmed the match!", false);
        }

        Log.WriteLine("Starting to reset report data", LogLevel.VERBOSE);

        // Copy pasta from MatchReporting.cs, mmaybe replace to method
        InterfaceChannel interfaceChannelToDeleteTheMessageIn = 
            leagueInterfaceAndMatchTuple.Item1.FindLeaguesInterfaceCategory(
                ).FindInterfaceChannelWithIdInTheCategory(
                    _component.Channel.Id);
        if (interfaceChannelToDeleteTheMessageIn == null)
        {
            Log.WriteLine(nameof(interfaceChannelToDeleteTheMessageIn) + " was null, with: " +
                _component.Channel.Id, LogLevel.CRITICAL);
            return (nameof(interfaceChannelToDeleteTheMessageIn) + " was null", false);
        }

        await interfaceChannelToDeleteTheMessageIn.DeleteMessagesInAChannelWithMessageName(MessageName.CONFIRMATIONMESSAGE);

        /*
        var msgID = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                _component.Channel.Id).FindInterfaceMessageWithNameInTheChannel(MessageName.CONFIRMATIONMESSAGE).MessageId;
        */

        // Resets the reported score of the modifying player
        teamReportData.ReportedScore.ObjectValue = "";
        teamReportData.ReportedScore.FieldFilled = false;

        // Loop through the teams and set confirmed match to false
        foreach (var reportDataKvp in leagueInterfaceAndMatchTuple.Item2.MatchReporting.TeamIdsWithReportData)
        {
            Log.WriteLine("Resetting " + reportDataKvp.Key + " 's confirmedMatch bool: "
                + reportDataKvp.Value.ConfirmedMatch + " to false", LogLevel.VERBOSE);
            reportDataKvp.Value.ConfirmedMatch = false;
        }

        leagueInterfaceAndMatchTuple.Item2.MatchReporting.ShowingConfirmationMessage = false;

        return ("", true);

    }
}