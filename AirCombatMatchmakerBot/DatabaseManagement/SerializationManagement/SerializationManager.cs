﻿using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

public static class SerializationManager
{
    static string dbPath = @"C:\AirCombatMatchmakerBot\Data";
    static string dbFileName = "database.json";
    static string dbPathWithFileName = dbPath + @"\" + dbFileName;

    public static async Task SerializeDB(bool _circularDependency = false)
    {
        Log.WriteLine("SERIALIZING DB", LogLevel.SERIALIZATION);

        if (!_circularDependency)
        {
            await SerializeUsersOnTheServer();
        }

        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
        serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
        serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
        serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;

        using (StreamWriter sw = new StreamWriter(dbPathWithFileName))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, Database.Instance, typeof(Database));
            writer.Close();
            sw.Close();
        };

        Log.WriteLine("DB SERIALIZATION DONE!", LogLevel.SERIALIZATION);
    }

    public static Task SerializeUsersOnTheServer()
    {
        Log.WriteLine("Serializing users on the server", LogLevel.SERIALIZATION);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return Task.CompletedTask;
        }

        foreach (SocketGuildUser user in guild.Users)
        {
            if (user == null)
            {
                Log.WriteLine("User was null!", LogLevel.ERROR);
            }
            else
            {
                // Move to method
                string userString = user.Username + " (" + user.Id + ")";
                Log.WriteLine("Looping on: " + userString, LogLevel.VERBOSE);

                if (!user.IsBot)
                {
                    if (!Database.Instance.PlayerData.CheckIfUserHasPlayerProfile(user.Id))
                    {
                        Log.WriteLine("User: " + user.Id +
                            " does not have a profile, disregarding", LogLevel.VERBOSE);

                        continue;
                    }

                    Database.Instance.CachedUsers.AddUserIdToCachedConcurrentBag(user.Id);
                }
                else
                {
                    Log.WriteLine(userString + " is a bot, disregarding.", LogLevel.VERBOSE);
                }
            }
        }
        Log.WriteLine("Done looping through current users.", LogLevel.VERBOSE);

        return Task.CompletedTask;
    }

    public static Task DeSerializeDB()
    {
        Log.WriteLine("DESERIALIZATION STARTING!", LogLevel.SERIALIZATION);

        FileManager.CheckIfFileAndPathExistsAndCreateItIfNecessary(dbPath, dbFileName);

        string json = File.ReadAllText(dbPathWithFileName);

        HandleDatabaseCreationOrLoading(json);

        Log.WriteLine("DB DESERIALIZATION DONE!", LogLevel.SERIALIZATION);

        return Task.CompletedTask;
    }

    // _json param to 0 to force creation of the new db
    public static Task HandleDatabaseCreationOrLoading(string _json)
    {
        if (_json == "0")
        {
            //FileManager.CheckIfFileAndPathExistsAndCreateItIfNecessary(dbPath, dbFileName);
            Database.Instance = new();
            Log.WriteLine("json was " + _json + ", creating a new db instance", LogLevel.DEBUG);

            return Task.CompletedTask;
        }

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        settings.NullValueHandling = NullValueHandling.Include;
        settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
        //settings.Converters.Add(new logConcurrentBagConverter<UnitName>());

        var newDeserializedObject = JsonConvert.DeserializeObject<Database>(_json, settings);


        if (newDeserializedObject == null)
        {
            return Task.CompletedTask;
        }

        Database.Instance = newDeserializedObject;

        return Task.CompletedTask;
    }
}