namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ILinkedAccountsConfigResultVisitor<out T>
    {
        T Visit(LinkedAccountsConfigResult.Success result);   
    }
}