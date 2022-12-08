using Discord.WebSocket;
using Newtonsoft.Json;

public static class SerializationManager
{
    static string dbPath = @"C:\AirCombatMatchmakerBot\Data\database.json";

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

        using (StreamWriter sw = new StreamWriter(dbPath))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, Database.Instance, typeof(Database));
            writer.Close();
            sw.Close();
        };

        Log.WriteLine("DB SERIALIZATION DONE!", LogLevel.SERIALIZATION);
    }

    public static async Task SerializeUsersOnTheServer()
    {
        Log.WriteLine("Serializing users on the server", LogLevel.SERIALIZATION);

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

        Log.WriteLine("User serialization done on the server", LogLevel.SERIALIZATION);
    }

    public static async Task DeSerializeDB()
    {
        Log.WriteLine("DESERIALIZATION DONE!", LogLevel.SERIALIZATION);

        string json = File.ReadAllText(dbPath);
        if (json != "0")
        {
            Database.Instance = Newtonsoft.Json.JsonConvert.DeserializeObject<Database>(json,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Include,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
        }
        else
        {
            Database.Instance = new();
        }

        Log.WriteLine("DB DESERIALIZATION DONE!", LogLevel.SERIALIZATION);
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