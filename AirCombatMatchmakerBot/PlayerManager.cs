using Discord;
using Discord.WebSocket;

public static class PlayerManager
{
    public static async Task HandleUserJoin(SocketGuildUser _user)
    {
        if (!_user.IsBot)
        {
            Log.WriteLine("User: " + _user + " has joined the discord with id: " + _user.Id +
                " starting the registation process", LogLevel.DEBUG);

            if (BotReference.clientRef != null)
            {
                SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);

                // Creates a private channel for the user to proceed with the registeration 
                await PlayerRegisteration.CreateANewRegisterationChannel(_user, guild, 1047529896735428638); // category ID
            }
            else Exceptions.BotClientRefNull();
        }
        else
        {
            Log.WriteLine("A bot: " + _user.Nickname +
                " joined the discord, disregarding the registeration process", LogLevel.WARNING);
        }
    }

    public static async Task HandlePlayerLeave(SocketGuild _guild, SocketUser _user)
    {
        Log.WriteLine(_user.Username + " (" + _user.Id +
            ") bailed out! Handling deleting registeration channels etc.", LogLevel.DEBUG);

        await ChannelManager.DeleteUsersChannelsOnLeave(_guild, _user);
    }

    public static async Task AddNewPlayer(SocketMessageComponent _component)
    {
        ulong playerId = _component.User.Id;

        var nickName = GetSocketGuildUserById(playerId).Nickname;

        Log.WriteLine("Adding a new player: " + nickName + " (" + playerId + ").", LogLevel.DEBUG);

        // Checks if the player is already in the databse, just in case
        if (!Database.Instance.PlayerData.PlayerIDs.ContainsKey(playerId))
        {
            Database.Instance.PlayerData.PlayerIDs.Add(playerId, new Player(playerId, nickName));
            await SerializationManager.SerializeDB();
        }
        else
        {
            Log.WriteLine("Tried to add a player that was already in the database!" ,LogLevel.WARNING);
        }
    }

    public static async Task HandleGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> before, SocketGuildUser _socketGuildUserAfter)
    {
        var playerValue = Database.Instance.PlayerData.PlayerIDs.First(x => x.Key == _socketGuildUserAfter.Id).Value;

        Log.WriteLine("Updating user: " + _socketGuildUserAfter.Username + " (" + _socketGuildUserAfter.Id + ")" + 
            " | name: " + playerValue.playerNickName + " -> " + _socketGuildUserAfter.Nickname, LogLevel.DEBUG);

        if (playerValue != null)
        {
            playerValue.playerNickName = _socketGuildUserAfter.Nickname;
        }
        else Log.WriteLine("Trying to update " + _socketGuildUserAfter.Username + "'s profile, no valid player found (not registed?) ", LogLevel.DEBUG);
    }

    public static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }

    public static SocketGuildUser GetSocketGuildUserById(ulong _id)
    {
        return (SocketGuildUser)BotReference.clientRef.GetUserAsync(_id).Result;
    }
}