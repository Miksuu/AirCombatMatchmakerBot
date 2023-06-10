using System.Runtime.Serialization;

[DataContract]
public class ScheduleObject : logClass<ScheduleObject>
{
    public DateTime RequestedSchedulingTime
    {
        get => requestedSchedulingTime.GetValue();
        set => requestedSchedulingTime.SetValue(value);
    }

    public int TeamIdThatRequestedScheduling
    {
        get => teamIdThatRequestedScheduling.GetValue();
        set => teamIdThatRequestedScheduling.SetValue(value);
    }


    [DataMember] private logClass<DateTime> requestedSchedulingTime = new logClass<DateTime>();
    [DataMember] private logClass<int> teamIdThatRequestedScheduling = new logClass<int>();


    public ScheduleObject() { }
    public ScheduleObject(DateTime _requestedTime, int _teamId)
    {
        Log.WriteLine("Creating a new " + nameof(ScheduleObject) + " by: " + _teamId, LogLevel.DEBUG);
        RequestedSchedulingTime = _requestedTime;
        TeamIdThatRequestedScheduling = _teamId;
    }
}