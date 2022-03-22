using NHSOnline.Backend.PfsApi.SecondaryCare;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public interface ISecondaryCareSummaryResultVisitor<out T>
    {
        T Visit(SecondaryCareSummaryResult.Success result);
        T Visit(SecondaryCareSummaryResult.BadGateway _);
    }
}