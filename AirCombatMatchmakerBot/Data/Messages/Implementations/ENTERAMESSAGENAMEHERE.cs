using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class ENTERAMESSAGENAMEHERE : BaseMessage
{
    public ENTERAMESSAGENAMEHERE()
    {
        messageName = MessageName.ENTERAMESSAGENAMEHERE;
    }

    public override void TempMethod()
    {

    }
}