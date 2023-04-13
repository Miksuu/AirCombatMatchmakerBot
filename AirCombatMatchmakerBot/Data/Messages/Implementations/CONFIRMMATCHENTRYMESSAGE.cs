using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Collections.Concurrent;

[DataContract]
public class CONFIRMMATCHENTRYMESSAGE : BaseMessage
{
    public CONFIRMMATCHENTRYMESSAGE()
    {
        messageName = MessageName.CONFIRMMATCHENTRYMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.CONFIRMMATCHENTRYBUTTON, 1),
            });

        messageEmbedTitle = "This message confirms the match entry [add more detailed message here]";
    }

    public override string GenerateMessage()
    {
        return messageDescription;
    }
}