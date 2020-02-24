namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity
{
    public interface IAssertedLoginIdentityService
    {
        CreateJwtResult CreateJwtToken(string idTokenJti);
    }
}