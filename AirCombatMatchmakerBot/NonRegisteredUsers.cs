﻿using Discord;
using Discord.WebSocket;
using System;
using System.Numerics;
using System.Threading.Tasks;

[Serializable]
public class NonRegisteredUser
{
    public ulong discordUserId { get; set; }
    public ulong discordRegisterationChannelId { get; set; }
    
    public bool channelHasBeenCreated { get; set; }

    NonRegisteredUser() { }
    public NonRegisteredUser(ulong discordUserId)
    {
        this.discordUserId = discordUserId;
    }

    public string ConstructChannelName()
    {
        return "registeration_" + discordUserId;
    }
}