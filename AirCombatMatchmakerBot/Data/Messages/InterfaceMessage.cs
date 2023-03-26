﻿using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[JsonObjectAttribute]   
public interface InterfaceMessage
{
    public MessageName MessageName { get; set; }
    public ConcurrentDictionary<ButtonName, int> MessageButtonNamesWithAmount { get; set; }
    public string Message { get; set; }
    public ulong MessageId { get; set; }
    public ulong MessageChannelId { get; set; }
    public ulong MessageCategoryId { get; set; }
    public ConcurrentBag<InterfaceButton> ButtonsInTheMessage { get; set; }

    public Task<(ulong, string)> CreateTheMessageAndItsButtonsOnTheBaseClass(
        Discord.WebSocket.SocketGuild _guild, InterfaceChannel _interfaceChannel,
        bool _displayMessage = true, ulong _leagueCategoryId = 0,
        SocketMessageComponent? _component = null, bool _ephemeral = true);
    public Task ModifyMessage(string _newContent);
    public abstract string GenerateMessage();
    public Task GenerateAndModifyTheMessage(bool _serialize = true);
    public Task<Discord.IMessage?> GetMessageById(IMessageChannel _channel);
}