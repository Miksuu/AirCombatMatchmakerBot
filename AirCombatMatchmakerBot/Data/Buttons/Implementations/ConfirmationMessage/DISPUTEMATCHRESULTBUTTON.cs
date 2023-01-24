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
        buttonLabel = "DISPUTE";
        buttonStyle = ButtonStyle.Danger;
    }

    public void CreateTheButton(){}

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return "";
    }
}