using System.Threading.Tasks;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidDriverWrapper : INativeDriverWrapper, IAndroidInteractor
    {
        WaitForAction PressBackButton();
        void AssertNotRunningInForeground();
        void AssertRunningInForeground();
        void PushTestFile();
        Task EnableAirplaneMode();
        Task ResetNetwork();
        void DismissKeyboard();
        void SendKey(int key);
        void CloseApp();
        void BackgroundApp();
        AndroidChromeApp OpenChromeApp();
        void LaunchApp();
    }
}