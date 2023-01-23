using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class REGISTRATIONMESSAGE : BaseMessage
{
    public REGISTRATIONMESSAGE()
    {
        messageName = MessageName.REGISTRATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int> 
        {
            { ButtonName.REGISTRATIONBUTTON, 1 },
        };
        message = "Click this button to register!";
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