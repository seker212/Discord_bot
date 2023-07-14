namespace DiscordBot.Core.Helpers
{
    public interface ITimezoneHelper
    {
        TimeZoneInfo? ConvertTimeZoneFromString(string timeZone);
        string GetCurrentTimeZoneTime(TimeZoneInfo timeZone);
    }

    public class TimeZoneHelper : ITimezoneHelper
    {
        public static readonly string TIME_FORMAT = "yyyy-mm-dd HH:mm:ss";

        /// <summary>
        /// Converts string value to a given timezone 
        /// </summary>
        /// <param name="timeZone">Valid time zone time</param>
        /// <returns>Returns TimeZoneInfo or null if timezone was incorrect</returns>
        public TimeZoneInfo? ConvertTimeZoneFromString(string timeZone)
        {
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return zone;
            }
            catch (TimeZoneNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtains current time in TIME_FORMAT format
        /// </summary>
        /// <param name="timeZone">timezone to which current time should be converted</param>
        /// <returns>Current time converted to a timezone, if no timezone passed only formatted time</returns>
        public string GetCurrentTimeZoneTime(TimeZoneInfo? timeZone)
            => timeZone == null ? DateTime.Now.ToString(TIME_FORMAT) 
            : TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString(TIME_FORMAT);
    }
}
