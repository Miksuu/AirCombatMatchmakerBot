using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class logConcurrentBag<T> : IEnumerable<T>, InterfaceLoggingClass
{
    [DataMember] private ConcurrentBag<T> _values = new ConcurrentBag<T>();

    public logConcurrentBag() { }

    public logConcurrentBag(IEnumerable<T> collection)
    {
        _values = new ConcurrentBag<T>(collection);
    }

    public string GetLoggingClassParameters<TKey, TValue>()
    {
        StringBuilder membersBuilder = new StringBuilder();
        foreach (var item in _values)
        {
            string finalValueForTheProperty = string.Empty;

            List<Type> regularVariableTypes = new List<Type>
            {
                typeof(ulong), typeof(Int32), typeof(float), typeof(bool)
            };

            if (regularVariableTypes.Contains(item.GetType()))
            {
                finalValueForTheProperty = item.ToString();
            }
            else
            {
                if (item is logClass<TKey>)
                {
                    finalValueForTheProperty = ((logClass<TKey>)(object)item).GetParameter();
                }
            }

            membersBuilder.Append(finalValueForTheProperty).Append(", ");
        }

        return membersBuilder.ToString().TrimEnd(',', ' ');
    }

    public ConcurrentBag<T> GetValue(
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Getting ConcurrentBag " + _memberName + " with count: " +
            _values.Count + " that has members of: " + GetConcurrentBagMembers(_values),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        return _values;
    }

    public void SetValue(ConcurrentBag<T> values,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Setting ConcurrentBag " + _memberName + " with count: " +_values.Count +
            " that has members of: " + GetConcurrentBagMembers(_values) + " TO: " + " with count: " +
            values.Count + " that has members of: " + GetConcurrentBagMembers(values),
            LogLevel.GET_VERBOSE, _filePath, "", _lineNumber);
        _values = values;
    }

    public void Add(T _item,
        [CallerFilePath] string _filePath = "",
        [CallerMemberName] string _memberName = "",
        [CallerLineNumber] int _lineNumber = 0)
    {
        Log.WriteLine("Adding item to ConcurrentBag " + _memberName + ": " + _item +
            " with count: " + _values.Count + " that has members of: " + GetConcurrentBagMembers(_values),
            LogLevel.ADD_VERBOSE, _filePath, "", _lineNumber);
        _values.Add(_item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public string GetConcurrentBagMembers(ConcurrentBag<T> _customValues)
    {
        StringBuilder membersBuilder = new StringBuilder();

        foreach (var item in _customValues)
        {
            switch (item)
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
                default:
                    Log.WriteLine("Tried to get type: " + typeof(T) + " unknown, undefined type?", LogLevel.CRITICAL);
                    break;
            }
        }

        return membersBuilder.ToString().TrimEnd(',', ' ');
    }


}
