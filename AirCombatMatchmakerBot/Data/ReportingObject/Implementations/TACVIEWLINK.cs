using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class TACVIEWLINK : BaseReportingObject
{
    public TACVIEWLINK()
    {
        thisReportingObject.TypeOfTheReportingObject = TypeOfTheReportingObject.REPORTEDSCORE;
        thisReportingObject.CachedDefaultStatus = EmojiName.YELLOWSQUARE;
        thisReportingObject.CurrentStatus = thisReportingObject.CachedDefaultStatus;

        thisReportingObject.AllowedMatchStatesToProcessOn = new ConcurrentBag<MatchState> {
            MatchState.REPORTINGPHASE,
            //MatchState.CONFIRMATIONPHASE, probably not necessary
        };
    }

    public override string ProcessTheReportingObjectAction(string _reportedObjectByThePlayer)
    {
        base.SetObjectValue(_reportedObjectByThePlayer);
        return "You posted tacview link: " + _reportedObjectByThePlayer;
    }

    public override void CancelTheReportingObjectAction()
    {
        throw new NotImplementedException();
    }
}