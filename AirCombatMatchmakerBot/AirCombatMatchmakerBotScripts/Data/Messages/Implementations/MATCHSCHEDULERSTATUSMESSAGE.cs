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

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        try
        {
            string finalMessage = string.Empty;

            lcc = new LeagueCategoryComponents(thisInterfaceMessage.MessageCategoryId);
            if (lcc.interfaceLeagueCached == null)
            {
                Log.WriteLine(nameof(lcc) + " was null!", LogLevel.ERROR);
                throw new InvalidOperationException(nameof(lcc) + " was null!");
            }

            thisInterfaceMessage.MessageEmbedTitle =
                EnumExtensions.GetEnumMemberAttrValue(lcc.interfaceLeagueCached.LeagueCategoryName);

            // Upcoming matches

            // Current matches

            finalMessage += "*Click the buttons below to join/leave the scheduler*";

            return Task.FromResult(thisInterfaceMessage.MessageDescription);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message);
            throw;
        }
    }

    public override string GenerateMessageFooter()
    {
        return "";
    }
}