using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class COMMENT : BaseCommand
{
    public COMMENT()
    {
        commandName = CommandName.COMMENT;
        commandDescription = "Posts a comment about your match.";

        commandOption = new("comment", "Enter your comment here.");
    }

    public override async Task ActivateCommandFunction()
    {
    }
}