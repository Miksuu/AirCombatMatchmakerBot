using Discord.WebSocket;
using System.Runtime.Serialization;

[Serializable]
public class Database
{
    // Singleton stuff
    private static Database? instance = null;
    private static readonly object padlock = new object();

    // The Database components
    public PlayerData PlayerData { get; set; }
    public Admins Admins { get; set; }
    public CachedUsers CachedUsers { get; set; }
    public Categories Categories { get; set; }
    public Leagues Leagues { get; set; }
    public List<LeagueMatch> ArchivedLeagueMatches { get; set; }

    public Database()
    {
        Admins = new Admins();
        CachedUsers = new CachedUsers();
        Categories = new Categories();
        PlayerData = new PlayerData();        
        Leagues = new Leagues();
        ArchivedLeagueMatches = new List<LeagueMatch>();
    }

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
}