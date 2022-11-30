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

    public static Task AddNewPlayer(ulong _playerID, string _playerName)
    {
        Log.WriteLine("Adding a new player: " + _playerName + " (" + _playerID + ").", LogLevel.DEBUG);

        // Checks if the player is already in the databse, just in case
        if (!Database.Instance.PlayerData.PlayerIDs.ContainsKey(_playerID))
        {
            Database.Instance.PlayerData.PlayerIDs.Add(_playerID, new Player(_playerID, _playerName));
            SerializationManager.SerializeDB();
        }
        else
        {
            Log.WriteLine("Tried to add a player that was already in the database!" ,LogLevel.WARNING);
        }




        return Task.CompletedTask;
    }
}