using System;
using System.Runtime.Serialization;

[DataContract]
public class ScheduleObject : logClass<ScheduleObject>
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

    [DataMember] private logClass<ulong> requestedSchedulingTimeInUnixTime = new logClass<ulong>();
    [DataMember] private logClass<int> teamIdThatRequestedScheduling = new logClass<int>();


    public ScheduleObject() { }
    public ScheduleObject(ulong _requestedTime, int _teamId)
    {
        Log.WriteLine("Creating a new " + nameof(ScheduleObject) + " by: " + _teamId +
            " with time: " + _requestedTime, LogLevel.DEBUG);

        RequestedSchedulingTimeInUnixTime = _requestedTime;
        TeamIdThatRequestedScheduling = _teamId;
    }

}