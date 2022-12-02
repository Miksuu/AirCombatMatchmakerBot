using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class PlayerRegisteration
{
    public static async Task<ulong> CreateANewRegisterationChannel(
        ulong _userId, SocketGuild _guild, ulong _forCategory)
    {
        NonRegisteredUser nonRegisteredUser = CheckIfDiscordUserHasARegisterationProfileAndCreateAndReturnIt(_userId);

        string channelName = nonRegisteredUser.ConstructChannelName();

        //if (Database.Instance.NonRegisteredUsers.Contains)

        // Try to find if the channel exists already (should not be the case)
        var channel = ChannelManager.FindChannel(_guild, channelName);
        if (channel == null)
        {
            if (BotReference.clientRef != null)
            {
                Log.WriteLine("Creating a channel named: " + channelName, LogLevel.DEBUG);

                var newChannel = await _guild.CreateTextChannelAsync(
                    channelName, tcp => tcp.CategoryId = _forCategory);

                // Place the newly created id to the object of non registered user
                nonRegisteredUser.discordRegisterationChannelId = newChannel.Id;

                await SerializationManager.SerializeDB();

                Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.VERBOSE);

                //channelCreationQueue.Add(nonRegisteredUser, _userId);
                //Log.WriteLine("Added to the queue done: " + channelCreationQueue.Count, LogLevel.DEBUG);

                return newChannel.Id;
            }
            else Exceptions.BotClientRefNull();
        }
        // If the channel exists showhow, give the permissions manually still.
        else
        {
            Log.WriteLine("This channel already exists! " +
                "(should not be the case). Giving permissions anyway.", LogLevel.ERROR);
            //await CreateANewRegisterationChannelManually(nonRegisteredUser, _userId, _guild);
        }
        return 0;
    }

    public static async Task CreateANewRegisterationChannelManually(
        NonRegisteredUser _nonRegisteredUser, SocketGuildUser _user, SocketGuild _guild)
    {
        string channelName = _nonRegisteredUser.ConstructChannelName();

        Log.WriteLine("Starting the creation of registration channelName: " + channelName +
            " for user: " + _user.Username, LogLevel.DEBUG);
        //var channel = ChannelManager.FindChannel(_guild, channelName);
        //channelCreationQueue.Add(_nonRegisteredUser, _userId);

        await ChannelManager.HandleChannelCreationManually(_nonRegisteredUser);
    }

    public static async Task CheckForUsersThatAreNotRegisteredAfterDowntime()
    {
        Log.WriteLine("Checking for Users that entered the discord during " +
            "the bot's downtime and that are not on the registeration list", LogLevel.DEBUG);

        if (BotReference.clientRef != null)
        {
            var guild = BotReference.clientRef.GetGuild(BotReference.GuildID);
            Log.WriteLine("USERS COUNT: " + guild.Users.Count(), LogLevel.DEBUG);

            // Loop through the users
            foreach (var user in guild.Users)
            {
                if (user != null)
                {
                    if (!user.IsBot)
                    {
                        Log.WriteLine("Checking " + user.Username + " aka "
                            + PlayerManager.CheckIfNickNameIsEmptyAndReturnUsername(user.Id) +
                            " (" + user.Id + ")", LogLevel.DEBUG);

                        if (PlayerManager.CheckIfUserIdExistsInTheDatabase(user.Id))
                        {
                            Log.WriteLine(user.Username + " found in the database", LogLevel.DEBUG);

                            // TODO: Handle recovery of the access to the user.
                        }
                        else
                        {
                            Log.WriteLine(user.Username + " not found in the database", LogLevel.DEBUG);

                            NonRegisteredUser nonRegisteredUser =
                                CheckIfDiscordUserHasARegisterationProfileAndCreateAndReturnIt(user.Id);

                            if (!nonRegisteredUser.channelHasBeenCreated)
                            {
                                if (nonRegisteredUser != null)
                                {
                                    await ChannelManager.HandleChannelCreationManually(nonRegisteredUser);
                                    nonRegisteredUser.channelHasBeenCreated = true;

                                    SerializationManager.SerializeDB();
                                }
                                else
                                {
                                    Log.WriteLine(nameof(nonRegisteredUser) + " was null!", LogLevel.ERROR);
                                }
                            }
                            else
                            {
                                Log.WriteLine("Channel had been done for " + user.Id + " already.", LogLevel.VERBOSE);
                            }
                        }
                    }
                    else Log.WriteLine("User " + user.Username + " is a bot, disregarding", LogLevel.VERBOSE);
                }
                else Log.WriteLine("User is null!", LogLevel.CRITICAL);
            }
        }
        else Exceptions.BotClientRefNull();

        await SerializationManager.SerializeDB();
    }

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

        Database.Instance.NonRegisteredUsers.Add(nonRegisteredUserNew);
        Log.WriteLine(nameof(NonRegisteredUser) + " count is now: " +
            Database.Instance.NonRegisteredUsers.Count, LogLevel.VERBOSE);

        return nonRegisteredUserNew;

    }
}