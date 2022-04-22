namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public interface INhsApimResultVisitor<out T>
    {
        T Visit(GetAuthTokenResult.Success result);

        T Visit(GetAuthTokenResult.BadRequest result);

        T Visit(GetAuthTokenResult.Forbidden result);
    }
}