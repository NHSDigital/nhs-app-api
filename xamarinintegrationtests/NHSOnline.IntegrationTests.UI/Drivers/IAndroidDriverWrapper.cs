namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidDriverWrapper : INativeDriverWrapper, IAndroidInteractor
    {
        void PressEnterKey();
        void PressBackButton();

        void AssertNotRunningInForeground();
        void AssertRunningInForeground();
    }
}