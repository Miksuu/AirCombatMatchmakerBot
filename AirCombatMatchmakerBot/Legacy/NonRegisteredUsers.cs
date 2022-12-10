
/*
[Serializable]
public class NonRegisteredUser
{
    public ulong discordUserId { get; set; }
    public ulong discordRegisterationChannelId { get; set; }

    NonRegisteredUser() { }
    public NonRegisteredUser(ulong discordUserId)
    {
        this.discordUserId = discordUserId;
    }

    public string ConstructChannelName()
    {
        return "registration_" + discordUserId;
    }
    /*
    public static async void RemoveUserRegisterationFromDatabase(ulong _discordId)
    {
        Log.WriteLine("Removing User Registeration Profile from the database with id: " +
            _discordId, LogLevel.DEBUG);

        var userToBeRemoved = 
            Database.Instance.NonRegisteredUsers.Find(x => x.discordUserId == _discordId);

        if (userToBeRemoved == null) 
        {
            Log.WriteLine("User id: " + _discordId + " was not found!", LogLevel.ERROR);
            return;
        }

        Database.Instance.NonRegisteredUsers.Remove(userToBeRemoved);

        await SerializationManager.SerializeDB();
    }

    public static bool CheckIfUserHasANonRegisterdUserProfile(ulong _userId)
    {
        foreach (NonRegisteredUser nonRegisteredUser in Database.Instance.NonRegisteredUsers)
        {
            Log.WriteLine("Checking if " + nameof(NonRegisteredUser) + " id: " +
                nonRegisteredUser.discordUserId + " matches userId: " + _userId, LogLevel.VERBOSE);
            if (nonRegisteredUser.discordUserId == _userId)
            {
                Log.WriteLine("Player " + _userId + " found", LogLevel.VERBOSE);
                return true;
            }
        }

        Log.WriteLine("Did not find " + _userId, LogLevel.VERBOSE);
        return false;
    }*/

//static List<SocketChannel> waitingChannels = new();
/*
public static async Task FinishChannelCreationFromDelegate(SocketChannel _newChannel)
{
    Log.WriteLine("Delegate fired. " + nameof(DowntimeManager.useWaitingChannels) +
        ": " + DowntimeManager.useWaitingChannels, LogLevel.DEBUG); ;
    if (DowntimeManager.useWaitingChannels)
    {
        waitingChannels.Add(_newChannel);
    }
    else await CreateMainRegisterationChannel(_newChannel);

    await SerializationManager.SerializeDB();
}


public static async Task CreateChannelsFromWaitingChannels()
{
    Log.WriteLine("Creating channels from the waiting channels. Count: " +
        waitingChannels.Count, LogLevel.DEBUG);
    foreach (SocketChannel _newChannel in waitingChannels)
    {
        await CreateMainRegisterationChannel(_newChannel);
    }
}*/



/*
public static Task DeleteUsersRegisterationChannel(ulong _userId)
{
    var guild = BotReference.GetGuildRef();

    if (guild == null)
    {
        Exceptions.BotGuildRefNull();
        return Task.CompletedTask;
    }

    string channelToLookFor = "registration_" + _userId;
    SocketGuildChannel? foundChannel = null;

    foreach (SocketGuildChannel channel in guild.Channels)
    {
        if (channel.Name == channelToLookFor)
        {
            foundChannel = channel;
            break;
        }
    }

    if (foundChannel == null)
    {
        Log.WriteLine("Channel was not found, perhaps the user had registered " +
            "and left after? Implement a better way here.", LogLevel.WARNING);
        return Task.CompletedTask;
    }

    // If the registering channel is removed afterwards, maybe handle this better way.
    Log.WriteLine("Deleting channel: " + foundChannel.Name +
        " with ID: " + foundChannel.Id, LogLevel.DEBUG);

    // Remove the player's channel
    foundChannel.DeleteAsync();

    return Task.CompletedTask;
} 
}*/

/*
// For the new users and the terminated users
private static async Task CreateARegisterationProfileForTheUser(
    SocketGuildUser _user, string _userNameWithNickName)
{
    if (_user.IsBot)
    {
        Log.WriteLine("A bot: " + _user.Username +
            " joined the discord, disregarding the registration process", LogLevel.DEBUG);
        return;
    }

    Log.WriteLine("User: " + _user + " has joined the discord with id: " + _user.Id +
        " starting the registation process. Checking " + _userNameWithNickName, LogLevel.DEBUG);

    if (DatabaseMethods.CheckIfUserIdExistsInTheDatabase(_user.Id))
    {
        Log.WriteLine(_user.Username + " found in the database", LogLevel.DEBUG);

        await RoleManager.GrantUserAccess(_user.Id, "Member");
    }
    else
    {
        Log.WriteLine(_user.Username + " not found in the database", LogLevel.DEBUG);

        NonRegisteredUser nonRegisteredUser =
            PlayerRegisteration.CheckIfDiscordUserHasARegisterationProfileAndCreateAndReturnIt(_user.Id);

        // Creates a private channel for the user to proceed with the registration 
        if (nonRegisteredUser == null)
        {
            Log.WriteLine(nameof(nonRegisteredUser) + " was null!", LogLevel.ERROR);
            return;
        }

        await PlayerRegisteration.CreateANewRegisterationChannel(nonRegisteredUser);
    }
}
*/

/*
    public static async Task CreateMainRegisterationChannel(SocketChannel _newChannel)
    {
        Log.WriteLine("Invoked newChannel creation", LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        /*
        if (!PlayerRegisteration.channelQueue.ContainsKey(_newChannel.Id))
        {
            Log.WriteLine("Does not contain key for: " + _newChannel.Id, LogLevel.WARNING);
            return;
        }*/

//using Discord;

//var channel = guild.GetTextChannel(_newChannel.Id) as ITextChannel;

// Place the newly created id to the object of non registered user
//PlayerRegisteration.channelQueue[_newChannel.Id].discordRegisterationChannelId = _newChannel.Id;

//string channelName = PlayerRegisteration.channelQueue[_newChannel.Id].ConstructChannelName();

// Sets the players permissions to be accessible
// (ASSUMES THAT THE CHANNEL GROUP IS PRIVATE BY DEFAULT)
/*
await ChannelPermissionsManager.SetRegisterationChannelPermissions(
    PlayerRegisteration.channelQueue[_newChannel.Id].discordUserId,
    channel,
    PermValue.Allow); 
// Creates the registration button
await ButtonComponents.CreateButtonMessage(channel,
    "Click this button to register [verification process with DCS" +
    " account linking will be included later here]",
    "Register", "mainRegisteration");*/

//Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.VERBOSE);

//PlayerRegisteration.channelQueue.Remove(_newChannel.Id);
//Log.WriteLine("Removed from the queue done: " + PlayerRegisteration.channelQueue.Count, LogLevel.DEBUG);

//await SerializationManager.SerializeDB();
//   }

//public static Dictionary<ulong, NonRegisteredUser> channelQueue = new();

/*
public static async Task CreateANewRegisterationChannel(
    NonRegisteredUser _nonRegisteredUser)
{
    var guild = BotReference.GetGuildRef();

    if (guild == null)
    {
        Exceptions.BotGuildRefNull();
        return;
    }

    Log.WriteLine("HANDLING CHANNEL CREATION FOR CHANNEL: " + _nonRegisteredUser.discordRegisterationChannelId +
        "discordUserId: " + _nonRegisteredUser.discordUserId, LogLevel.DEBUG);

    string channelName = _nonRegisteredUser.ConstructChannelName();

    Log.WriteLine("Creating a channel named: " + channelName, LogLevel.DEBUG);

    var newChannel = await guild.CreateTextChannelAsync(
        channelName, tcp => tcp.CategoryId = 1047529896735428638);

    // Make the program wait that the channel is done
    channelQueue.Add(newChannel.Id, _nonRegisteredUser);
    Log.WriteLine("Added to the queue done: " + PlayerRegisteration.channelQueue.Count, LogLevel.DEBUG);
}*/

/*
public static NonRegisteredUser CheckIfDiscordUserHasARegisterationProfileAndCreateAndReturnIt(ulong _userId)
{
    foreach (NonRegisteredUser nonRegisteredUser in Database.Instance.NonRegisteredUsers)
    {
        Log.WriteLine("Checking if " + nameof(NonRegisteredUser) + " id: " +
            nonRegisteredUser.discordUserId + " matches userId: " + _userId, LogLevel.VERBOSE);
        if (nonRegisteredUser.discordUserId == _userId)
        {
            Log.WriteLine("The user was found on " + nameof(NonRegisteredUser) + " list.", LogLevel.VERBOSE);

            return nonRegisteredUser;
        }
    }

    // If the code doesn't find the profile
    Log.WriteLine("No " + _userId + " was found from the " + nameof(NonRegisteredUser) +
        " list either, adding a new one in to it", LogLevel.DEBUG);
    NonRegisteredUser nonRegisteredUserNew = new NonRegisteredUser(_userId);

    bool contains = false;

    foreach (NonRegisteredUser nonRegisteredUser in Database.Instance.NonRegisteredUsers)
    {
        if (nonRegisteredUser.discordUserId == nonRegisteredUserNew.discordUserId)
        {
            Log.WriteLine(nameof(Database.Instance.NonRegisteredUsers) +
                " already contains: " + nonRegisteredUserNew.discordUserId, LogLevel.ERROR);
            contains = true;
        }
    }

    if (!contains)
    {
        Database.Instance.NonRegisteredUsers.Add(nonRegisteredUserNew);
    }

    Log.WriteLine(nameof(NonRegisteredUser) + " count is now: " +
        Database.Instance.NonRegisteredUsers.Count, LogLevel.VERBOSE);

    return nonRegisteredUserNew;
}*/

//public static Dictionary<ulong, NonRegisteredUser> channelQueue = new();