using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public interface IWebContainer
    {
        internal IWebInteractor ContainerInteractor { get; }
    }
}