using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class Admins
{
    ConcurrentBag<ulong> AdminIDs
    {
        get => adminIDs.GetValue();
        set => adminIDs.SetValue(value);
    }

    [DataMember] private logConcurrentBag<ulong> adminIDs = new logConcurrentBag<ulong> { 111788167195033600 };

    public bool CheckIfCommandSenderWasAnAdmin(SocketSlashCommand _command)
    {
        Log.WriteLine("Checking if command sender: " + _command.User.Id + " is an admin.");
        return AdminIDs.Contains(_command.User.Id);
    }
}