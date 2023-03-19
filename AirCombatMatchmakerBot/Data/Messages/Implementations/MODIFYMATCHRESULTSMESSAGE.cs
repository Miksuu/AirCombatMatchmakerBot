using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class MODIFYMATCHRESULTSMESSAGE : BaseMessage
{
    public MODIFYMATCHRESULTSMESSAGE()
    {
        messageName = MessageName.MODIFYMATCHRESULTSMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
            { ButtonName.MODIFYSCOREBUTTON, 4 }
        };
        message = "Modify your results here: ";
    }

    public override string GenerateMessage()
    {
        return message;
    }
}