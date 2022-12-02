using Discord;
using Discord.WebSocket;
using System;
using System.Numerics;
using System.Threading.Tasks;

public static class DatabaseMethods
{
    public static void RemoveUserRegisterationFromDatabase(ulong _discordId)
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

        SerializationManager.SerializeDB();
    }
}