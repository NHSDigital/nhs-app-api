namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ISwitchAccountResultVisitor<out T>
    {
        T Visit(SwitchAccountResult.Success result);

        T Visit(SwitchAccountResult.Failure result);
    }
}