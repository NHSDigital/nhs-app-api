namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ILinkedAccountsResultVisitor<out T>
    {
        T Visit(LinkedAccountsResult.Success result);
    }
}
