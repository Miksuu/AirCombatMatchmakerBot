﻿using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class MATCHREPORTSCHANNEL : BaseChannel
{
    public MATCHREPORTSCHANNEL()
    {
        channelType = ChannelType.MATCHREPORTSCHANNEL;
        channelMessages = new ConcurrentDictionary<MessageName, bool>
        {
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
            {
                new Overwrite(
                    _guild.EveryoneRole.Id, PermissionTarget.Role,
                    new OverwritePermissions(sendMessages: PermValue.Deny, viewChannel: PermValue.Deny)),
                new Overwrite(_role.Id, PermissionTarget.Role,
                    new OverwritePermissions(viewChannel: PermValue.Allow)),
            };
    }
}