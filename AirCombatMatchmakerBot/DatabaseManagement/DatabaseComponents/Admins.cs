using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class Admins
{
    List<ulong> AdminIDs
    {
        get
        {
            Log.WriteLine("Getting " + nameof(adminIDs) + " with count of: " +
                adminIDs.Count, LogLevel.VERBOSE);
            return adminIDs;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(adminIDs)
                + " to: " + value, LogLevel.VERBOSE);
            adminIDs = value;
        }
    }

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
        return AdminIDs.Contains(_command.User.Id);
    }
}