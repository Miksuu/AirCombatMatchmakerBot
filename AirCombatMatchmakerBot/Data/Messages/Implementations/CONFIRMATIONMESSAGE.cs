using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class CONFIRMATIONMESSAGE : BaseMessage
{
    public CONFIRMATIONMESSAGE()
    {
        messageName = MessageName.CONFIRMATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>();
        /*
        {
            { ButtonName.REPORTSCOREBUTTON, 4 }
        };*/
        message = "Insert the confirmation message here";
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