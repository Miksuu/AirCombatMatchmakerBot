using Discord;
using System.Runtime.Serialization;

[DataContract]
public class LeagueData
{
    public Teams Teams { get; set; }
    public ChallengeStatus ChallengeStatus { get; set; }
    [DataMember] private bool matchmakerActive { get; set; }
    public LeagueData()
    {
        Teams = new();
        ChallengeStatus = new();
    }
}