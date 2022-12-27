using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class Admins
{
    [DataMember] private List<ulong> adminIDs { get; set; }

    public Admins()
    {
        // Load this from json
        adminIDs = new List<ulong> {
            111788167195033600
        };

    }

    public bool CheckIfCommandSenderWasAnAdmin(SocketSlashCommand _command)
    {
        Log.WriteLine("Checking if command sender: " + _command.User.Id + " is an admin.", LogLevel.VERBOSE);
        return adminIDs.Contains(_command.User.Id);
    }
}