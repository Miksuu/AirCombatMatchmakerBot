using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CONFIRMMATCHRESULTBUTTON : BaseButton
{
    MatchChannelComponents mcc;
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
        mcc = new MatchChannelComponents(_interfaceMessage);
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

        List<ReportData> reportDataTupleWithString;

        try
        {
            reportDataTupleWithString =
                mcc.leagueMatchCached.MatchReporting.GetTeamReportDatasOfTheMatchWithPlayerId(
                    mcc.interfaceLeagueCached, mcc.leagueMatchCached, componentPlayerId);
        }
        catch (Exception ex)
        {
            return new Response(ex.Message, false);
        }

        if (reportDataTupleWithString.ElementAt(0).ConfirmedMatch)
        {
            return new Response("You have already confirmed the match!", false);
        }

        reportDataTupleWithString.ElementAt(0).ConfirmedMatch = true;

        if (reportDataTupleWithString.ElementAt(1).ConfirmedMatch == true)
        {
            //mcc.leagueMatchCached.MatchReporting.MatchState = MatchState.MATCHDONE;
            Log.WriteLine("Both teams are done with the reporting on match: " +
                mcc.leagueMatchCached.MatchId, LogLevel.DEBUG);
        }


        InterfaceMessage confirmationMessage;
        try
        {
            confirmationMessage =
                Database.Instance.Categories.FindInterfaceCategoryWithId(
                    _interfaceMessage.MessageCategoryId).FindInterfaceChannelWithIdInTheCategory(
                        _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.CONFIRMATIONMESSAGE);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }

        Log.WriteLine("Found: " + confirmationMessage.MessageId + " with content: " + 
            confirmationMessage.MessageDescription, LogLevel.DEBUG);

        await confirmationMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("Reached end before the return with player id: " + componentPlayerId +
            " with finalResposne: " + finalResponse, LogLevel.DEBUG);

        return new Response(finalResponse, true);
    }
}