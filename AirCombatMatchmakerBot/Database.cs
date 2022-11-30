using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public class Database
{
    private static Database? instance = null;
    private static readonly object padlock = new object();

    //public List<Tournament> Tournaments { get; set; }

    public Dictionary<ulong, string> adminIDs { get; set; }

    public List<ulong> cantRegisterIDs { get; set; }

    public PlayerData PlayerData { get; set; }

    public int channelTournamentIndex { get; set; }
    public int channelMatchIndex { get; set; }

    public Database()
    {
        PlayerData = new PlayerData();
        //Tournaments = new List<Tournament>();
        adminIDs = new Dictionary<ulong, string>();
        cantRegisterIDs = new List<ulong>();
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