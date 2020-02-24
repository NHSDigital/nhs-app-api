namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity
{
    public interface ICreateJwtResultVisitor<out T>
    {
        T Visit(CreateJwtResult.Success result);

        T Visit(CreateJwtResult.InternalServerError result);
    }
}