using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class MATCHSCHEDULERSTATUSMESSAGE : BaseMessage
{
    public MATCHSCHEDULERSTATUSMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHSCHEDULERSTATUSMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.JOINMATCHSCHEDULER, 1),
                new KeyValuePair<ButtonName, int>(ButtonName.LEAVEMATCHSCHEDULER, 1),
            });

        thisInterfaceMessage.MessageEmbedTitle = "[Insert league name here] match scheduler status";
        thisInterfaceMessage.MessageDescription = "Click the buttons to join/leave";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage()
    {
        if (thisInterfaceMessage.MessageDescription == null)
        {
            Log.WriteLine("MessageDescription was null!", LogLevel.CRITICAL);
            return Task.FromResult("MessageDescription was null!");
        }

        return Task.FromResult(thisInterfaceMessage.MessageDescription);
    }
}