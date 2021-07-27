namespace NHSOnline.App.NhsLogin
{
    public interface IOnDemandGpReturnCheckResultVisitor<T>
    {
        T Visit(OnDemandGpReturnCheckResult.Complete complete);
    }
}