﻿using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

public static class SerializationManager
{
    static string dbPath = @"C:\AirCombatMatchmakerBot\Data\database.json";

    // Run this everytime when relevant data needs to be saved to the database
    public static async Task SerializeDB(bool _circularDependency = false)
    {
        Log.WriteLine("SERIALIZING DB", LogLevel.VERBOSE);

        if (!_circularDependency)
        {
            await SerializeUsersOnTheServer();
        }
        
        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
        //serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
        serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
        serializer.MaxDepth = 64;

        //Console.WriteLine("SERIALIZATION, STARTING for " + Database.Instance.Tournaments.Count);

        using (StreamWriter sw = new StreamWriter(dbPath))
        using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
        {
            //string jsonStringSer = JsonSerializer.Serialize<Database>(Database.Instance, options);
            serializer.Serialize(writer, Database.Instance, typeof(Database));
            writer.Close();
            sw.Close();
        };

        Log.WriteLine("DB SERIALIZATION DONE!", LogLevel.VERBOSE);
    }

    public static async Task SerializeUsersOnTheServer()
    {
        Log.WriteLine("Serializing users on the server", LogLevel.VERBOSE);

        if (BotReference.clientRef != null)
        {
            var guild = BotReference.GetGuildRef();

            if (guild != null)
            {
                foreach (SocketGuildUser user in guild.Users)
                {
                    if (user != null)
                    {
                        // Move to method
                        string userString = user.Username + " (" + user.Id + ")";
                        Log.WriteLine("Looping on: " + userString, LogLevel.VERBOSE);

                        if (!user.IsBot)
                        {
                            AddUserIdToCachedList(userString, user.Id);
                        }
                        else
                        {
                            Log.WriteLine(userString + " is a bot, disregarding.", LogLevel.VERBOSE);
                        }
                    }
                    else Log.WriteLine("User was null!", LogLevel.CRITICAL);
                }
                Log.WriteLine("Done looping through current users.", LogLevel.VERBOSE);

            } else Exceptions.BotGuildRefNull();
        }
        else Exceptions.BotClientRefNull();

        await SerializeDB(true);

        Log.WriteLine("User serialization done on the server", LogLevel.VERBOSE);
    }

    public static Task DeSerializeDB()
    {
        string jsonString = "";

        using (var fs = File.Open(dbPath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader r = new StreamReader(fs))
            {
                jsonString = r.ReadToEnd();
                Database.Instance = JsonConvert.DeserializeObject<Database>(jsonString);
                r.Close();
            }
            fs.Close();
        }

        Log.WriteLine("DE-SERIALIZING JSON: " + jsonString, LogLevel.VERBOSE);
        
        return Task.CompletedTask;
    }

    public static void AddUserIdToCachedList(string _userString, ulong _userId)
    {
        Log.WriteLine("Adding " + _userString + " to the cache list", LogLevel.VERBOSE);

        if (!Database.Instance.cachedUserIDs.Contains(_userId))
        {
            Database.Instance.cachedUserIDs.Add(_userId);
            Log.WriteLine("Added " + _userString +
                " to cached users list.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("User " + _userString + " is already on the list", LogLevel.VERBOSE);
        }
    }

    public static async void RemoveUserFromTheCachedList(string _userName, ulong _userId)
    {
        Log.WriteLine("Removing " + _userName + "(" + _userId + ")" + " from the cache list", LogLevel.VERBOSE);

        if (Database.Instance.cachedUserIDs.Contains(_userId))
        {
            Database.Instance.cachedUserIDs.Remove(_userId);
            Log.WriteLine("Removed " + _userName +
                 " from the cached users list.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("User " + _userName + " is not present on the list!", LogLevel.WARNING);
        }

        await SerializeDB();

        Log.WriteLine("Done with removing " + _userName + " from the cached users list", LogLevel.VERBOSE);
    }
}