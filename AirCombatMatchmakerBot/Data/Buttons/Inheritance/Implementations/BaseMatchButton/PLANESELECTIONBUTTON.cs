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
        buttonLabel = "Plane";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = false;
    }

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return Task.FromResult(("", true));
    }
}