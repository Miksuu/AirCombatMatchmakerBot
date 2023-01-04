using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class MATCHSTARTMESSAGE : BaseMessage
{
    public MATCHSTARTMESSAGE()
    {
        messageName = MessageName.MATCHSTARTMESSAGE;
        showOnChannelGeneration = true;
        message = "Insert the match start message here";
    }
}