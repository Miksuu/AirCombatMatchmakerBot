using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class TACVIEWLINK : BaseReportingObject
{
    public TACVIEWLINK()
    {
        thisReportingObject.TypeOfTheReportingObject = TypeOfTheReportingObject.TACVIEWLINK;
        thisReportingObject.CachedDefaultStatus = EmojiName.REDSQUARE;
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

        // Makes the other tacview submission to be optional
        foreach (var item in _TeamIdsWithReportData)
        {
            if (item.Key != _reportingTeamId)
            {
                var tacviewReportingObject = item.Value.ReportingObjects.FirstOrDefault(
                    x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.TACVIEWLINK);

                var tacviewReportingObjectInterface = (InterfaceReportingObject)tacviewReportingObject;

                if (tacviewReportingObjectInterface.CurrentStatus == EmojiName.REDSQUARE)
                {
                    tacviewReportingObject.SetObjectValue(
                        tacviewReportingObjectInterface.ObjectValue, EmojiName.YELLOWSQUARE);
                }
            }
        }

        return "You posted tacview link: " + _reportedObjectByThePlayer;
    }

    public override void CancelTheReportingObjectAction()
    {
        throw new NotImplementedException();
    }
}