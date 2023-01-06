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
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
            { ButtonName.REPORTSCOREBUTTON, 4 }
        };
        message = "Insert the reporting message here";
    }

    public override string GenerateMessage(ulong _channelId, ulong _channelCategoryId)
    {
        return message;
    }
}