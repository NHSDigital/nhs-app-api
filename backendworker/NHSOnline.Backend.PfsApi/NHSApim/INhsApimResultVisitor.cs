namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public interface INhsApimResultVisitor<out T>
    {
        T Visit(GetAuthTokenResult.Success result);

        T Visit(GetAuthTokenResult.Unauthorized result);
    }
}