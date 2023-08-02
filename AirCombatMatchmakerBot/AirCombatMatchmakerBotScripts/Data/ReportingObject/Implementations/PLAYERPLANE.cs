using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class PLAYERPLANE : BaseReportingObject
{
    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, UnitName> TeamMemberIdsWithSelectedPlanesByTheTeam
    {
        get => teamMemberIdsWithSelectedPlanesByTheTeam.GetValue();
        set => teamMemberIdsWithSelectedPlanesByTheTeam.SetValue(value);
    }

    public PLAYERPLANE()
    {
        thisReportingObject.TypeOfTheReportingObject = TypeOfTheReportingObject.PLAYERPLANE;
        thisReportingObject.CachedDefaultStatus = EmojiName.WHITECHECKMARK;
        thisReportingObject.CurrentStatus = thisReportingObject.CachedDefaultStatus;
        thisReportingObject.HiddenBeforeConfirmation = true;

        thisReportingObject.AllowedMatchStatesToProcessOn = new ConcurrentBag<MatchState> {
            MatchState.PLAYERREADYCONFIRMATIONPHASE,
        };
    }

    [DataMember]
    private logConcurrentDictionary<ulong, UnitName> teamMemberIdsWithSelectedPlanesByTheTeam =
        new logConcurrentDictionary<ulong, UnitName>();

    public string GetTeamPlanes()
    {
        Log.WriteLine("Starting to " + nameof(GetTeamPlanes) + " with count: " +
            teamMemberIdsWithSelectedPlanesByTheTeam.Count());

        StringBuilder membersBuilder = new StringBuilder();
        foreach (var item in teamMemberIdsWithSelectedPlanesByTheTeam)
        {
            //UnitName objectValueEnum = (UnitName)Enum.Parse(typeof(UnitName), item.Value.ToString());
            Log.WriteLine("objectValue: " + item.Value);
            string unitNameEnumMemberValue = EnumExtensions.GetEnumMemberAttrValue(item.Value);
            Log.WriteLine("unitNameEnumMemberValue: " + unitNameEnumMemberValue);
            membersBuilder.Append(unitNameEnumMemberValue).Append(", ");
        }
        return membersBuilder.ToString().TrimEnd(',', ' ');
    }

    public override string ProcessTheReportingObjectAction(
        string _reportedObjectByThePlayer, ConcurrentDictionary<int, ReportData>? _TeamIdsWithReportData = null, int _reportingTeamId = 0)
    {
        base.SetObjectValue(_reportedObjectByThePlayer);
        return "You selected plane: " + _reportedObjectByThePlayer;
    }

    public override void CancelTheReportingObjectAction()
    {
        throw new NotImplementedException();
    }
}