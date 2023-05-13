using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class Admins : logClass<Admins>, InterfaceLoggableClass
{
    ConcurrentBag<ulong> AdminIDs
    {
        get => adminIDs.GetValue();
        set => adminIDs.SetValue(value);
    }

    [DataMember] private logConcurrentBag<ulong> adminIDs = new logConcurrentBag<ulong> { 111788167195033600 };

    public List<string> GetClassParameters()
    {
        Log.WriteLine(nameof(GetClassParameters) + " on: " + nameof(Leagues), LogLevel.VERBOSE);
        return new List<string> { adminIDs.GetLoggingClassParameters()};
    }

    public bool CheckIfCommandSenderWasAnAdmin(SocketSlashCommand _command)
    {
        Log.WriteLine("Checking if command sender: " + _command.User.Id + " is an admin.", LogLevel.VERBOSE);
        return AdminIDs.Contains(_command.User.Id);
    }
}