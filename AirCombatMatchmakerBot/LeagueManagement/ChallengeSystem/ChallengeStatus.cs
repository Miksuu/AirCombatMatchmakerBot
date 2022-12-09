[Serializable]
public class ChallengeStatus
{
    public List<Team> teamsInTheQueue { get; set; }
    public ChallengeStatus()
    {
        teamsInTheQueue= new List<Team>();
    }
}