using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class REPORTINGSTATUSMESSAGE : BaseMessage
{
    public REPORTINGSTATUSMESSAGE()
    {
        messageName = MessageName.REPORTINGSTATUSMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>();
        message = "Insert the reporting status message here";
    }

    public override string GenerateMessage(ulong _channelId, ulong _channelCategoryId)
    {
        string reportingStatusMessage = string.Empty;

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(_channelCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return "";
        }

        LeagueMatch? foundMatch =
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(_channelId);
        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return "";
        }

        foreach (var item in foundMatch.TeamsInTheMatch)
        {

        }


        return message;
    }
}