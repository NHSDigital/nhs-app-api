using NHSOnline.IntegrationTests.UI.Drivers.WebContext;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface INativeDriverWrapper : IDriverWrapper
    {
        WebContextStrategies Web { get; }
        void LoggedOutHomeScreenLoaded();
    }
}