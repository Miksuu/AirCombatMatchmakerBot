using Discord;
using System.Formats.Asn1;
using System.Threading.Channels;

public static class LeagueManager
{
    public static Task CreateLeaguesOnStartup()
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

        return Task.CompletedTask;
    }

    public static void CreateALeague(ITextChannel _channel, LeagueName _leagueName)
    {
        Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

        var leagueInstance = ClassExtensions.GetInstance(_leagueName.ToString());
        ILeague leagueInterface = (ILeague)leagueInstance;

        Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
            leagueInterface.LeagueName, LogLevel.VERBOSE);

        if (Database.Instance.StoredLeagues != null)
        {
            if (Database.Instance.StoredLeagues.Any(l => l.LeagueName == _leagueName))
            {
                Log.WriteLine("name: " + _leagueName.ToString() +
                    " was already in the list, returning", LogLevel.VERBOSE);
                return;
            }
            else
            {
                Log.WriteLine("name: " + _leagueName.ToString() +
                    " was not found, creating a League for it", LogLevel.VERBOSE);

                leagueInterface = CreateALeagueJoinButton(_channel, leagueInterface).Result;

                StoreTheLeague(leagueInterface);
            }

        }
        else Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
            " was null!", LogLevel.CRITICAL);
    }

    public static async Task<ILeague> CreateALeagueJoinButton(
        ITextChannel _channel, ILeague _leagueInterface)
    {
        string leagueButtonRegisterationCustomId =
            _leagueInterface.LeagueName.ToString() + "_register";

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        _leagueInterface.LeagueData = new();

        _leagueInterface.LeagueData.leagueChannelMessageId =
            await BotMessaging.CreateButton(
                _channel,
                GenerateALeagueJoinButtonMessage(_leagueInterface),
                "Join",
                leagueButtonRegisterationCustomId); // Maybe replace this with some other system

        return _leagueInterface;
    }

    public static string GenerateALeagueJoinButtonMessage(ILeague _leagueInterface)
    {
        string leagueEnumAttrValue =
            ClassExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);


        return " " + "\n" + leagueEnumAttrValue + "\n" +
            GetAllowedUnitsAsString(_leagueInterface) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCount(_leagueInterface);
             
    }

    private static string GetIfTheLeagueHasPlayersOrTeamsAndCount(ILeague _leagueInterface)
    {
        int count = _leagueInterface.LeagueData.Teams.Count;

        if (_leagueInterface.LeaguePlayerCountPerTeam > 1)
        {
            return "Teams: " + count;
        }
        else
        {
            return "Players: " + count;
        }
    }

    private static string GetAllowedUnitsAsString(ILeague _leagueInterface)
    {
        string allowedUnits = string.Empty;

        for (int u = 0; u < _leagueInterface.LeagueUnits.Count; ++u)
        {
            allowedUnits += ClassExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueUnits[u]);

            // Is not the last index
            if (u != _leagueInterface.LeagueUnits.Count - 1)
            {
                allowedUnits += ", ";
            }
        }

        return allowedUnits;
    }

    public static void StoreTheLeague(ILeague _leagueInterface)
    {
        if (_leagueInterface.LeagueData.leagueChannelMessageId != 0)
        {
            if (Database.Instance.StoredLeagues != null)
            {
                Database.Instance.StoredLeagues.Add(_leagueInterface);
            }
            else Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
                " was null!", LogLevel.CRITICAL);
        }
        else Log.WriteLine(nameof(_leagueInterface.LeagueData.leagueChannelMessageId) +
            " was 0, channel not created succesfully?", LogLevel.CRITICAL);
    }


    /*
    public static ILeague MakeInterfaceFromAEnumName<T> (T _enumInput)
    {
        return (ILeague)ClassExtensions.GetInstance(_enumInput).ToString();
    }*/
}