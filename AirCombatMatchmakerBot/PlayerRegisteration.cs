using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class PlayerRegisteration
{
    public static async Task<ulong> CreateANewRegisterationChannel(
        NonRegisteredUser _nonRegisteredUser)
    {
        Log.WriteLine("HANDING CHANNEL CREATION FOR CHANNEL: " + _nonRegisteredUser.discordRegisterationChannelId +
            "discordUserId: " + _nonRegisteredUser.discordUserId, LogLevel.DEBUG);

        if (BotReference.clientRef != null)
        {
            SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);
            string channelName = _nonRegisteredUser.ConstructChannelName();

            Log.WriteLine("Creating a channel named: " + channelName, LogLevel.DEBUG);

            var newChannel = await guild.CreateTextChannelAsync(
                channelName, tcp => tcp.CategoryId = 1047529896735428638);

            var channel = guild.GetChannel(newChannel.Id);

            // Place the newly created id to the object of non registered user
            _nonRegisteredUser.discordRegisterationChannelId = channel.Id;

            // Sets the players permissions to be accessible
            // (ASSUMES THAT THE CHANNEL GROUP IS PRIVATE BY DEFAULT)
            await ChannelManager.SetRegisterationChannelPermissions(_nonRegisteredUser.discordUserId, guild, channel);
            // Creates the registeration button
            BotMessaging.CreateButton(channel,
                "Click this button to register [verification process with DCS" +
                " account linking will be included later here]",
                "Register", channelName);

            Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.VERBOSE);

            //channelCreationQueue.Add(nonRegisteredUser, _userId);
            //Log.WriteLine("Added to the queue done: " + channelCreationQueue.Count, LogLevel.DEBUG);

            return newChannel.Id;
        }
        else Exceptions.BotClientRefNull();

        return 0;
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
                        // Profile found, disregard
                        if (CheckIfUserHasANonRegisterdUserProfile(user.Id) ||
                            PlayerManager.CheckIfUserIdExistsInTheDatabase(user.Id))
                        {
                            Log.WriteLine(
                                user.Username + "(" + user.Id + ") was found, disregarding", LogLevel.VERBOSE);
                        }
                        // Run handle user join that will server the same purpose than the new player joining
                        // when the bot is up
                        else
                        {
                            Log.WriteLine(user.Username + "(" + user.Id + ")" + "was not found!" +
                                " handling user join during downtime.", LogLevel.DEBUG);
                            await PlayerManager.HandleUserJoin(user);
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