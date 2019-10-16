namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ILinkedAccountAccessSummaryResultVisitor<out T>
    {
        T Visit(LinkedAccountAccessSummaryResult.Success result);

        T Visit(LinkedAccountAccessSummaryResult.NotFound result);

        T Visit(LinkedAccountAccessSummaryResult.BadGateway result);
    }
}
