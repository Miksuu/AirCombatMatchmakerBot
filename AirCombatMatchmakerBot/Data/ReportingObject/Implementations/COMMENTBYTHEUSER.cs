using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class COMMENTBYTHEUSER : BaseReportingObject
{
    public COMMENTBYTHEUSER()
    {
        thisReportingObject.TypeOfTheReportingObject = TypeOfTheReportingObject.COMMENTBYTHEUSER;
        thisReportingObject.CachedDefaultStatus = EmojiName.YELLOWSQUARE;
        thisReportingObject.CurrentStatus = thisReportingObject.CachedDefaultStatus;

        thisReportingObject.AllowedMatchStatesToProcessOn = new ConcurrentBag<MatchState> {
            MatchState.REPORTINGPHASE,
            MatchState.CONFIRMATIONPHASE,
        };
    }
    public override string ProcessTheReportingObjectAction(
        string _reportedObjectByThePlayer, ConcurrentDictionary<int, ReportData>? _TeamIdsWithReportData = null, int _reportingTeamId = 0)
    {
        base.SetObjectValue(_reportedObjectByThePlayer);
        return "You posted comment: " + _reportedObjectByThePlayer;
    }

    public override void CancelTheReportingObjectAction()
    {
        base.SetObjectValue("");
        thisReportingObject.CurrentStatus = EmojiName.YELLOWSQUARE;
    }
}