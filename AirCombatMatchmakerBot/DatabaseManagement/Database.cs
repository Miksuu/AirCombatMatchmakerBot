using Discord.WebSocket;
using System.Runtime.Serialization;

[Serializable]
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

    public async Task RemovePlayerFromTheDatabase(ulong _playerDiscordId)
    {
        Log.WriteLine("Removing player: " + _playerDiscordId + " from the database.", LogLevel.DEBUG);

        await PlayerData.DeletePlayerProfile(_playerDiscordId);
        CachedUsers.RemoveUserFromTheCachedList(_playerDiscordId);

        foreach (InterfaceLeague interfaceLeague in Leagues.StoredLeagues)
        {
            foreach (Team team in interfaceLeague.LeagueData.Teams.TeamsList)
            {
                if (team.Players.Any(p => p.PlayerDiscordId == _playerDiscordId))
                {
                    foreach (LeagueMatch match in interfaceLeague.LeagueData.Matches.MatchesList)
                    {
                        if (match.TeamsInTheMatch.ContainsKey(team.TeamId))
                        {
                            Log.WriteLine("Match: " + match.MatchId + " contains: " + team.TeamId +
                                " which has player: " + _playerDiscordId, LogLevel.DEBUG);

                            match.FinishTheMatch(interfaceLeague, team.TeamId);
                        }
                    }

                    // Removes from the queue and updates the message
                    interfaceLeague.LeagueData.ChallengeStatus.TeamsInTheQueue.Remove(team.TeamId);
                    var challengeMessage = Categories.FindCreatedCategoryWithChannelKvpWithId(
                        interfaceLeague.DiscordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
                            ChannelType.CHALLENGE).FindInterfaceMessageWithNameInTheChannel(
                                MessageName.CHALLENGEMESSAGE);

                    if (challengeMessage == null)
                    {
                        Log.WriteLine(nameof(challengeMessage) + " was null!", LogLevel.ERROR);
                        continue;
                    }

                    await challengeMessage.GenerateAndModifyTheMessage();

                    interfaceLeague.LeagueData.Teams.TeamsList.Remove(team);
                    Log.WriteLine("Found and removed" + _playerDiscordId + " in team: " + team.TeamName +
                        " with id: " + team.TeamId, LogLevel.DEBUG);
                }
            }
        }

        // Remove user's access (back to the registration...)
        await RoleManager.RevokeUserAccess(_playerDiscordId, "Member");

        foreach (InterfaceLeague interfaceLeague in Database.Instance.Leagues.StoredLeagues)
        {
            if (interfaceLeague.LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                interfaceLeague.LeaguePlayerCountPerTeam, _playerDiscordId))
            {
                await RoleManager.RevokeUserAccess(_playerDiscordId, EnumExtensions.GetEnumMemberAttrValue(
                    interfaceLeague.LeagueCategoryName));
            } 
        }

        await SerializationManager.SerializeDB();
    }
}