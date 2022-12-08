public static class PlayerRegisteration
{
    public static Dictionary<ulong, NonRegisteredUser> channelQueue = new();

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
    }

    /*
    public static async Task CreateANewRegisterationChannelManually(
        NonRegisteredUser _nonRegisteredUser)
    {
        string channelName = _nonRegisteredUser.ConstructChannelName();

        Log.WriteLine("Starting the creation of registration channelName: " + channelName +
            " for userId: " + _nonRegisteredUser.discordUserId, LogLevel.DEBUG);
        //var channel = ChannelManager.FindChannel(_guild, channelName);
        //channelCreationQueue.Add(_nonRegisteredUser, _userId);

        await ChannelManager.HandleChannelCreationManually(_nonRegisteredUser);
    } */

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
    }
}