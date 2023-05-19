using System.Runtime.Serialization;

[DataContract]
public class ScheduledEvent<T> : logClass<ScheduledEvent<T>>, InterfaceLoggableClass
{
    public ulong TimeToExecuteTheEventOn
    {
        get => timeToExecuteTheEventOn.GetValue();
        set => timeToExecuteTheEventOn.SetValue(value);
    }

    [DataMember] private logClass<ulong> timeToExecuteTheEventOn = new logClass<ulong>();
    [DataMember] private logClass<T> eventClass = new logClass<T>();

    public List<string> GetClassParameters()
    {
        return new List<string> { timeToExecuteTheEventOn.GetParameter(), eventClass.GetParameter()};
    }
}