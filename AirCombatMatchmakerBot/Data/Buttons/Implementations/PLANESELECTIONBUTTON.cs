using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class PLANESELECTIONBUTTON : BaseButton
{
    public PLANESELECTIONBUTTON()
    {
        buttonName = ButtonName.PLANESELECTIONBUTTON;
        thisInterfaceButton.ButtonLabel = "Plane";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = false;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return Task.FromResult(new Response("", true));
    }
}