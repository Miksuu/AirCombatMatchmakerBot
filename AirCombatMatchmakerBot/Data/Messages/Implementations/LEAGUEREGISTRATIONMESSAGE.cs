using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class LEAGUEREGISTRATIONMESSAGE : BaseMessage
{
    public LEAGUEREGISTRATIONMESSAGE()
    {
        messageName = MessageName.LEAGUEREGISTRATIONMESSAGE;
        showOnChannelGeneration = true;
        messageButtonNames = new List<ButtonName> 
        {
            ButtonName.LEAGUEREGISTRATIONBUTTON,
        };
        message = "Click this button to register!";
    }

    /*
    public void CreateTheMessageAndItsButtonsOnTheInheritedClass(
        SocketGuild _guild, ulong _channelId)
    {
        base.CreateTheMessageAndItsButtonsOnTheBaseClass(_guild, _channelId);
    }*/
}