using Discord;
using System.Formats.Asn1;
using System.Threading.Channels;

public static class LeagueManager
{
    public static async Task CreateLeaguesOnStartup()
    {
        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            // Hardcoded channel id for now
            var channel = guild.GetTextChannel(1049555859656671232) as ITextChannel;

            if (channel != null)
            {
                Log.WriteLine("Channel found: " + channel.Name +
                    "(" + channel.Id + ")", LogLevel.VERBOSE);

                foreach (LeagueName leagueName in Enum.GetValues(typeof(LeagueName)))
                {
                    CreateALeague(channel, leagueName);
                }
            }
            else Log.WriteLine("Channel was null, wrong id in the code?", LogLevel.CRITICAL);


        } else Exceptions.BotGuildRefNull();
    }

    public static void CreateALeague(ITextChannel _channel, LeagueName _leagueName)
    {
        Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

        ILeague leagueInterface = MakeInterfaceFromAEnumName(_leagueName);

        Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
            leagueInterface.LeagueName, LogLevel.VERBOSE);

        leagueInterface = CreateALeagueJoinButton(_channel, leagueInterface).Result;

        StoreTheLeague(leagueInterface);
    }

    public static async Task<ILeague> CreateALeagueJoinButton(
        ITextChannel _channel, ILeague _leagueInterface)
    {
        string leagueEnumAttrValue =
            ClassExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);

        string leagueButtonRegisterationCustomId =
            _leagueInterface.LeagueName.ToString() + "_register";

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        _leagueInterface.LeagueData = new();

        _leagueInterface.LeagueData.leagueChannelMessageId =
            await BotMessaging.CreateButton(
                _channel,
                "Click this button to register to: " + leagueEnumAttrValue,
                "Join",
                leagueButtonRegisterationCustomId); // Maybe replace this with some other system

        return _leagueInterface;
    }

    public static void StoreTheLeague(ILeague _leagueInterface)
    {
        if (_leagueInterface.LeagueData.leagueChannelMessageId != 0)
        {
            if (Database.Instance.StoredLeagues != null)
            {
                //Database.Instance.StoredLeagues.Add(_leagueInterface);
            }
            else Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
                " was null!", LogLevel.CRITICAL);
        }
        else Log.WriteLine(nameof(_leagueInterface.LeagueData.leagueChannelMessageId) +
            " was 0, channel not created succesfully?", LogLevel.CRITICAL);
    }

    public static ILeague MakeInterfaceFromAEnumName<T> (T _enumInput)
    {
        return (ILeague)ClassExtensions.GetInstance(_enumInput.ToString());
    }
}