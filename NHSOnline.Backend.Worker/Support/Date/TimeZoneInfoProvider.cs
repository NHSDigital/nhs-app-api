using System;

namespace NHSOnline.Backend.Worker.Support.Date
{
    public class TimeZoneInfoProvider
    {
        private const string WindowsLondonTimeZone = "GMT Standard Time";
        private const string IanaLondon = "Europe/London";
        public TimeZoneInfo Create()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(WindowsLondonTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(IanaLondon);
                }
                catch (Exception)
                {
                    return TimeZoneInfo.Utc;
                }
                
            }
            catch (Exception)
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
