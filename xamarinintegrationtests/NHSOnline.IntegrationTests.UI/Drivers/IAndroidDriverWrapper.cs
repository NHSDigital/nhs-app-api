using System.Threading.Tasks;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidDriverWrapper : INativeDriverWrapper, IAndroidInteractor
    {
        WaitForAction PressBackButton();
        void AssertNotRunningInForeground();
        void AssertRunningInForeground();
        void PushTestFile();
        Task EnableAirplaneMode();
        void DismissKeyboard();
        void SendKey(int key);
    }
}