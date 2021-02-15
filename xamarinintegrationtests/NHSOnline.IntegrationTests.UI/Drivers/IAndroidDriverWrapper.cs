namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidDriverWrapper : INativeDriverWrapper, IAndroidInteractor
    {
        void PressTabKey();
        void PressEnterKey();
        void PressBackButton();

        void AssertNotRunningInForeground();
        void AssertRunningInForeground();
    }
}