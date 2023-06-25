using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class MATCHSCHEDULERSTATUSMESSAGE : BaseMessage
{
    LeagueCategoryComponents lcc;

    public MATCHSCHEDULERSTATUSMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHSCHEDULERSTATUSMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.JOINMATCHSCHEDULER, 1),
                new KeyValuePair<ButtonName, int>(ButtonName.LEAVEMATCHSCHEDULER, 1),
            });

        thisInterfaceMessage.MessageEmbedTitle = "[Insert League Name] Status";
        thisInterfaceMessage.MessageDescription = "Click the buttons to join/leave";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage()
    {
        //try
        //{

        //}
        //catch (Exception ex)
        //{
        //    Log.WriteLine(ex.Message);
        //    throw;
        //}

        //string finalMessage = string.Empty;

        ////lcc = new LeagueCategoryComponents(MessageCategoryId);
        //if (lcc.interfaceLeagueCached == null)
        //{
        //    Log.WriteLine(nameof(lcc) + " was null!", LogLevel.CRITICAL);
        //    throw new InvalidOperationException(nameof(lcc) + " was null!");
        //}

        return Task.FromResult(thisInterfaceMessage.MessageDescription);
    }
}