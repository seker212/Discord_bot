namespace DiscordBot.Core.Helpers
{
    /// <summary>
    /// Helper for using <see cref="TimeZoneInfo"/> and strings.
    /// </summary>
    public interface ITimezoneHelper
    {
        /// <summary>
        /// Converts string value to a given timezone 
        /// </summary>
        /// <param name="timeZone">Valid time zone time</param>
        /// <returns>Returns TimeZoneInfo or null if timezone was incorrect</returns>
        TimeZoneInfo? ConvertTimeZoneFromString(string timeZone);

        /// <summary>
        /// Obtains current time in yyyy-mm-dd HH:mm:ss format
        /// </summary>
        /// <param name="timeZone">timezone to which current time should be converted</param>
        /// <returns>Current time converted to a timezone, if no timezone passed only formatted time</returns>
        string GetCurrentTimeZoneTime(TimeZoneInfo timeZone);
    }

    /// <inheritdoc cref="ITimezoneHelper"/>
    public class TimeZoneHelper : ITimezoneHelper
    {
        private const string DATETIME_FORMAT = "yyyy-mm-dd HH:mm:ss";

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

        public string GetCurrentTimeZoneTime(TimeZoneInfo? timeZone)
            => timeZone == null ? DateTime.Now.ToString(DATETIME_FORMAT) 
            : TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString(DATETIME_FORMAT);
    }
}
