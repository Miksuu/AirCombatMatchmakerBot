using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class CONFIRMMATCHRESULTBUTTON : BaseButton
{
    public CONFIRMMATCHRESULTBUTTON()
    {
        buttonName = ButtonName.CONFIRMMATCHRESULTBUTTON;
        buttonLabel = "CONFIRM";
        buttonStyle = ButtonStyle.Success;
    }

    public void CreateTheButton(){}

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        return "";
    }
}