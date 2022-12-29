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
        message = "Insert league registration message here";
    }
}