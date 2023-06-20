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
    public ScheduleObject(DateTime? _requestedTime, int _teamId)
    {
        Log.WriteLine("Creating a new " + nameof(ScheduleObject) + " by: " + _teamId, LogLevel.DEBUG);

        if (!_requestedTime.HasValue)
        {
            Log.WriteLine(nameof(_requestedTime) + " was null!", LogLevel.CRITICAL);
            return;
        }
        RequestedSchedulingTimeInUnixTime = (ulong)(_requestedTime.Value - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        TeamIdThatRequestedScheduling = _teamId;
    }

}