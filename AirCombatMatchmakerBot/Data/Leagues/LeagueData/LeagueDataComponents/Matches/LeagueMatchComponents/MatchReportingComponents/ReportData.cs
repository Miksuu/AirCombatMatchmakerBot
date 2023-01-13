using System.Runtime.Serialization;

[DataContract]
public class ReportData
{
    public int ReportedResult
    {
        get
        {
            Log.WriteLine("Getting " + nameof(reportedResult), LogLevel.VERBOSE);
            return reportedResult;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(reportedResult)
                + " to: " + value, LogLevel.VERBOSE);
            reportedResult = value;
        }
    }

    [DataMember] private int reportedResult { get; set; }

    public ReportData(int _playerReportedResult)
    {
        reportedResult = _playerReportedResult;
    }
}