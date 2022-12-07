using Discord;

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
                    Log.WriteLine("Looping on leagueName: " + leagueName.ToString(), LogLevel.VERBOSE);

                    ILeague leagueInterface = MakeInterfaceFromAEnumName(leagueName);

                    Log.WriteLine("Made a " + nameof(leagueInterface) + " named: "+
                        leagueInterface.LeagueName,LogLevel.VERBOSE );

                    string leagueEnumAttrValue = 
                        ClassExtensions.GetEnumMemberAttrValue(leagueInterface.LeagueName);

                    Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
                        leagueEnumAttrValue,LogLevel.VERBOSE);

                    string leagueButtonRegisterationCustomId = 
                        leagueInterface.LeagueName.ToString() + "_register";

                    Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
                        leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

                    leagueInterface.LeagueData = new();

                    leagueInterface.LeagueData.leagueChannelMessageId =
                        await BotMessaging.CreateButton(
                            channel,
                            "Click this button to register to: " + leagueEnumAttrValue,
                            "Join",
                            leagueButtonRegisterationCustomId); // Maybe replace this with some other system

                    if (leagueInterface.LeagueData.leagueChannelMessageId != 0)
                    {
                        if (Database.Instance.StoredLeagues != null)
                        {
                            //Database.Instance.StoredLeagues.Add(leagueInterface);
                        }
                        else Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
                            " was null!", LogLevel.CRITICAL);
                    }
                    else Log.WriteLine(nameof(leagueInterface.LeagueData.leagueChannelMessageId) +
                        " was 0, channel not created succesfully?", LogLevel.CRITICAL);
                }
            }
            else Log.WriteLine("Channel was null, wrong id in the code?", LogLevel.CRITICAL);


        } else Exceptions.BotGuildRefNull();
    }

    public static ILeague MakeInterfaceFromAEnumName<T> (T _enumInput)
    {
        return (ILeague)ClassExtensions.GetInstance(_enumInput.ToString());
    }
}