using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class CONFIRMATIONMESSAGE : BaseMessage
{
    public CONFIRMATIONMESSAGE()
    {
        messageName = MessageName.CONFIRMATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
            { ButtonName.CONFIRMMATCHRESULTBUTTON, 1 },
            { ButtonName.MODIFYMATCHRESULTBUTTON, 1 },
            { ButtonName.DISPUTEMATCHRESULTBUTTON, 1 }
        };
        message = "You can either Confirm, Modify or Dispute the result below.";
    }

    public override string GenerateMessage()
    {
        return message;
    }
}