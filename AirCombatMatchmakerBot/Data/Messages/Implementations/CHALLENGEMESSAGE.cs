﻿using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class CHALLENGEMESSAGE : BaseMessage
{
    public CHALLENGEMESSAGE()
    {
        messageName = MessageName.CHALLENGEMESSAGE;
        showOnChannelGeneration = true;
        messageButtonNames = new List<ButtonName> 
        {
            ButtonName.CHALLENGEBUTTON,
        };
        message = "Insert the challenge message here";
    }
}