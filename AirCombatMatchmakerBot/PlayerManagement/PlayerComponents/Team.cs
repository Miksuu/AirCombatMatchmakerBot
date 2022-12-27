using System.Runtime.Serialization;

[DataContract]
public class Team
{
    public int skillRating { get; set; }
    public string teamName { get; set; }
    public List<Player> players { get; set; }
    public bool teamActive { get; set; }
    public int teamId { get; set; }

    public Team()
    {
        skillRating = 1600;
        teamName = string.Empty;
        players = new List<Player>();
        teamActive = false;
    }

    public Team(List<Player> _players, string _teamName, int _teamId)
    {
        skillRating = 1600;
        teamName = _teamName;
        players = _players;
        teamId = _teamId;
    }
}