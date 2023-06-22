using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class MATCHSCHEDULINGMESSAGE : BaseMessage
{
    MatchChannelComponents mcc { get; set; }
    public MATCHSCHEDULINGMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHSCHEDULINGMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
            });

        thisInterfaceMessage.MessageEmbedTitle = "Match Scheduling";
        thisInterfaceMessage.MessageDescription = "Schedule the match here";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        try
        {
            mcc = new MatchChannelComponents(this);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                    nameof(mcc.leagueMatchCached) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return errorMsg;
            }

            var scheduleObject = mcc.leagueMatchCached.ScheduleObject;
            var teamsInTheMatch = mcc.leagueMatchCached.TeamsInTheMatch;
            if (!teamsInTheMatch.ContainsKey(scheduleObject.TeamIdThatRequestedScheduling))
            {
                return thisInterfaceMessage.MessageDescription;
            }

            
            var requestedTime = TimeService.ConvertToDateTimeFromUnixTime(scheduleObject.RequestedSchedulingTimeInUnixTime);

            var teamNameThatScheduled = teamsInTheMatch.First(
                t => t.Key == scheduleObject.TeamIdThatRequestedScheduling).Value;
            /*
            string time = TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlace(requestedTime);
            Log.WriteLine("time: " + time, LogLevel.VERBOSE);
            Log.WriteLine("teamNameThatScheduled: " + teamNameThatScheduled, LogLevel.VERBOSE);*/

            return requestedTime + " requested by team: " + teamNameThatScheduled;
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return ex.Message;
        }

    }
}