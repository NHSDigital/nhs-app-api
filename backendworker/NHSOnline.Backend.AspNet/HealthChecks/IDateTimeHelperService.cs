namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public interface IDateTimeHelperService
    {
        long GetUtcNowTimestampAsUnixTimeSeconds();
    }
}