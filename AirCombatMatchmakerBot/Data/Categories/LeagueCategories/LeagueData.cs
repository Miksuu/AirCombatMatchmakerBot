[Serializable]
public class LeagueData
{
    public List<Team> Teams { get; set; }
    public int currentTeamInt { get; set; }
    public bool matchmakerActive { get; set; }
    public ChallengeStatus challengeStatus { get; set; }
    public LeagueData()
    {
        Teams = new();
        challengeStatus = new();
        currentTeamInt = 1;
    }
}