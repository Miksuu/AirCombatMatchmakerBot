using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class LEAGUEREGISTRATIONMESSAGE : BaseMessage
{
    public LEAGUEREGISTRATIONMESSAGE()
    {
        messageName = MessageName.LEAGUEREGISTRATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int> 
        {
            { ButtonName.LEAGUEREGISTRATIONBUTTON, 1 }
        };
        message = "Insert league registration message here";
    }

    public override string GenerateMessage()
    {
        return message;
    }

    public override bool GenerateTuple(FieldInfo _field, ReportData _reportData, Type _type)
    {
        throw new NotImplementedException();
    }
}