using Discord;
using Discord.WebSocket;
using System;
using System.Numerics;
using System.Threading.Tasks;

public static class DatabaseMethods
{
    public static async void RemoveUserRegisterationFromDatabase(ulong _discordId)
    {
        Log.WriteLine("Removing User Registeration Profile from the database with id: " +
            _discordId, LogLevel.DEBUG);

        var userToBeRemoved = 
            Database.Instance.NonRegisteredUsers.Find(x => x.discordUserId == _discordId);
        if (userToBeRemoved != null) 
        {
            Database.Instance.NonRegisteredUsers.Remove(userToBeRemoved);
        }
        else
        {
            Log.WriteLine("User id: " + _discordId + " was not found!", LogLevel.ERROR);
        }

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


    // Just checks if the User discord ID profile exists in the database file
    public static bool CheckIfUserIdExistsInTheDatabase(ulong _id)
    {
        return Database.Instance.PlayerData.PlayerIDs.ContainsKey(_id);
    }
}