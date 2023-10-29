﻿using Discord;
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
        thisInterfaceMessage.MessageName = MessageName.REGISTRATIONMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.REGISTRATIONBUTTON, 1),
            });

        thisInterfaceMessage.MessageEmbedTitle = "Click this button to register!";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        /*
        if (MessageDescription == null)
        {
            Log.WriteLine("MessageDescription was null!", LogLevel.DEBUG);
            return "";
        }*/

        return Task.FromResult(thisInterfaceMessage.MessageDescription);
    }

    public override string GenerateMessageFooter()
    {
        return "";
    }
}