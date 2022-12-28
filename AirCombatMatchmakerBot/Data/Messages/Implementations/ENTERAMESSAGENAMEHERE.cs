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
        showOnChannelGeneration = true;
        messageButtonNames = new List<ButtonName> 
        {
            
        };
    }

    public override void TempMethod()
    {

    }
}