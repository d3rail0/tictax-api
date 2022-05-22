using System;

namespace tictax.api.Helpers
{
    public class DateHelper
    {
        public static DateTime UnixTimestampToDateTime(long unixTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            return dateTimeOffset.UtcDateTime;
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero);
            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}
