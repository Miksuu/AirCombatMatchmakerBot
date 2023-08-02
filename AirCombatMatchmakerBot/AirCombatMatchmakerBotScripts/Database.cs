using Discord;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.Serialization;

[DataContract]
public class Database
{
    public static Database Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }
        set
        {
            instance = value;
        }
    }

    // Singleton stuff
    private static Database? instance;
    private static readonly object padlock = new object();

    static string appName = Assembly.GetEntryAssembly()?.GetName()?.FullName;

    // File paths
    public static string mainAppnameDataDirectory = @"C:\" + appName + @"\Data\";
    public static string discordDataDir = mainAppnameDataDirectory + @"\DiscordBotDatabase";
    public static string dbPathWithFileName = discordDataDir + @"\" + "database.json";

    static string dbTempFileName = "database.tmp";
    public static string dbTempPathWithFileName = DiscordBotDatabase.dbPathWithFileName + @"\" + dbTempFileName;

    // The Database components
    [DataMember] public Leagues leagues = new Leagues();
}