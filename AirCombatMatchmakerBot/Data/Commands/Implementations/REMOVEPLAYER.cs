using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class REMOVEPLAYER : BaseCommand
{
    public REMOVEPLAYER()
    {
        commandName = CommandName.REMOVEPLAYER;
        commandDescription = "Removes a player from the database, resolves all the matches";
        commandOption = new("playeruid", "Enter playerUID here");
        isAdminCommand = true;
    }

    protected override async Task<string> ActivateCommandFunction(SocketSlashCommand _command, string _firstOptionString)
    {
        ulong commandUserId = _command.User.Id;
        ulong playerToBeRemoved = 0;

        if (ulong.TryParse(_firstOptionString, out ulong output))
        {
            playerToBeRemoved = output;
        }
        else
        {
            Log.WriteLine("Command input was invalid " + _firstOptionString, LogLevel.CRITICAL);
            return "Command input was invalid!";
        }

        await Database.Instance.RemovePlayerFromTheDatabase(playerToBeRemoved);

        Log.WriteLine("playerToBeRemoved: " + playerToBeRemoved, LogLevel.DEBUG);

        return "Player: " + playerToBeRemoved + " removed succesfully!";
    }
}