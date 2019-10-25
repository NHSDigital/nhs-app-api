namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public interface ILinkedAccountsConfigResultVisitor<out T>
    {
        T Visit(LinkedAccountsConfigResult.Success result);   
    }
}