[Serializable]
public class LeagueData
{
    public List<Team> Teams { get; set; }
    public bool active { get; set; }
    public LeagueData()
    {
        Teams = new();
    }
}