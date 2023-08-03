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
    MatchChannelComponents mcc;
    public REPORTSCOREBUTTON()
    {
        buttonName = ButtonName.REPORTSCOREBUTTON;
        thisInterfaceButton.ButtonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _channelCategoryId)
    {
        thisInterfaceButton.ButtonLabel = _buttonIndex.ToString();
        Log.WriteLine("is: " + nameof(buttonName) +
            " set label to: " + thisInterfaceButton.ButtonLabel);

        return "";
    }

    public async override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        try
        {
            string[] splitStrings = thisInterfaceButton.ButtonCustomId.Split('_');
            ulong playerId = _component.User.Id;
            int playerReportedResult = int.Parse(splitStrings[1]);
            InterfaceMessage reportingStatusMessage =
                DiscordBotDatabase.Instance.Categories.FindInterfaceCategoryWithCategoryId(
                    _interfaceMessage.MessageCategoryId).FindInterfaceChannelWithIdInTheCategory(
                        _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.REPORTINGSTATUSMESSAGE);


            Log.WriteLine("Pressed by: " + playerId + " in: " + reportingStatusMessage.MessageChannelId +
                " with label int: " + playerReportedResult + " in category: " +
                thisInterfaceButton.ButtonCategoryId, LogLevel.DEBUG);

            mcc = new MatchChannelComponents(_interfaceMessage);

            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                    nameof(mcc.leagueMatchCached) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.ERROR);
                return new Response(errorMsg, false);
            }

            Response response = mcc.leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
                playerId, playerReportedResult.ToString(),
                TypeOfTheReportingObject.REPORTEDSCORE,
                _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId).Result;

            if (!response.serialize)
            {
                return response;
            }

            response = await mcc.leagueMatchCached.MatchReporting.PrepareFinalMatchResult(
                playerId, _interfaceMessage.MessageChannelId);

            if (!response.serialize)
            {
                return response;
            }

            Log.WriteLine("Reached end before the return with player id: " +
                playerId + " with response:" + response.responseString, LogLevel.DEBUG);

            return response;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }
}