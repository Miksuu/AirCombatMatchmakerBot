using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CONFIRMMATCHRESULTBUTTON : BaseButton
{
    MatchChannelComponents mcc = new MatchChannelComponents();
    public CONFIRMMATCHRESULTBUTTON()
    {
        buttonName = ButtonName.CONFIRMMATCHRESULTBUTTON;
        thisInterfaceButton.ButtonLabel = "CONFIRM";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override async Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        mcc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);
        
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        string finalResponse = string.Empty;

        ulong componentPlayerId = _component.User.Id;

        Log.WriteLine("Activating button function: " + buttonName.ToString() + " by: " +
            componentPlayerId + " in msg: " + _interfaceMessage.MessageId, LogLevel.VERBOSE);

        var reportDataTupleWithString =
            mcc.leagueMatchCached.MatchReporting.GetTeamReportDatasOfTheMatchWithPlayerId(
            mcc.interfaceLeagueCached, mcc.leagueMatchCached, componentPlayerId);
        if (reportDataTupleWithString.Item1 == null)
        {
            Log.WriteLine(nameof(reportDataTupleWithString) + " was null!", LogLevel.CRITICAL);
            return new Response(reportDataTupleWithString.Item2, false);
        }
        if (reportDataTupleWithString.Item2 != "")
        {
            Log.WriteLine("User: " + componentPlayerId + " confirm a match on channel: " +
                _component.Channel.Id + "!", LogLevel.WARNING);
            return new Response(reportDataTupleWithString.Item2, false);
        }

        if (reportDataTupleWithString.Item1.ElementAt(0).ConfirmedMatch)
        {
            return new Response("You have already confirmed the match!", false);
        }

        reportDataTupleWithString.Item1.ElementAt(0).ConfirmedMatch = true;

        if (reportDataTupleWithString.Item1.ElementAt(1).ConfirmedMatch == true)
        {
            mcc.leagueMatchCached.MatchReporting.MatchState = MatchState.MATCHDONE;
            Log.WriteLine("Both teams are done with the reporting on match: " +
                mcc.leagueMatchCached.MatchId, LogLevel.DEBUG);
        }

        InterfaceMessage? confirmationMessage =
            Database.Instance.Categories.FindInterfaceCategoryWithId(
                _interfaceMessage.MessageCategoryId).FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.CONFIRMATIONMESSAGE);

        if (confirmationMessage == null)
        {
            string errorMsg = nameof(confirmationMessage) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        Log.WriteLine("Found: " + confirmationMessage.MessageId + " with content: " + confirmationMessage.MessageDescription, LogLevel.DEBUG);

        await confirmationMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("Reached end before the return with player id: " + componentPlayerId +
            " with finalResposne: " + finalResponse, LogLevel.DEBUG);

        return new Response(finalResponse, true);
    }
}