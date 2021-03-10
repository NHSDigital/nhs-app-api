namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidDriverWrapper : INativeDriverWrapper, IAndroidInteractor
    {
        WaitForAction PressBackButton();
        void AssertNotRunningInForeground();
        void AssertRunningInForeground();
    }
}