using System;
using System.Net.Http;
using System.Text.Json;

public static class TimeService
{
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
                    var currentTime = DateTime.Parse(apiResponse.utc_datetime, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime();
                    Log.WriteLine(currentTime.ToString(), LogLevel.VERBOSE);
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
    }

    public static ulong CalculateTimeUntilWithUnixTime(ulong _timeUntil)
    {
        ulong timeDiff = _timeUntil - (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        return timeDiff;
    }

    public class ApiResponse
    {
        public string utc_datetime { get; set; }
    }
}
