﻿using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUESTATUS : BaseChannel
{
    public LEAGUESTATUS()
    {
        channelType = ChannelType.LEAGUESTATUS;
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
        {
        };
    }

    /*
    public Task PostChannelMessages()
    {
        return Task.CompletedTask;
    }*/
}