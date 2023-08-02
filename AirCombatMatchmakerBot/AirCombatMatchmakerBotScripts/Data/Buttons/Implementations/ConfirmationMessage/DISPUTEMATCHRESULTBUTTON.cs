using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class DISPUTEMATCHRESULTBUTTON : BaseButton
{
    public DISPUTEMATCHRESULTBUTTON()
    {
        buttonName = ButtonName.DISPUTEMATCHRESULTBUTTON;
        thisInterfaceButton.ButtonLabel = "DISPUTE";
        buttonStyle = ButtonStyle.Danger;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return Task.FromResult(new Response("Not implemented yet!", false));
    }
}