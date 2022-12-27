using Discord;
using System.Runtime.Serialization;

[DataContract]
public class LeagueData
{
    [DataMember] public Teams Teams { get; set; }
    [DataMember] public ChallengeStatus ChallengeStatus { get; set; }
    [DataMember] private bool matchmakerActive { get; set; }
    public LeagueData()
    {
        Teams = new();
        ChallengeStatus = new();
    }
}