using System.Runtime.Serialization;

[DataContract]
public class Team
{
    [DataMember] public int skillRating { get; set; }
    [DataMember] public string teamName { get; set; }
    [DataMember] public List<Player> players { get; set; }
    [DataMember] public bool teamActive { get; set; }
    [DataMember] public int teamId { get; set; }

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