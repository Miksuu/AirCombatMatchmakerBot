using Newtonsoft.Json;

public static class SerializationManager
{
    static string dbPath = @"C:\AirCombatMatchmakerBot\Data\database.json";

    // Run this everytime when relevant data needs to be saved to the database
    public static Task SerializeDB()
    {
        /*   
           List<Tournament> TournamentsT = new List<Tournament>();

           Dictionary<ulong, string> adminIDsT = new Dictionary<ulong, string>()
           {
               { 111788167195033600, "Miksuu" }
           };

           List<ulong> cantRegisterIDsT = new List<ulong>()
           {
               956237183012982815 // The bot
           };

           var db = new Database(TournamentsT, adminIDsT, cantRegisterIDsT);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            MaxDepth = 64 // Fixed
        }; */

        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
        serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
        serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
        serializer.MaxDepth= 64;

        //Connsole.WriteLine("SERIALIZATION, STARTING for " + Database.Instance.Tournaments.Count);

        using (StreamWriter sw = new StreamWriter(dbPath))
        using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
        {
            //string jsonStringSer = JsonSerializer.Serialize<Database>(Database.Instance, options);
            serializer.Serialize(writer, Database.Instance, typeof(Database));
            writer.Close();
            sw.Close();
        };



        //Log.WriteLine("SERIALIZING JSON: " + jsonStringSer);


        //Log.WriteLine(ServerManager.Instance.dcsInstallationPath);

        return Task.CompletedTask;
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
}