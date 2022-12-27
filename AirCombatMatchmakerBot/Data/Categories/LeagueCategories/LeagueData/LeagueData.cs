using Discord;

[Serializable]
public class LeagueData
{
    public Teams Teams { get; set; }
    public ChallengeStatus ChallengeStatus { get; set; }
    private bool matchmakerActive { get; set; }
    public LeagueData()
    {
        Teams = new();
        ChallengeStatus = new();
    }
}