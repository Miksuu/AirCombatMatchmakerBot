using System.Runtime.Serialization;

[DataContract]
public abstract class BaseMatchButton : BaseButton, InterfaceButton
{
    protected InterfaceLeague? interfaceLeagueCached { get; set; }
    protected LeagueMatch? leagueMatchCached { get; set; }

    protected string FindMatchTupleAndInsertItToTheCache(InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting to find with: " + _interfaceMessage.MessageId +
            " and with category id: " + _interfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        if (interfaceLeagueCached != null || leagueMatchCached != null)
        {
            Log.WriteLine("Already cached, returning", LogLevel.VERBOSE);
            return "";
        }

        InterfaceChannel interfaceChannel =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId);
        if (interfaceChannel == null)
        {
            string errorMsg = nameof(interfaceChannel) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            //return (errorMsg, false);
            return errorMsg;
        }
        Log.WriteLine("Found: " + nameof(interfaceChannel) +
            interfaceChannel.ChannelId, LogLevel.VERBOSE);

        MATCHCHANNEL? matchChannel = (MATCHCHANNEL)interfaceChannel;
        if (matchChannel == null)
        {
            string errorMsg = nameof(matchChannel) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            //return (errorMsg, false);
            return errorMsg;
        }
        Log.WriteLine("Found: " + nameof(matchChannel) + matchChannel, LogLevel.VERBOSE);

        var matchTuple =
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
                buttonCategoryId, _interfaceMessage.MessageChannelId);
        if (matchTuple.Item1 == null || matchTuple.Item2 == null)
        {
            Log.WriteLine(matchTuple.Item3, LogLevel.CRITICAL);
            //return (matchTuple.Item3, false);
            return matchTuple.Item3;
        }

        interfaceLeagueCached = matchTuple.Item1;
        leagueMatchCached = matchTuple.Item2;

        Log.WriteLine("Set: " + interfaceLeagueCached + " | " +
            leagueMatchCached, LogLevel.VERBOSE);

        return matchTuple.Item3;
    }
}