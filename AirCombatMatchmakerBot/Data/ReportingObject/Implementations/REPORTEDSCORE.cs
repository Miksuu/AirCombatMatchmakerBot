using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class REPORTEDSCORE : BaseReportingObject
{
    public REPORTEDSCORE()
    {
        thisReportingObject.TypeOfTheReportingObject = TypeOfTheReportingObject.REPORTEDSCORE;
        thisReportingObject.CachedDefaultStatus = EmojiName.REDSQUARE;
        thisReportingObject.CurrentStatus = thisReportingObject.CachedDefaultStatus;

        thisReportingObject.AllowedMatchStatesToProcessOn = new ConcurrentBag<MatchState> {
            MatchState.REPORTINGPHASE,
        };
    }
    public override string ProcessTheReportingObjectAction(
        string _reportedObjectByThePlayer, ConcurrentDictionary<int, ReportData>? _TeamIdsWithReportData = null, int _reportingTeamId = 0)
    {
        base.SetObjectValue(_reportedObjectByThePlayer);
        return "You reported score of: " + _reportedObjectByThePlayer;
    }

    public override void CancelTheReportingObjectAction()
    {
        throw new NotImplementedException();
    }
}