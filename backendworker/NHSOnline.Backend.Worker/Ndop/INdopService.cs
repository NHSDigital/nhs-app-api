namespace NHSOnline.Backend.Worker.Ndop
{
    public interface INdopService
    {
        GetNdopResult GetJwtToken(string nhsNumber);
    }
}