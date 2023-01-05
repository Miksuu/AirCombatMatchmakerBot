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
        buttonName = ButtonName.REPORTSCOREBUTTON;
        buttonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, ulong _channelId,
        ulong _messageId, string _message, string[] _splitStrings)
    {
        return "pressed: " + _component.Id;
    }
}