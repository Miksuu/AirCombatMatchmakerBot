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

    public List<ulong> adminIDs { get; set; }

    public List<ulong> cantRegisterIDs { get; set; }

    // The registered users
    public PlayerData PlayerData { get; set; }

    // Users not registered, more simple class for them without any player data.
    public List<NonRegisteredUser> NonRegisteredUsers { get; set; }

    // The stored leagues that implement the ILeague interface
    public List<ILeague>? StoredLeagues { get; set; }

    public List<ulong> cachedUserIDs { get; set; }

    public int channelTournamentIndex { get; set; }
    public int channelMatchIndex { get; set; }

    public Database()
    {
        PlayerData = new PlayerData();
        NonRegisteredUsers = new List<NonRegisteredUser>();
        cachedUserIDs = new List<ulong>();

        // Load this from json
        adminIDs = new List<ulong> { 
            111788167195033600
        };

        cantRegisterIDs = new List<ulong>();

        StoredLeagues = new();
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