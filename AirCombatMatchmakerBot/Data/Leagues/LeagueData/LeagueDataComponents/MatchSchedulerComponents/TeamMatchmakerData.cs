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

    //public int TeamMissedMatchesFromScheduler
    //{
    //    get => teamMissedMatchesFromScheduler.GetValue();
    //    set => teamMissedMatchesFromScheduler.SetValue(value);
    //}

    public int TeamThatWasFoughtPreviously
    {
        get => teamThatWasFoughtPreviously.GetValue();
        set => teamThatWasFoughtPreviously.SetValue(value);
    }

    [DataMember] private logClass<TeamMatchmakingState> teamMatchmakingState = new logClass<TeamMatchmakingState>();
    //[DataMember] private logVar<int> teamMissedMatchesFromScheduler = new logVar<int>();
    [DataMember] private logVar<int> teamThatWasFoughtPreviously = new logVar<int>();

    public TeamMatchmakerData() { }

    public void SetValuesOnFindingAMatch(int _opposingTeamKey)
    {
        TeamMatchmakingState = TeamMatchmakingState.INMATCH;
        TeamThatWasFoughtPreviously = _opposingTeamKey;
        //TeamMissedMatchesFromScheduler = 0;
    }
}