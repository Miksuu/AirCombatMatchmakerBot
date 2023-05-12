using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logConcurrentDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    [DataMember] private ConcurrentDictionary<TKey, TValue> _values;

    public logConcurrentDictionary()
    {
        _values = new ConcurrentDictionary<TKey, TValue>();
    }

    public logConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
        _values = new ConcurrentDictionary<TKey, TValue>(collection ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>());
    }

    public ConcurrentDictionary<TKey, TValue> GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting ConcurrentBag " + _memberName + " with count: " +
            _values.Count + " that has members of: " + GetConcurrentDictionaryMembers(_values),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _values;
    }

    public void SetValue(ConcurrentDictionary<TKey, TValue> values,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ConcurrentBag " + _memberName + " with count: " +_values.Count +
            " that has members of: " + GetConcurrentDictionaryMembers(_values) + " TO: " + " with count: " +
            values.Count + " that has members of: " + GetConcurrentDictionaryMembers(values),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        _values = values;
    }

    public void Add(KeyValuePair<TKey, TValue> _itemKvp,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Adding item to ConcurrentBag " + _memberName + ": " + _itemKvp +
            " with count: " + _values.Count + " that has members of: " + GetConcurrentDictionaryMembers(_values),
            LogLevel.ADD_VERBOSE, _filePath, "", _lineNumber);

        var key = _itemKvp.Key;
        var val = _itemKvp.Value;
        _values.TryAdd( key, val);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public string GetConcurrentDictionaryMembers(ConcurrentDictionary<TKey, TValue> _customValues)
    {
        StringBuilder membersBuilder = new StringBuilder();

        foreach (var item in _customValues)
        {
            switch (item.Value)
            {
                case UnitName unitName:
                    membersBuilder.Append(EnumExtensions.GetEnumMemberAttrValue(unitName)).Append(", ");
                    break;
                case ChannelType channelType:
                    membersBuilder.Append(EnumExtensions.GetEnumMemberAttrValue(channelType)).Append(", ");
                    break;
                case ulong or int:
                    membersBuilder.Append(item.ToString()).Append(", ");
                    break;
                case Player player:
                    membersBuilder.Append(player.PlayerDiscordId + "|" + player.PlayerNickName).Append(", ");
                    break;
                case Team team:
                    membersBuilder.Append(team.TeamId + "|" + team.TeamName + "|" + team.Players + "|" +
                        team.SkillRating + "|" + team.TeamActive).Append(", ");
                    break;
                case LeagueMatch leagueMatch:
                    membersBuilder.Append(leagueMatch.TeamsInTheMatch + "|" + leagueMatch.MatchId + "|" + leagueMatch.MatchChannelId + "|" +
                        leagueMatch.MatchReporting + "|" + leagueMatch.MatchLeague).Append(", ");
                    break;
                case InterfaceLeague interfaceLeague:
                    membersBuilder.Append(interfaceLeague.LeagueCategoryName + "|" + interfaceLeague.LeagueEra + "|" +
                        interfaceLeague.LeaguePlayerCountPerTeam + "|" + interfaceLeague.LeagueUnits + "|" +
                        interfaceLeague.LeagueData).Append(", ");
                    break;
                case InterfaceButton interfaceButton:
                    membersBuilder.Append(interfaceButton.ButtonName + "|" + interfaceButton.ButtonLabel + "|" +
                        interfaceButton.ButtonStyle + "|" + interfaceButton.ButtonCategoryId + "|" +
                        interfaceButton.ButtonCustomId + "|" + interfaceButton.EphemeralResponse).Append(", ");
                    break;
                case PlayerData playerData:
                    membersBuilder.Append(playerData).Append(", ");
                    break;
                default:
                    Log.WriteLine("Tried to get type: " + typeof(TKey) + " unknown, undefined type?", LogLevel.CRITICAL);
                    break;
            }
        }

        return membersBuilder.ToString().TrimEnd(',', ' ');
    }

}
