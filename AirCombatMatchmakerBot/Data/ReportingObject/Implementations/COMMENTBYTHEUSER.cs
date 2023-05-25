using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class COMMENTBYTHEUSER : BaseReportingObject
{
    public COMMENTBYTHEUSER()
    {
        thisReportingObject.TypeOfTheReportingObject = TypeOfTheReportingObject.REPORTEDSCORE;
        thisReportingObject.CachedDefaultStatus = EmojiName.REDSQUARE;
        thisReportingObject.CurrentStatus = thisReportingObject.CachedDefaultStatus;

        thisReportingObject.AllowedMatchStatesToProcessOn = new ConcurrentBag<MatchState> {
            MatchState.REPORTINGPHASE,
            MatchState.CONFIRMATIONPHASE,
        };
    }
    public override string ProcessTheReportingObjectAction(string _reportedObjectByThePlayer)
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