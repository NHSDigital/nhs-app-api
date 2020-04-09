namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ISwitchAccountResultVisitor<out T>
    {
        T Visit(SwitchAccountResult.Success result);

        T Visit(SwitchAccountResult.AlreadyAuthenticated result);

        T Visit(SwitchAccountResult.NotFound result);

        T Visit(SwitchAccountResult.Failure result);
    }
}