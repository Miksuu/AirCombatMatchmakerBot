using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class REGISTRATIONMESSAGE : BaseMessage
{
    public REGISTRATIONMESSAGE()
    {
        messageName = MessageName.REGISTRATIONMESSAGE;
        showOnChannelGeneration = true;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int> 
        {
            { ButtonName.REGISTRATIONBUTTON, 1 },
        };
        message = "Click this button to register!";
    }

    public override string GenerateMessage(ulong _channelId, ulong _channelCategoryId)
    {
        return message;
    }
}