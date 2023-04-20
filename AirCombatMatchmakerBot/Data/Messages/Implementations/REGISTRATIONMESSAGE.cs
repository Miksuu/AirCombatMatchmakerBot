using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Collections.Concurrent;

[DataContract]
public class REGISTRATIONMESSAGE : BaseMessage
{
    public REGISTRATIONMESSAGE()
    {
        messageName = MessageName.REGISTRATIONMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.REGISTRATIONBUTTON, 1),
            });

        messageEmbedTitle = "Click this button to register!";
    }

    protected override void GenerateCustomMessageButtonNamesWithAmount()
    {

    }

    public override string GenerateMessage()
    {
        return messageDescription;
    }
}