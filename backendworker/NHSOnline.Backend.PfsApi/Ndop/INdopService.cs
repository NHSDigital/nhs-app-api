namespace NHSOnline.Backend.PfsApi.Ndop
{
    public interface INdopService
    {
        GetNdopResult GetJwtToken(string nhsNumber);
    }
}