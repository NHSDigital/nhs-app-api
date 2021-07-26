namespace NHSOnline.App.NhsLogin
{
    public interface IOnGpDemandReturnCheckResultVisitor<T>
    {
        T Visit(OnGpDemandReturnCheckResult.Complete complete);
    }
}