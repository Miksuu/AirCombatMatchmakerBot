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
    [DataMember] private logClass<TeamMatchmakingState> teamMatchmakingState = new logClass<TeamMatchmakingState>();

    public TeamMatchmakerData() { }
}