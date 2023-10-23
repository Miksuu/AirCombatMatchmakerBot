using System;
using System.Runtime.Serialization;

[DataContract]
public class ScheduleObject
{
    public ulong RequestedSchedulingTimeInUnixTime
    {
        get => requestedSchedulingTimeInUnixTime.GetValue();
        set => requestedSchedulingTimeInUnixTime.SetValue(value);
    }

    public int TeamIdThatRequestedScheduling
    {
        get => teamIdThatRequestedScheduling.GetValue();
        set => teamIdThatRequestedScheduling.SetValue(value);
    }

    [DataMember] private logVar<ulong> requestedSchedulingTimeInUnixTime = new logVar<ulong>();
    [DataMember] private logVar<int> teamIdThatRequestedScheduling = new logVar<int>();


    public ScheduleObject() { }
    public ScheduleObject(ulong _requestedTime, int _teamId)
    {
        Log.WriteLine("Creating a new " + nameof(ScheduleObject) + " by: " + _teamId +
            " with time: " + _requestedTime, LogLevel.DEBUG);

        RequestedSchedulingTimeInUnixTime = _requestedTime;
        TeamIdThatRequestedScheduling = _teamId;
    }

}