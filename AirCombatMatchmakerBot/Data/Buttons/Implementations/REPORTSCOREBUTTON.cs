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

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        InterfaceMessage reportingStatusMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.REPORTINGSTATUSMESSAGE);

        string[] splitStrings = buttonCustomId.Split('_');

        string finalResponse = "Something went wrong with the reporting the match result of: " +
            splitStrings[1] + ". An admin has been informed.";

        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + reportingStatusMessage.MessageChannelId + 
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        /*
        foreach (var item in splitStrings)
        {
            Log.WriteLine(item, LogLevel.DEBUG);
        }*/

        // Find the league with the cached category ID
        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(buttonCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.CRITICAL);
            return "";
        }

        LeagueMatch? foundMatch = 
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(
                reportingStatusMessage.MessageChannelId);
        if (foundMatch == null)
        {
            Log.WriteLine("Match with: " + reportingStatusMessage.MessageChannelId +
                " was not found.", LogLevel.CRITICAL);
            return finalResponse;
        }

        finalResponse = foundMatch.MatchReporting.ReportMatchResult(
            interfaceLeague, playerId, playerReportedResult, reportingStatusMessage).Result;

        Log.WriteLine("Reached end before the return with player id: " + playerId, LogLevel.DEBUG);

        return finalResponse;
    }
}