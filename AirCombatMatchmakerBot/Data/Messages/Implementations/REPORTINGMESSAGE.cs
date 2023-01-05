using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class REPORTINGMESSAGE : BaseMessage
{
    public REPORTINGMESSAGE()
    {
        messageName = MessageName.REPORTINGMESSAGE;
        showOnChannelGeneration = true;
        messageButtonNames = new List<ButtonName>
        {
            ButtonName.REPORTSCOREBUTTON,
        };
        message = "Insert the reporting message here";
    }
}