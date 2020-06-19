namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface INativeDriverWrapper : IDriverWrapper
    {
        INativeWebContext Web();
    }
}