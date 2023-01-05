using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;

[DataContract]
public class REPORTSCOREBUTTON : BaseButton
{
    public REPORTSCOREBUTTON()
    {
        buttonName = ButtonName.REGISTRATIONBUTTON;
        buttonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, string _splitString,
        ulong _channelId, ulong _messageId, string _message)
    {
    }
}