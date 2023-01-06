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
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int> 
        {
            { ButtonName.LEAGUEREGISTRATIONBUTTON, 1 }
        };
        message = "Insert league registration message here";
    }

    public override string GenerateMessage(ulong _channelId, ulong _channelCategoryId)
    {
        return message;
    }
}