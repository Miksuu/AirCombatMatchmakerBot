using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class CONFIRMMATCHENTRYBUTTON : BaseMatchButton
{
    public CONFIRMMATCHENTRYBUTTON()
    {
        buttonName = ButtonName.CONFIRMMATCHENTRYBUTTON;
        buttonLabel = "CONFIRM";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = true;
    }

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return Task.FromResult(("Not implemented yet!", false));
    }
}