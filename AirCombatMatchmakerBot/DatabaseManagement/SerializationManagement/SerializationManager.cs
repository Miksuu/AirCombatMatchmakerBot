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
            //serializer.Serialize(writer, Database.Instance, typeof(Database));
            writer.Close();
            sw.Close();
        };

        Log.WriteLine("DB SERIALIZATION DONE!", LogLevel.SERIALIZATION);
    }

    public static async Task SerializeUsersOnTheServer()
    {
        Log.WriteLine("Serializing users on the server", LogLevel.SERIALIZATION);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
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
                    if (!UserManager.CheckIfUserHasPlayerProfile(user.Id))
                    {
                        Log.WriteLine("User: " + user.Id +
                            " does not have a profile, disregarding", LogLevel.VERBOSE);

                        continue;
                    }

                    AddUserIdToCachedList(user.Id);
                }
                else
                {
                    Log.WriteLine(userString + " is a bot, disregarding.", LogLevel.VERBOSE);
                }
            }
        }
        Log.WriteLine("Done looping through current users.", LogLevel.VERBOSE);

        await SerializeDB(true);

        Log.WriteLine("User serialization done on the server", LogLevel.SERIALIZATION);
    }

    public static Task DeSerializeDB()
    {
        Log.WriteLine("DESERIALIZATION DONE!", LogLevel.SERIALIZATION);

        string json = File.ReadAllText(dbPath);
        if (json == "0")
        {
            Database.Instance = new();
            Log.WriteLine("json was " + json + ", creating a new db instance", LogLevel.DEBUG);

            return Task.CompletedTask;
        }

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        settings.NullValueHandling = NullValueHandling.Include;
        settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

        var newDeserializedObject = JsonConvert.DeserializeObject<Database>(json, settings);

        if (newDeserializedObject == null)
        {
            return Task.CompletedTask;
        }

        Database.Instance = newDeserializedObject;

        Log.WriteLine("DB DESERIALIZATION DONE!", LogLevel.SERIALIZATION);

        return Task.CompletedTask;
    }

    public static void AddUserIdToCachedList(ulong _userId)
    {
        Log.WriteLine("Adding " + _userId + " to the cache list", LogLevel.VERBOSE);

        if (!Database.Instance.cachedUserIDs.Contains(_userId))
        {
            Database.Instance.cachedUserIDs.Add(_userId);
            Log.WriteLine("Added " + _userId +
                " to cached users list.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("User " + _userId + " is already on the list", LogLevel.VERBOSE);
        }
    }

    public static async void RemoveUserFromTheCachedList(string _userName, ulong _userId)
    {
        Log.WriteLine("Removing " + _userName + "(" + _userId + ")" + " from the cache list", LogLevel.VERBOSE);

        if (!Database.Instance.cachedUserIDs.Contains(_userId))
        {
            Log.WriteLine("User " + _userName + " is not present on the list!", LogLevel.WARNING);
            return;
        }

        Database.Instance.cachedUserIDs.Remove(_userId);

        Log.WriteLine("Removed " + _userName + " from the cached users list.", LogLevel.DEBUG);

        await SerializeDB();
    }
}