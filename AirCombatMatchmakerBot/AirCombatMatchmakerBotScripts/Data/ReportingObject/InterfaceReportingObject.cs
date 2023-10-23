using Newtonsoft.Json;
using System.Collections.Concurrent;

[JsonObjectAttribute]
public interface InterfaceReportingObject
{
    public TypeOfTheReportingObject TypeOfTheReportingObject { get; set; }
    public EmojiName CachedDefaultStatus { get; set; }
    public EmojiName CurrentStatus { get; set; }
    public bool HiddenBeforeConfirmation { get; set; }
    public string ObjectValue { get; set; }
    public ConcurrentBag<MatchState> AllowedMatchStatesToProcessOn { get; set; }

    public abstract string ProcessTheReportingObjectAction(
        string _reportedObjectByThePlayer, ConcurrentDictionary<int, ReportData>? _TeamIdsWithReportData = null, int _reportingTeamId = 0);
    public abstract void CancelTheReportingObjectAction();
}