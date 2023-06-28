using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class TeamMatchmakerData : logClass<MatchScheduler>
{
    public TeamMatchmakingState TeamMatchmakingState
    {
        get => teamMatchmakingState.GetValue();
        set => teamMatchmakingState.SetValue(value);
    }

    public int TeamMissedMatchesFromScheduler
    {
        get => teamMissedMatchesFromScheduler.GetValue();
        set => teamMissedMatchesFromScheduler.SetValue(value);
    }

    [DataMember] private logClass<TeamMatchmakingState> teamMatchmakingState = new logClass<TeamMatchmakingState>();
    [DataMember] private logClass<int> teamMissedMatchesFromScheduler = new logClass<int>();

    public TeamMatchmakerData() { }
}