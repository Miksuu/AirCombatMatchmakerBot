using Microsoft.VisualBasic;
using System.Globalization;
using System.Text.Json;

public static class TimeService
{
    /*
    private const string TimeApiUrl = "http://worldtimeapi.org/api/timezone/etc/utc";

    public static async Task<DateTime> GetCurrentTime()
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(TimeApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var timeData = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(timeData);
                    var currentTime = DateTime.Parse(
                        apiResponse.utc_datetime, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime();
                    Log.WriteLine(currentTime.ToString());
                    return currentTime;
                }
                else
                {
                    Log.WriteLine("Failed to retrieve current time from the API. Response: " + response.StatusCode, LogLevel.CRITICAL);
                    throw new InvalidOperationException("Failed to retrieve current time from the API.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.WriteLine("An error occurred while retrieving the current time from the API: " + ex.Message, LogLevel.CRITICAL);
            throw;
        }

    
    public class ApiResponse
    {
        public string utc_datetime { get; set; }
    }
    }*/

    public static ulong CalculateTimeUntilWithUnixTime(ulong _timeUntil)
    {
        ulong timeDiff = _timeUntil - (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        return timeDiff;
    }

    public static string ConvertToZuluTimeFromUnixTime(ulong _unixTime)
    {
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime dateTime = unixEpoch.AddSeconds(_unixTime).ToUniversalTime();
        string formattedDateTime = dateTime.ToString("dd/MM/yyyy HHmm") + "z";
        return formattedDateTime;
    }


    // Second iteration generated by the GPT
    public static string ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlace(ulong _time)
    {
        var timeLeft = _time - (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

        if (timeLeft <= 0)
        {
            // If the time has already expired
            return "Expired";
        }
        else if (timeLeft <= 59)
        {
            // If the time is less than or equal to 59 seconds
            return $"{timeLeft} second{(timeLeft > 1 ? "s" : "")}";
        }
        else if (timeLeft <= 20 * 60)
        {
            // If the time is less than or equal to 20 minutes
            var minutes = timeLeft / 60;
            var seconds = timeLeft % 60;
            return $"{minutes} minute{(minutes > 1 ? "s" : "")} {seconds} second{(seconds > 1 ? "s" : "")}";
        }
        else if (timeLeft <= 24 * 60 * 60)
        {
            // If the time is less than or equal to 24 hours
            var hours = timeLeft / (60 * 60);
            var minutes = (timeLeft % (60 * 60)) / 60;

            if (hours > 0)
            {
                if (minutes > 0)
                {
                    return $"{hours} hour{(hours > 1 ? "s" : "")} {minutes} minute{(minutes > 1 ? "s" : "")}";
                }

                return $"{hours} hour{(hours > 1 ? "s" : "")}";
            }

            return $"{minutes} minute{(minutes > 1 ? "s" : "")}";
        }
        else if (timeLeft <= 7 * 24 * 60 * 60)
        {
            // If the time is less than or equal to 7 days
            var days = timeLeft / (24 * 60 * 60);
            var hours = (timeLeft % (24 * 60 * 60)) / (60 * 60);
            var minutes = (timeLeft % (60 * 60)) / 60;
            var timeLeftString = "";

            if (days > 0)
            {
                timeLeftString += $"{days} day{(days > 1 ? "s" : "")} ";
            }

            if (hours > 0)
            {
                timeLeftString += $"{hours} hour{(hours > 1 ? "s" : "")} ";
            }

            if (hours == 0)
            {
                timeLeftString += $"{minutes} minute{(minutes > 1 ? "s" : "")}";
            }

            return timeLeftString;
        }
        else
        {
            // For times more than 7 days
            var days = timeLeft / (24 * 60 * 60);
            var hours = (timeLeft % (24 * 60 * 60)) / (60 * 60);
            var timeLeftString = "";

            if (days > 0)
            {
                timeLeftString += $"{days} day{(days > 1 ? "s" : "")} ";
            }

            if (hours > 0)
            {
                timeLeftString += $"{hours} hour{(hours > 1 ? "s" : "")}";
            }

            return timeLeftString;
        }
    }

    public static DateTime? GetDateTimeFromUserInput(string _dateAndTime)
    {
        _dateAndTime = _dateAndTime.ToLower();

        // Parse the input date and time string
        if (_dateAndTime.Equals("now"))
        {
            DateTime currentDateTime = DateTime.UtcNow;
            DateTime newDateTime = currentDateTime.AddSeconds(300);
            return newDateTime;
        }

        else if (_dateAndTime.StartsWith("today "))
        {
            string timeString = _dateAndTime.Substring(6);
            DateTime currentDate = DateTime.UtcNow.Date;
            if (!TimeSpan.TryParseExact(timeString, new[] {
                @"hh\:mm'z'", @"hh'z'",
                @"hhmm'z'"
            }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
            {
                return null;
            }
            return currentDate.Add(timeComponent);
        }
        else if (_dateAndTime.StartsWith("tomorrow "))
        {
            string timeString = _dateAndTime.Substring(9);
            DateTime tomorrowDate = DateTime.UtcNow.Date.AddDays(1);
            if (!TimeSpan.TryParseExact(timeString, new[] {
                @"hh\:mm'z'", @"hh'z'",
                @"hhmm'z'"
            }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
            {
                return null;
            }
            return tomorrowDate.Add(timeComponent);
        }
        else if (_dateAndTime.EndsWith("today"))
        {
            DateTime currentDate = DateTime.UtcNow.Date;
            string timeString = _dateAndTime.Replace("today", "").Trim();
            if (!TimeSpan.TryParseExact(timeString, new[] {
                @"hh\:mm'z'", @"hh'z'",
                @"hhmm'z'"
            }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
            {
                return null;
            }
            return currentDate.Add(timeComponent);
        }
        else if (_dateAndTime.EndsWith("tomorrow"))
        {
            DateTime tomorrowDate = DateTime.UtcNow.Date.AddDays(1);
            string timeString = _dateAndTime.Replace("tomorrow", "").Trim();
            if (!TimeSpan.TryParseExact(timeString, new[] {
                @"hh\:mm'z'", @"hh'z'",
                @"hhmm'z'"
            }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
            {
                return null;
            }
            return tomorrowDate.Add(timeComponent);
        }
        else
        {
            bool regularFormatParse = DateTime.TryParseExact(_dateAndTime, new[] {
                "dd/MM/yyyy HH:mm'z'", "dd/MM/yyyy HH'z'",
                "dd/MM/yyyy HHmm'z'",
                "dd.MM.yyyy HH:mm'z'", "dd.MM.yyyy HH'z'",
                "dd.MM.yyyy HHmm'z'",
                "HH:mm'z' dd/MM/yyyy", "HH'z' dd/MM/yyyy",
                "HHmm'z' dd/MM/yyyy",
                "HH:mm'z' dd.MM.yyyy", "HH'z' dd.MM.yyyy",
                "HHmm'z' dd.MM.yyyy"
            },
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime suggestedScheduleDate);

            if (!regularFormatParse)
            {
                if (!TryParseWeekdayAndTime(_dateAndTime, out suggestedScheduleDate))
                {
                    Log.WriteLine(suggestedScheduleDate.ToString());

                    if (suggestedScheduleDate == DateTime.MinValue)
                    {
                        TimeSpan duration;
                        if (TryParseDuration(_dateAndTime, out duration))
                        {
                            suggestedScheduleDate = DateTime.UtcNow.Add(duration);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            // Assume the year is the current year if not provided
            if (suggestedScheduleDate.Year == 1)
            {
                suggestedScheduleDate = suggestedScheduleDate.AddYears(DateTime.UtcNow.Year - 1);
            }

            return suggestedScheduleDate;
        }
    }

    // Helper method to parse weekday and time, generated by GPT
    private static bool TryParseWeekdayAndTime(string input, out DateTime dateTime)
    {
        string[] daysOfWeek = { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
        string[] daysOfWeekShort = { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };

        // Check if the input contains a weekday
        foreach (string day in daysOfWeek)
        {
            string dayLower = day.ToLower();

            if (input.Contains(dayLower))
            {
                DateTime currentDate = DateTime.UtcNow.Date;
                int currentDayOfWeek = (int)currentDate.DayOfWeek;
                int targetDayOfWeek = Array.IndexOf(daysOfWeek, dayLower);

                if (targetDayOfWeek == -1)
                {
                    targetDayOfWeek = Array.IndexOf(daysOfWeekShort, dayLower.Substring(0, 3));
                }

                int daysToAdd = (targetDayOfWeek - currentDayOfWeek + 7) % 7;

                if (daysToAdd == 0 && currentDate.TimeOfDay.TotalSeconds > 0)
                {
                    // If the current time is past the specified time, move to the next week
                    daysToAdd = 7;
                }

                DateTime scheduledDate = currentDate.AddDays(daysToAdd);

                string timeString = input.Replace(dayLower, "").Trim();
                if (!TimeSpan.TryParseExact(timeString, new[] {
                @"hh\:mm\:ss'z'", @"hh\:mm'z'", @"hh'z'",
                @"hhmmss'z'", @"hhmm'z'"
            }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
                {
                    dateTime = default;
                    return false;
                }

                dateTime = scheduledDate.Add(timeComponent);
                return true;
            }
        }

        dateTime = default;
        return false;
    }

    private static bool TryParseDuration(string input, out TimeSpan duration)
    {
        duration = TimeSpan.Zero;

        var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts.Length % 2 != 0)
        {
            return false; // Invalid input format
        }

        try
        {
            for (int i = 0; i < parts.Length; i += 2)
            {
                int value;
                if (!int.TryParse(parts[i], out value))
                {
                    return false; // Failed to parse value
                }

                string unit = parts[i + 1].ToLower();

                if (unit.StartsWith("second"))
                {
                    duration = duration.Add(TimeSpan.FromSeconds(value));
                }
                else if (unit.StartsWith("minute"))
                {
                    duration = duration.Add(TimeSpan.FromMinutes(value));
                }
                else if (unit.StartsWith("hour"))
                {
                    duration = duration.Add(TimeSpan.FromHours(value));
                }
                else if (unit.StartsWith("day"))
                {
                    duration = duration.Add(TimeSpan.FromDays(value));
                }
                else
                {
                    return false; // Invalid unit
                }
            }
        }
        catch (Exception)
        {
            return false; // Failed to parse input
        }

        return true;
    }
}
