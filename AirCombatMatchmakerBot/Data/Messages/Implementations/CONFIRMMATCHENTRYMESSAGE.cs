using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class CONFIRMMATCHENTRYMESSAGE : BaseMessage
{
    public CONFIRMMATCHENTRYMESSAGE()
    {
        messageName = MessageName.CONFIRMMATCHENTRYMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.CONFIRMMATCHENTRYBUTTON, 1),
            });

        messageEmbedTitle = "This message confirms the match entry [add more detailed message here]";
    }

    public override string GenerateMessage()
    {
        return messageDescription;
    }
}