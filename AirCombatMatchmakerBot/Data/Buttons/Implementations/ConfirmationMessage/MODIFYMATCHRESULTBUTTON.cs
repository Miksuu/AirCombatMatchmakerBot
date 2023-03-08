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
    }

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return Task.FromResult(("Not implemented yet!", false));
    }
}