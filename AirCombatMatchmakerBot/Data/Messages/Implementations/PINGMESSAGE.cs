using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord.WebSocket;

[DataContract]
public class PINGMESSAGE : BaseMessage
{
    public PINGMESSAGE()
    {
        messageName = MessageName.PINGMESSAGE;
        messageDescription = ""; 
    }

    public override string GenerateMessage()
    {
        return messageDescription;
    }

    public string GenerateRawMessage(string _input, string _embedTitle = "")
    {
        Log.WriteLine("Generating a raw message with input: " + _input +
            " and title: " + _embedTitle, LogLevel.VERBOSE);
        messageEmbedTitle = _embedTitle;
        messageDescription = _input;
        return messageDescription;
    }

    public async void PostAndDeleteTheMessage(
        DiscordSocketClient _client, InterfaceLeague _interfaceLeague, LeagueMatch _leagueMatch, InterfaceChannel _interfaceChannel)
    {
        Log.WriteLine("Starting to post " + nameof(PINGMESSAGE) + " for match:" + _leagueMatch.MatchId, LogLevel.VERBOSE);
        ulong[] playerIdsInTheMatch = _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague);
        foreach (ulong id in playerIdsInTheMatch)
        {
            messageDescription += "<@" + id.ToString() + "> ";
        }

        var newInterfaceMessage = await CreateTheMessageAndItsButtonsOnTheBaseClass(
        _client, _interfaceChannel, false, true, _interfaceLeague.DiscordLeagueReferences.LeagueCategoryId);

        newInterfaceMessage.CachedUserMessage.DeleteAsync();
    }
}