using System.Runtime.Serialization;

[DataContract]
public class BaseLeague : ILeague
{
    LeagueName ILeague.LeagueName
    {
        get => leagueName;
        set => leagueName = value;
    }

    LeagueType ILeague.LeagueType
    {
        get => leagueType;
        set => leagueType = value;
    }

    Era ILeague.LeagueEra
    {
        get => leagueEra;
        set => leagueEra = value;
    }

    int ILeague.LeaguePlayerCountPerTeam
    {
        get => leaguePlayerCountPerTeam;
        set => leaguePlayerCountPerTeam = value;
    }

    List<UnitName> ILeague.LeagueUnits
    {
        get => leagueUnits;
        set => leagueUnits = value;
    }

    LeagueData ILeague.LeagueData
    {
        get => leagueData;
        set => leagueData = value;
    }

    DiscordLeagueReferences ILeague.DiscordLeagueReferences
    {
        get => discordleagueReferences;
        set => discordleagueReferences = value;
    }

    // Generated based on the implementation
    public LeagueName leagueName;
    public LeagueType leagueType;
    public Era leagueEra;
    public int leaguePlayerCountPerTeam;

    public List<UnitName> leagueUnits = new List<UnitName>();

    public LeagueData leagueData;

    public DiscordLeagueReferences discordleagueReferences;

    public BaseLeague()
    {
    }
}