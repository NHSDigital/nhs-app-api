using NHSOnline.Backend.PfsApi.SecondaryCare;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public interface ISecondaryCareWaitTimesResultVisitor<out T>
    {
        T Visit(SecondaryCareWaitTimesResult.Success result);
        T Visit(SecondaryCareWaitTimesResult.BadGateway _);
        T Visit(SecondaryCareWaitTimesResult.Timeout _);
        T Visit(SecondaryCareWaitTimesResult.NotEnabled _);
    }
}