using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public interface IAndroidContainer
    {
        internal IAndroidInteractor ContainerInteractor { get; }
    }
}